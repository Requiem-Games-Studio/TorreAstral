using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public string gameScene;

    public void PlayGame()
    {
        SceneManager.LoadScene(gameScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
