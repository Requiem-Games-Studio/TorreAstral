using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectData : MonoBehaviour
{
    public TMPro.TextMeshProUGUI[] time;
    public TMPro.TextMeshProUGUI[] progress;
    public TMPro.TextMeshProUGUI[] newName;



    private void Start()
    {
        CheckSlot();
    }

    public void CheckSlot()
    {
        for (int i = 0; i < 3; i++) // 4 slots
        {
            if (SaveManager.Instance.SaveExists(i))
            {
                SaveData data = SaveManager.Instance.LoadGame(i);

                Debug.Log("Slot " + i + " ocupado");
                newName[i].text = "" + data.playerName;
                time[i].text = "Time: " + data.playTime.ToString("F1") + "s";
                progress[i].text = "" + data.playProgress + "%";
            }
            else
            {
                Debug.Log("Slot " + i + " vacío");

                newName[i].text = "";
                time[i].text = "";
                progress[i].text = "";
            }
        }
    }

    public void SelectSlot(int slotIndex)
    {
        if (SaveManager.Instance.SaveExists(slotIndex))
        {
            Debug.Log("Cargando partida del slot " + slotIndex);
            SaveManager.Instance.currentSlot = slotIndex;
            SaveManager.Instance.currentData = SaveManager.Instance.LoadGame(slotIndex);
        }
        else
        {
            Debug.Log("Slot vacío, creando nueva partida...");
            SaveData newData = new SaveData();
            newData.playerName = "";
            newData.playTime = 0f;
            newData.playProgress = 0f;
            newData.playerPosition = new Vector2(0, 0);

            SaveManager.Instance.SaveGame(slotIndex, newData);

            SaveManager.Instance.currentSlot = slotIndex;
            SaveManager.Instance.currentData = newData;
        }

        SceneManager.LoadScene("Game");
    }

    public void DeletSlot(int slotID)
    {
        SaveManager.Instance.DeleteSlot(slotID);
        newName[slotID].text = "";
        time[slotID].text = "";
        progress[slotID].text = "";

    }

}
