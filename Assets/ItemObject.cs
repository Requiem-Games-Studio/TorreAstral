using UnityEngine;

public class ItemObject : MonoBehaviour
{
    //Scriptable object del item
    public Item item;

    //Script del jugador que gaurda el item
    //[HideInInspector]
    public Equipment equipment;

    public bool consumable;

    public void AddItemToPlayer()
    {
        if (equipment != null && equipment.AddItem(item))
        {
            Destroy(gameObject);
        }
    }

}
