using Fusion;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : NetworkBehaviour
{

    public NetworkObject[] enemyPrefab;
    public List<int> idSpawnList;

    public void SpawnBychunks(SpawnPoints spawnPoints)
    {

        foreach (Transform point in spawnPoints.point)
        {

            string[] data = point.name.Split('_');

            int idSpawn = int.Parse(data[1]);
            string type = data[2];
            int idObject = int.Parse(data[3]);

            //Debug.Log("Spawn: " + $"ID: {idSpawn}" + $"Tipo: {type}" + $"ID Object: {idObject}");

            // Ya existe un enemigo en este punto
            if (idSpawnList.Contains(idSpawn))
                continue;

            NetworkObject enemy = Runner.Spawn(
                enemyPrefab[idObject],
                point.position,
                point.rotation
            );

            idSpawnList.Add(idSpawn);
        }
    }
}
