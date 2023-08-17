using UnityEngine;
using System.Collections;

public class Enemy : Character
{
    public Spell spell;
    public float minCooldown;
    public float maxCooldown;
    public int score = 100;
    [ColorUsage(true, true)]
    public Color flashColor = new Color(1, 1, 1, 1);

    private float currentCooldown;
    private SpriteRenderer sprite;
    private Color baseGlow;
    private Player player;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        baseGlow = sprite.material.GetColor("_Glow");
    }

    public override void Start()
    {
        base.Start();
        currentCooldown = Random.Range(minCooldown, maxCooldown);
        player = GameManager.instance.player;
    }

    private void Update()
    {
        if (spell != null)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0)
            {
                spell.ActivateSpell(this, castPoint.position, GameManager.instance.player.transform, "Player");
                currentCooldown = Random.Range(minCooldown, maxCooldown);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Character character = collision.GetComponent<Character>();

        if (collision.CompareTag("Player") && !character.invulnerable)
        {
            character.TakeDamage(meleeDamage);
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        base.TakeDamage(damageAmount);
        if (gameObject.activeSelf)
        {
            StopCoroutine(Flash());
            StartCoroutine(Flash());
        }
    }

    public override void Die()
    {
        GameManager.instance.score += score;
        base.Die();
    }

    private IEnumerator Flash()
    {
        sprite.material.SetColor("_Glow", flashColor);
        yield return new WaitForSeconds(0.1f);
        sprite.material.SetColor("_Glow", baseGlow);
    }
}
