using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelectButton : MonoBehaviour
{
    public string sceneToLoad;

    public void EnterLevel()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
