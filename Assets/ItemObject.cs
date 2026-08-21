using UnityEngine;

public class ItemObject : MonoBehaviour
{

    public Item item;

    public Equipment equipment;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (equipment.AddItem(item))
            {
                Destroy(gameObject);
            }
        }
    }

}
