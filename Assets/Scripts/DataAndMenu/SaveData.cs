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
}
