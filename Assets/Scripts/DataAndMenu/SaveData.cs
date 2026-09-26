using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public string playerName;
    public float playTime;
    public float playProgress;
    public Vector2 playerPosition = new Vector3(-547, -48.4f, 0);
    public int health;
    public List<Vector2Int> exploredChunks = new List<Vector2Int>();

    // NUEVO: IDs de collectables ya tomados
    public List<string> takenCollectables = new List<string>();

    // ESTRUCTURA PARA OBJETOS POR CHUNK:
    public List<ChunkData> chunkDataList = new List<ChunkData>();

    // Obtener la lista de objetos guardados en un chunk
    public List<SavedObject> GetObjectsInChunk(Vector2Int chunkPos)
    {
        ChunkData chunk = chunkDataList.Find(c => c.chunkPosition == chunkPos);
        return chunk != null ? chunk.savedObjects : new List<SavedObject>();
    }

    // Agregar o actualizar los objetos de un chunk
    public void SaveObjectsInChunk(Vector2Int chunkPos, List<SavedObject> objects)
    {
        ChunkData chunk = chunkDataList.Find(c => c.chunkPosition == chunkPos);

        if (chunk == null)
        {
            chunk = new ChunkData { chunkPosition = chunkPos };
            chunkDataList.Add(chunk);
        }

        chunk.savedObjects = objects;
    }
}


[System.Serializable]
public class SavedObject
{
    public string nombre;
    public Vector3 posicion;
}

[System.Serializable]
public class ChunkData
{
    public Vector2Int chunkPosition; // Coordenadas del chunk (ej. x: 0, y: 1)
    public List<SavedObject> savedObjects = new List<SavedObject>();
}
