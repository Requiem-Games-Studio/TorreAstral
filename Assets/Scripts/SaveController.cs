using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class SaveController : MonoBehaviour
{
    public GameObject player;
    public SaveData data;
    public ChunkManagerByName chunkManager;
    //public WorldMapManager chunkManager;

    void Start()
    {
        data = SaveManager.Instance.currentData;
        //LoadData();
    }

    public void LoadData(GameObject playerHost)
    {
        player = playerHost;
        //Posicion del jugador Cargado desde PlayerSpawner
        Debug.Log("Load Chunks" + data.exploredChunks);
        //chunkManager.exploredChunks = new HashSet<Vector2Int>(data.exploredChunks);
        //chunkManager.CheckStatusMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5) && player !=null)
        {
            SavePlayerData();
        }
    }


    public void SavePlayerData()
    {
        if (player == null) return;
        // Llenar datos
        data.playTime += Time.deltaTime;
        data.playProgress = GetProgress();
        data.playerPosition = player.transform.position;
        data.health = GetHealth();

        // NUEVO: Guardar el estado actual de los chunks que están en pantalla
        if (chunkManager != null)
        {
            chunkManager.SaveAllActiveChunks();
        }
        //Debug.Log("Chunks guardados: " + chunkManager.exploredChunks.Count);
        //data.exploredChunks = chunkManager.exploredChunks.ToList();

        // Guardar en disco
        SaveManager.Instance.SaveGame(
                SaveManager.Instance.currentSlot,
                data
            );

        Debug.Log("Juego guardado correctamente");
    }

    float GetProgress() => 0.5f;
    int GetHealth() => 100;
    int GetCollectables() => 250;
}
