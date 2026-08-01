using System.Collections.Generic;
using UnityEngine;

public class ChunkManagerByName : MonoBehaviour
{
    [Header("Chunk Settings")]
    public GameObject[] chunkPrefabs;
    public int chunkWidth = 34;
    public int chunkHeight = 24;
    public int loadRadius = 1;

    public Transform player;
    public float xOffset, yOffset;

    private Dictionary<Vector2Int, GameObject> loadedChunks = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> chunkPrefabMap = new Dictionary<Vector2Int, GameObject>();

    private Vector2Int currentPlayerChunk;

    void Awake()
    {
        InitializeChunkMap();
    }

    public void StartChunkManager()
    {
        //InitializeChunkMap();

        if (player != null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }       
        currentPlayerChunk = GetPlayerChunk();
        UpdateChunks();
    }

    void Update()
    {
        Vector2Int newChunk = GetPlayerChunk();

        if (newChunk != currentPlayerChunk)
        {
            currentPlayerChunk = newChunk;
            UpdateChunks();
        }
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

    void UpdateChunks()
    {
        Vector2Int playerChunk = currentPlayerChunk;

        // CARGAR CHUNKS
        for (int x = -loadRadius; x <= loadRadius; x++)
        {
            for (int y = -loadRadius; y <= loadRadius; y++)
            {
                Vector2Int chunkCoord = new Vector2Int(playerChunk.x + x, playerChunk.y + y);

                if (!loadedChunks.ContainsKey(chunkCoord) && chunkPrefabMap.ContainsKey(chunkCoord))
                {
                    LoadChunk(chunkCoord);
                }
            }
        }

        // DESCARGAR CHUNKS (logica cuadrada consistente)
        List<Vector2Int> chunksToUnload = new List<Vector2Int>();

        foreach (var chunk in loadedChunks.Keys)
        {
            int dx = Mathf.Abs(chunk.x - playerChunk.x);
            int dy = Mathf.Abs(chunk.y - playerChunk.y);

            if (dx > loadRadius || dy > loadRadius)
            {
                chunksToUnload.Add(chunk);
            }
        }

        foreach (var chunkCoord in chunksToUnload)
        {
            UnloadChunk(chunkCoord);
        }
    }

    Vector2Int GetPlayerChunk()
    {
        if (player == null)
        {
            return Vector2Int.zero;
        }

        int chunkX = Mathf.FloorToInt((player.position.x + xOffset) / chunkWidth);
        int chunkY = Mathf.FloorToInt((player.position.y + yOffset) / chunkHeight);

        return new Vector2Int(chunkX, chunkY);
    }

    void LoadChunk(Vector2Int chunkCoord)
    {
        Vector3 chunkPosition = new Vector3(chunkCoord.x * chunkWidth, chunkCoord.y * chunkHeight, 0);

        GameObject chunkPrefab = chunkPrefabMap[chunkCoord];

        GameObject newChunk = Instantiate(chunkPrefab, chunkPosition, Quaternion.identity);
        newChunk.name = "" + chunkCoord;

        loadedChunks.Add(chunkCoord, newChunk);

    }

    void UnloadChunk(Vector2Int chunkCoord)
    {
        if (loadedChunks.TryGetValue(chunkCoord, out GameObject chunk))
        {
            Destroy(chunk);
            loadedChunks.Remove(chunkCoord);
        }
    }
}
