using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject menu,equipment,connectionPanel;
    public SaveController saveController;


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
        SaveGame();
        Application.Quit();
    }

    public void MainMenu()
    {
        SaveGame();
        //SceneManager.LoadScene("Menu");
    }

    public void SaveGame()
    {              
        saveController = GameObject.FindGameObjectWithTag("Runner").GetComponent<SaveController>();
        
        if(saveController != null )
        {
            saveController.SavePlayerData();
        }
    }

    public void UseConnectionPanel()
    {
        connectionPanel.SetActive(!connectionPanel.activeSelf);
    }
}
