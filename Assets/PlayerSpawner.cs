using UnityEngine;
using Fusion;
using static Unity.Collections.Unicode;


public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
{
    public GameObject playerPrefab;

    public void PlayerJoined(PlayerRef player)
    {
        if (Runner.IsServer)
        {
            Runner.Spawn(
                playerPrefab,
                new Vector3(0, 1, 0),
                Quaternion.identity,
                player
            );
        }
    }
}
