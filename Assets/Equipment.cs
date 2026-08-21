using UnityEngine;

public class Equipment : MonoBehaviour
{

    [Header("Inventory Size")]
    public int rows = 4;
    public int columns = 6;

    [Header("UI Slots")]
    public Slot[] slotsUI;

    private InventorySlot[,] inventory;

    private void Awake()
    {
        inventory = new InventorySlot[rows, columns];

        InitializeInventory();
        UpdateUI();
    }

    private void InitializeInventory()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                inventory[row, column] = new InventorySlot();
            }
        }
    }

    public bool AddItem(Item item)
    {
        if (item == null)
            return false;

        // Primero intenta encontrar un stack existente
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                InventorySlot slot = inventory[row, column];

                if (slot.item == item && slot.quantity < item.maxStack)
                {
                    slot.quantity++;

                    UpdateUI();
                    return true;
                }
            }
        }

        // Si no existe un stack, busca un espacio vacío
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                InventorySlot slot = inventory[row, column];

                if (slot.item == null)
                {
                    slot.item = item;
                    slot.quantity = 1;

                    UpdateUI();
                    return true;
                }
            }
        }

        Debug.Log("Inventario lleno.");
        return false;
    }

    public void RemoveItem(int row, int column)
    {
        if (!IsValidPosition(row, column))
            return;

        InventorySlot slot = inventory[row, column];

        if (slot.item == null)
            return;

        slot.quantity--;

        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
        }

        UpdateUI();
    }

    public InventorySlot GetSlot(int row, int column)
    {
        if (!IsValidPosition(row, column))
            return null;

        return inventory[row, column];
    }

    private bool IsValidPosition(int row, int column)
    {
        return row >= 0 &&
               row < rows &&
               column >= 0 &&
               column < columns;
    }

    private void UpdateUI()
    {
        int index = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                if (index >= slotsUI.Length)
                    return;

                slotsUI[index].UpdateSlot(inventory[row, column]);

                index++;
            }
        }
    }
}
