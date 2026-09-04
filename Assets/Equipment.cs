using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class Equipment : MonoBehaviour
{

    [Header("Inventory Size")]
    public int rows = 4;
    public int columns = 6;

    [Header("UI Slots")]
    public Slot[] slotsUI;
    public Slot weaponSlot,helmentSlot,armorSlot,glovesSlot,bootsSlot;

    private InventorySlot weaponInv, helmentInv, armorInv, glovesInv, bootsInv;

    private InventorySlot[,] inventory;

    public Transform pivotWeapon;
    public WeaponManager weaponManager;


    private void Awake()
    {
        weaponInv = new InventorySlot();
        helmentInv = new InventorySlot();
        armorInv = new InventorySlot();
        glovesInv = new InventorySlot();
        bootsInv = new InventorySlot();
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

        if(item.itemType == ItemType.Weapon && weaponInv.item == null)
        {
            weaponInv.item = item;
            weaponInv.quantity = 1;
            GameObject newWeapon = Instantiate(item.itemPrefab,pivotWeapon.position,pivotWeapon.rotation,pivotWeapon);
            weaponManager.SetNewWeapon(newWeapon);
            UpdateUI();
            return true;
        }
        if (item.itemType == ItemType.Helmet && helmentInv.item == null)
        {
            helmentInv.item = item;
            helmentInv.quantity = 1;
            UpdateUI();
            return true;
        }
        if (item.itemType == ItemType.Armor && armorInv.item == null)
        {
            armorInv.item = item;
            armorInv.quantity = 1;
            UpdateUI();
            return true;
        }
        if (item.itemType == ItemType.Gloves && glovesInv.item == null)
        {
            glovesInv.item = item;
            glovesInv.quantity = 1;
            UpdateUI();
            return true;
        }
        if (item.itemType == ItemType.Boots && bootsInv.item == null)
        {
            bootsInv.item = item;
            bootsInv.quantity = 1;
            UpdateUI();
            return true;
        }

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
        weaponSlot.UpdateSlot(weaponInv);
        helmentSlot.UpdateSlot(helmentInv);
        armorSlot.UpdateSlot(armorInv);
        glovesSlot.UpdateSlot(glovesInv);
        bootsSlot.UpdateSlot(bootsInv);

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
