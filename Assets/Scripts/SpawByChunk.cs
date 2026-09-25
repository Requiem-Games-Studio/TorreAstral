using Fusion;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SpawByChunk : MonoBehaviour
{
    public Collider2D col;

    public List<SavedObject> objetos = new List<SavedObject>();


    [ContextMenu("Save Enemies And Items")]
    public void SaveEnemiesAndItems()
    {
        Debug.Log("Save Enemies And Items");

        // Limpiamos los datos anteriores
        objetos.Clear();

        Collider2D[] results = new Collider2D[50];

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        int count = col.Overlap(filter, results);

        for (int i = 0; i < count; i++)
        {
            if (results[i] != null && results[i].CompareTag("Object"))
            {
                SavedObject objeto = new SavedObject();

                objeto.nombre = results[i].name;
                objeto.posicion = results[i].transform.position;

                objetos.Add(objeto);

                Debug.Log(
                    "Guardado: " +
                    objeto.nombre +
                    " | Posición: " +
                    objeto.posicion
                );
            }
        }

        Debug.Log("Objetos guardados: " + objetos.Count);
    }
}

[System.Serializable]
public class SavedObject
{
    public string nombre;
    public Vector3 posicion;
}