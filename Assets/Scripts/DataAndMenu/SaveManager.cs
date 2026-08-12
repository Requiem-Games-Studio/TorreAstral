using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditor.Overlays;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public int currentSlot;
    public SaveData currentData;

    public Vector2 defaultStartPosition = new Vector2(-547, -48.4f);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Crear partida nueva
    public void NewGame(int slotIndex)
    {
        currentSlot = slotIndex;
        currentData = new SaveData();

        // Posición inicial correcta
        currentData.playerPosition = defaultStartPosition;

        SaveGame();
    }

    // Guardar usando slot actual
    public void SaveGame()
    {
        if (currentData == null) return;

        string path = GetSlotPath(currentSlot);
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(path, json);
    }

    // Guardar con parámetros (como ya lo tienes)
    public void SaveGame(int slotIndex, SaveData data)
    {
        string path = GetSlotPath(slotIndex);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    // Cargar slot y preparar datos
    public void LoadSlot(int slotIndex)
    {
        currentSlot = slotIndex;
        currentData = LoadGame(slotIndex);

        if (currentData == null)
        {
            Debug.Log("No había save, creando nuevo...");
            currentData = new SaveData();
            SaveGame();
        }

        if (currentData.takenCollectables == null)
            currentData.takenCollectables = new List<string>();
    }

    // Cargar archivo directo
    public SaveData LoadGame(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            Debug.LogWarning("No hay datos en el slot " + slotIndex);
            return null;
        }
    }

    // Verificar si existe save
    public bool SaveExists(int slotIndex)
    {
        return File.Exists(GetSlotPath(slotIndex));
    }

    // Ruta del slot
    private string GetSlotPath(int slotIndex)
    {
        return Application.persistentDataPath + "/saveSlot" + slotIndex + ".json";
    }

    public void DeleteSlot(int slotIndex)
    {
        string path = GetSlotPath(slotIndex);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log("Slot " + slotIndex + " borrado");
        }
    }
}
