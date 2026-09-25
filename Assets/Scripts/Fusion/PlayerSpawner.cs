using UnityEngine;
using Fusion;
using static Unity.Collections.Unicode;


public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;
    public ChunkManagerByName chunkManager;

    private bool saveLoaded = false;
    public SaveController saveController;

    public void PlayerJoined(PlayerRef player)
    {
        NetworkObject playerObject = null;

        if (Runner.IsServer)
        {
            playerObject = Runner.Spawn(
                playerPrefab,
                saveController.data.playerPosition,
                Quaternion.identity,
                player
            );

            chunkManager.CheckPlayers();
        }

        // Solo el host carga los datos una vez
        if (!saveLoaded && player == Runner.LocalPlayer)
        {
            saveLoaded = true;
            Debug.Log("El HOST ha entrado. Cargando datos...");
            saveController.LoadData(playerObject.gameObject);
        }
    }

}
