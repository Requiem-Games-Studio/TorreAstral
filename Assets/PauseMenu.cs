using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject menu;


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(!menu.activeSelf);
        }
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
