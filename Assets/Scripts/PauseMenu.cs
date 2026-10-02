using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{

    public GameObject menu,equipment;
    public SaveController saveController;

    [HideInInspector]
    public string playerMode = "Single";


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
        saveController = GameObject.Find(playerMode).GetComponent<SaveController>();     
        
        if(saveController != null )
        {
            saveController.SavePlayerData();
        }
    }
}
