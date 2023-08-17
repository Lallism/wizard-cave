using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public class KeyIcon
    {
        public GameObject icon;
        public string name;

        public KeyIcon(GameObject iconObject, string keyName)
        {
            icon = iconObject;
            name = keyName;
        }
    }

    public static GameManager instance;

    public bool gamePaused = false;

    public GameObject pauseMenu;
    public GameObject levelCompleteUI;
    public GameObject keyIcon;
    public Transform keyList;
    public int score;
    public Text scoreText;
    public Image healthBar;
    public Image manaBar;
    public Image selectionBorder;
    public Image[] spellIcons;
    public Image[] cooldowns;
    public Image interactPrompt;
    public Image interactPromptBg;
    [HideInInspector] public Player player;

    private List<KeyIcon> keyIcons = new List<KeyIcon>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Too many GameManagers in the scene");
            return;
        }

        instance = this;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        InvokeRepeating("UpdateUI", .2f, .2f);
    }

    private void Update()
    {
        UpdateCooldowns();

        if(Input.GetButtonDown("Cancel"))
        {
            TogglePause();
        }
    }

    public void SpawnParticles(ParticleSystem particleSystem, Vector3 location)
    {
        ParticleSystem particles = Instantiate(particleSystem, location, Quaternion.identity);
        Destroy(particles.gameObject, particles.main.duration);
    }

    public void KillPlayer()
    {
        player.gameObject.SetActive(false);
        //Destroy(player.gameObject);
        Invoke("RestartGame", 3f);
    }

    public void UpdateUI()
    {
        scoreText.text = "SCORE: " + score.ToString();
        healthBar.rectTransform.localScale = new Vector3(Mathf.Max(player.currentHealth / player.maxHealth, 0), 1, 1);
        manaBar.rectTransform.localScale = new Vector3(Mathf.Max(player.currentMana / player.maxMana, 0), 1, 1);
    }

    public void SetBorder()
    {
        Vector3 selectedSpell = spellIcons[player.activeSlot].transform.position;
        selectionBorder.transform.position = selectedSpell;
    }

    public void SetSpellIcon(Sprite sprite, int slot)
    {
        spellIcons[slot].sprite = sprite;
    }

    public void AddToKeyList(Key keyToAdd)
    {
        GameObject key = Instantiate(keyIcon, keyList);
        key.GetComponent<Image>().sprite = keyToAdd.sprite;
        keyIcons.Add(new KeyIcon(key, keyToAdd.keyName));
    }

    public void RemoveFromKeyList(Key keyToRemove)
    {
        KeyIcon keyIcon = keyIcons.Find(x => x.name == keyToRemove.keyName);
        Destroy(keyIcon.icon.gameObject);
        keyIcons.Remove(keyIcon);
    }

    private void UpdateCooldowns()
    {
        for (int i = 0; i < cooldowns.Length; i++)
        {
            if (player.equippedSpells[i].spell != null)
            {
                cooldowns[i].fillAmount = player.equippedSpells[i].currentCooldown / player.equippedSpells[i].spell.cooldown;
            }
        }
    }

    public void TogglePause()
    {
        if (!gamePaused)
        {
            gamePaused = true;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else if (pauseMenu.activeInHierarchy)
        {
            gamePaused = false;
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }

    public void LevelComplete()
    {
        gamePaused = true;
        player.gameObject.SetActive(false);
        levelCompleteUI.SetActive(true);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}
