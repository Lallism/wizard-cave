using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public Text scoreText;

    private void OnEnable()
    {
        if (scoreText != null)
        {
            scoreText.text = "FINAL SCORE: " + GameManager.instance.score.ToString();
        }
    }

    public void ContinueGame()
    {
        GameManager.instance.TogglePause();
    }

    public void MainMenu()
    {
        GameManager.instance.TogglePause();
        SceneManager.LoadScene("LevelSelect");
    }
}
