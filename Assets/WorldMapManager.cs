using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapManager : MonoBehaviour
{
    public Transform calabozoMaps;

    public HashSet<Vector2Int> exploredChunks = new HashSet<Vector2Int>();
    public GameObject[] map0;

    // Diccionario para acceso rápido a los chunks del minimapa
    public Dictionary<Vector2Int, Transform> minimapChunks = new Dictionary<Vector2Int, Transform>();

    public static WorldMapManager Instance;



    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        RegisterMinimapChunks();
        CheckStatusMap();
    }

    void RegisterMinimapChunks()
    {
        foreach (Transform chunk in calabozoMaps)
        {
            string[] parts = chunk.name.Split(',');

            if (parts.Length == 2)
            {
                int x = int.Parse(parts[0]);
                int y = int.Parse(parts[1]);

                Vector2Int coord = new Vector2Int(x, y);

                if (!minimapChunks.ContainsKey(coord))
                {
                    minimapChunks.Add(coord, chunk);
                }
            }
        }
    }

    public void CheckStatusMap()
    {
        foreach (Vector2Int coord in exploredChunks)
        {
            RevealChunk(coord);
        }

        //if (SaveManager.Instance.currentData.takenCollectables.Contains("Map0"))
        //{
        //    for (int i = 0; i < map0.Length; i++)
        //    {
        //        map0[i].SetActive(true);
        //    }
        //}
    }

    public void RevealChunk(Vector2Int coord)
    {
        Debug.Log("Chunk revelado: " + coord);

        if (minimapChunks.TryGetValue(coord, out Transform chunk))
        {
            chunk.gameObject.SetActive(true);

            Image img = chunk.GetComponent<Image>();
            if (img != null)
                img.color = Color.white;
        }
    }

    public void SaveChunkExplored(Vector2Int chunkCoord)
    {
        if (exploredChunks.Add(chunkCoord))
        {
            RevealChunk(chunkCoord);
        }
    }
}