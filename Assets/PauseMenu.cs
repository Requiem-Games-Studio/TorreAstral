using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    public GameObject menu,equipment;


    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetActive(!menu.activeSelf);
        }


        if (Input.GetKeyDown(KeyCode.I))
        {
            equipment.SetActive(!equipment.activeSelf);
        }
    }


    public void ExitGame()
    {
        Application.Quit();
    }
}
