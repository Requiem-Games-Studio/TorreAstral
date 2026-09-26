using System.Collections.Generic;
using UnityEngine;

public class SaveObjectsByChunk : MonoBehaviour
{
    public Collider2D col;

    public List<SavedObject> objetos = new List<SavedObject>();


    [ContextMenu("Save Enemies And Items")]
    public List<SavedObject> SaveEnemiesAndItems()
    {
        Debug.Log("Save Enemies And Items");

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

                // Limpiamos el "(Clone)" del nombre si es un objeto instanciado
                objeto.nombre = results[i].name.Replace("(Clone)", "").Trim();
                objeto.posicion = results[i].transform.position;

                objetos.Add(objeto);
            }
        }

        return objetos; // Retornamos la lista capturada
    }
}
