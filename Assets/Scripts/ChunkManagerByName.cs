using System.Collections.Generic;
using UnityEngine;
using System.Linq; // Necesario para comparar los conjuntos de forma eficiente
using Fusion;
using Photon.Realtime;
using System;

public class ChunkManagerByName : NetworkBehaviour
{
    [Header("Chunk Settings")]
    public GameObject[] chunkPrefabs;
    public int chunkWidth = 34;
    public int chunkHeight = 24;
    public int loadRadius = 1;

    // Cambia el array por List<Transform>
    public List<Transform> players = new List<Transform>();
    public float xOffset, yOffset;

    private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> chunkPrefabMap = new Dictionary<Vector2Int, GameObject>();

    private HashSet<Vector2Int> currentPlayersChunks = new HashSet<Vector2Int>();

    public NetworkObject[] objectSpawned;

    private Dictionary<string, NetworkObject> prefabDictionary = new Dictionary<string, NetworkObject>();

    public SaveData data;

    public override void Spawned()
    {
        Debug.Log("Spawn Manager");        
        CheckPlayers();
        InitializeChunkMap();

        // Llenamos el diccionario cuando el NetworkBehaviour aparece en la red
        prefabDictionary.Clear();
        foreach (NetworkObject prefab in objectSpawned)
        {
            if (prefab != null && !prefabDictionary.ContainsKey(prefab.name))
            {
                prefabDictionary.Add(prefab.name, prefab);
            }
        }
    }

    public void CheckPlayers()
    {
        GameObject[] newPlayer = GameObject.FindGameObjectsWithTag("Player");
        for (int i = 0; i < newPlayer.Length; i++)
        {
            if (!players.Contains(newPlayer[i].transform))
            {
                players.Add(newPlayer[i].transform);
            }
        }      
        StartChunkManager();
    }


    public void StartChunkManager()
    {
        // Guardamos los chunks iniciales
        currentPlayersChunks = GetAllPlayerChunks();
        UpdateChunks();
    }

    void Update()
    {
        // 1. Obtenemos el conjunto actual de chunks donde hay jugadores
        HashSet<Vector2Int> newPlayersChunks = GetAllPlayerChunks();

        // 2. Comparamos si el conjunto cambió con respecto al último frame
        if (!AreChunkSetsEqual(currentPlayersChunks, newPlayersChunks))
        {
            currentPlayersChunks = newPlayersChunks;
            UpdateChunks();
        }
    }
    // Método auxiliar para comparar si dos HashSet contienen exactamente las mismas posiciones
    private bool AreChunkSetsEqual(HashSet<Vector2Int> setA, HashSet<Vector2Int> setB)
    {
        if (setA == null || setB == null) return setA == setB;
        if (setA.Count != setB.Count) return false;

        // SetEquals verifica si ambos conjuntos tienen los mismos elementos sin importar el orden
        return setA.SetEquals(setB);
    }

    void InitializeChunkMap()
    {
        foreach (GameObject prefab in chunkPrefabs)
        {
            if (TryGetCoords(prefab.name, out Vector2Int coords))
            {
                if (!chunkPrefabMap.ContainsKey(coords))
                {
                    chunkPrefabMap.Add(coords, prefab);
                }
                else
                {
                    Debug.LogWarning("Chunk duplicado en coordenada: " + coords);
                }
            }
            else
            {
                Debug.LogError("Nombre inválido: " + prefab.name + " (Debe ser x,y)");
            }
        }
    }

    bool TryGetCoords(string name, out Vector2Int coords)
    {
        coords = Vector2Int.zero;

        string[] split = name.Split(',');
        if (split.Length != 2) return false;

        if (!int.TryParse(split[0], out int x)) return false;
        if (!int.TryParse(split[1], out int y)) return false;

        coords = new Vector2Int(x, y);
        return true;
    }

    public void UpdateChunks()
    {

        // 1. Crear un conjunto con TODOS los chunks que deben estar cargados en el mapa
        // considerando el radio alrededor de CADA jugador.
        HashSet<Vector2Int> requiredChunks = new HashSet<Vector2Int>();

        foreach (Vector2Int pChunk in currentPlayersChunks)
        {
            for (int x = -loadRadius; x <= loadRadius; x++)
            {
                for (int y = -loadRadius; y <= loadRadius; y++)
                {
                    Vector2Int chunkCoord = new Vector2Int(pChunk.x + x, pChunk.y + y);
                    requiredChunks.Add(chunkCoord); // Si ya existe en el HashSet, no se duplica
                }
            }
        }

        // 2. CARGAR CHUNKS
        // Iteramos solo sobre los chunks que acabamos de determinar que deben estar activos
        foreach (Vector2Int chunkCoord in requiredChunks)
        {
            if (!loadedChunks.ContainsKey(chunkCoord) && chunkPrefabMap.ContainsKey(chunkCoord))
            {
                LoadChunk(chunkCoord);
            }
        }

        // 3. DESCARGAR CHUNKS
        // Si un chunk cargado NO está en la lista de chunks requeridos por ningún jugador, se descarga.
        List<Vector2Int> chunksToUnload = new List<Vector2Int>();

        foreach (var loadedChunkCoord in loadedChunks.Keys)
        {
            if (!requiredChunks.Contains(loadedChunkCoord))
            {
                chunksToUnload.Add(loadedChunkCoord);
            }
        }

        foreach (var chunkCoord in chunksToUnload)
        {
            UnloadChunk(chunkCoord);
        }
    }

    // Tu función de obtención de chunks para múltiples jugadores
    private HashSet<Vector2Int> GetAllPlayerChunks()
    {
        HashSet<Vector2Int> activeChunks = new HashSet<Vector2Int>();

        foreach (Transform p in players) // Asegúrate de tener tu array/lista 'players'
        {
            if (p == null) continue;

            int chunkX = Mathf.FloorToInt((p.position.x + xOffset) / chunkWidth);
            int chunkY = Mathf.FloorToInt((p.position.y + yOffset) / chunkHeight);

            activeChunks.Add(new Vector2Int(chunkX, chunkY));
        }

        return activeChunks;
    }

    void LoadChunk(Vector2Int chunkCoord)
    {
        Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkWidth, chunkCoord.y * chunkHeight, 0);

        GameObject chunkPrefab = chunkPrefabMap[chunkCoord];

        GameObject newChunk = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
        newChunk.name = "" + chunkCoord;

        loadedChunks.Add(chunkCoord, newChunk);



        // Obtienes la lista de objetos guardados en este chunk
        List<SavedObject> objectsInChunk = data.GetObjectsInChunk(chunkCoord);

        if (objectsInChunk.Count > 0)
        {
            // Le pasas la lista completa a tu método de spawneo
            SpawnSavedObjects(objectsInChunk);
        }       
    }

    public void SpawnSavedObjects(List<SavedObject> objetosGuardados)
    {
        // Solo el Host o StateAuthority debe spawnear en red
        if (!HasStateAuthority) return;

        foreach (SavedObject objeto in objetosGuardados)
        {
            if (prefabDictionary.TryGetValue(objeto.nombre, out NetworkObject prefab))
            {
                Runner.Spawn(prefab, objeto.posicion, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning($"No se encontró el prefab en el diccionario: {objeto.nombre}");
            }
        }
    }

    public void SaveAllActiveChunks()
    {
        data = SaveManager.Instance.currentData;

        // Recorremos solo los chunks que están activos en escena en este instante
        foreach (var kvp in loadedChunks)
        {
            Vector2Int chunkCoord = kvp.Key;
            GameObject chunkGO = kvp.Value;

            if (chunkGO != null)
            {
                SaveObjectsByChunk chunkScript = chunkGO.GetComponent<SaveObjectsByChunk>();
                if (chunkScript != null)
                {
                    // Escaneamos los objetos actuales de este chunk
                    List<SavedObject> savedObjects = chunkScript.SaveEnemiesAndItems();

                    // Actualizamos la lista global en el SaveData
                    data.SaveObjectsInChunk(chunkCoord, savedObjects);
                }
            }
        }
    }

    void UnloadChunk(Vector2Int chunkCoord)
    {
        data = SaveManager.Instance.currentData;

        if (loadedChunks.TryGetValue(chunkCoord, out GameObject chunk))
        {
            SaveObjectsByChunk chunkScript = chunk.GetComponent<SaveObjectsByChunk>();

            if (chunkScript != null)
            {
                // 1. Ejecutas el escaneo y obtienes la lista de objetos
                List<SavedObject> savedObjects = chunkScript.SaveEnemiesAndItems();

                // 2. Guardas la lista en tu SaveData para esta coordenada
                data.SaveObjectsInChunk(chunkCoord, savedObjects);
            }


            Destroy(chunk);
            loadedChunks.Remove(chunkCoord);
        }
    }
}
