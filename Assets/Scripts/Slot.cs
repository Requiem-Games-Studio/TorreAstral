using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text quantityText;
    public ItemType allowedType;

    public void UpdateSlot(InventorySlot inventorySlot)
    {
        if (inventorySlot == null || inventorySlot.item == null)
        {
            itemIcon.enabled = false;
            quantityText.text = "";
            return;
        }

        itemIcon.enabled = true;
        itemIcon.sprite = inventorySlot.item.icon;

        if (inventorySlot.quantity > 1)
        {
            quantityText.text = inventorySlot.quantity.ToString();
        }
        else
        {
            quantityText.text = "";
        }
    }
}
