using System.Collections;
using Unity.VisualScripting;
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
    public ArmorManager armorManager;



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

        Debug.Log("Awake Inventory");
        this.gameObject.SetActive(false);
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
            GameObject newWeapon = Instantiate(item.playerObject,pivotWeapon.position,pivotWeapon.rotation,pivotWeapon);
            weaponManager.SetNewWeapon(newWeapon);
            UpdateUI();
            return true;
        }
        if (item.itemType == ItemType.Helmet && helmentInv.item == null)
        {
            helmentInv.item = item;
            helmentInv.quantity = 1;
            GameObject newHelment = Instantiate(item.playerObject);
            armorManager.SetNewHelment(newHelment);
            UpdateUI();
            return true;
        }
        if (item.itemType == ItemType.Armor && armorInv.item == null)
        {
            armorInv.item = item;
            armorInv.quantity = 1;
            GameObject newArmor = Instantiate(item.playerObject);
            armorManager.SetNewChest(newArmor);
            UpdateUI();
            return true;
        }
        if (item.itemType == ItemType.Gloves && glovesInv.item == null)
        {
            glovesInv.item = item;
            glovesInv.quantity = 1;
            GameObject newArmor = Instantiate(item.playerObject);
            armorManager.SetNewArms(newArmor);
            UpdateUI();
            return true;
        }
        if (item.itemType == ItemType.Boots && bootsInv.item == null)
        {
            Debug.Log("Botas añadidas");
            bootsInv.item = item;
            bootsInv.quantity = 1;
            GameObject newBoots = Instantiate(item.playerObject);
            armorManager.SetNewBoots(newBoots);
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


    /// REMOVE ITEMS //
    public void RemoveItem(int row, int column)
    {
        if (!IsValidPosition(row, column))
            return;

        InventorySlot slot = inventory[row, column];

        if (slot.item == null)
            return;
        //Instantiar objeto en la posicion del jugador
        if(slot.item.worldObject != null)
        {
            Debug.Log("Soltar Objeto");
            Instantiate(slot.item.worldObject, pivotWeapon.transform.position, Quaternion.identity);
        }

        slot.quantity--;

        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
        }

        UpdateUI();
    }
    public void OnClickRemove(string coord)
    {
        // 1. Separar el string usando el carácter delimitador (coma, guion, espacio, etc.)
        string[] partes = coord.Split(',');

        // 2. Validar que la cadena tenga exactamente 2 elementos
        if (partes.Length == 2)
        {
            // 3. Convertir cada texto a número int de forma segura
            if (int.TryParse(partes[0], out int row) && int.TryParse(partes[1], out int column))
            {
                RemoveItem(row, column);
                return;
            }
        }
    }

    public void RemoveWeapon()
    {
        InventorySlot slot = weaponInv;

        if (slot.item == null)
            return;
        //Instantiar objeto en la posicion del jugador
        if (slot.item.worldObject != null)
        {
            Debug.Log("Soltar Objeto");
            Instantiate(slot.item.worldObject,pivotWeapon.transform.position,Quaternion.identity);
        }

        weaponManager.UnequipWeapon();

        slot.quantity--;

        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
        }

        UpdateUI();
    }
    public void RemoveHelment()
    {
        InventorySlot slot = helmentInv;

        if (slot.item == null)
            return;

        armorManager.UnequipHelment();

        //Instantiar objeto en la posicion del jugador
        if (slot.item.worldObject != null)
        {
            Instantiate(slot.item.worldObject, pivotWeapon.transform.position, Quaternion.identity);
        }
        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
        }
        UpdateUI();
    }
    public void RemoveChest()
    {
        InventorySlot slot = armorInv;

        if (slot.item == null)
            return;

        armorManager.UnequipChest();

        //Instantiar objeto en la posicion del jugador
        if (slot.item.worldObject != null)
        {
            Instantiate(slot.item.worldObject, pivotWeapon.transform.position, Quaternion.identity);
        }
        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
        }
        UpdateUI();
    }
    public void RemoveArms()
    {
        InventorySlot slot = glovesInv;

        if (slot.item == null)
            return;

        armorManager.UnequipArms();

        //Instantiar objeto en la posicion del jugador
        if (slot.item.worldObject != null)
        {
            Instantiate(slot.item.worldObject, pivotWeapon.transform.position, Quaternion.identity);
        }
        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
        }
        UpdateUI();
    }
    public void RemoveBoots()
    {
        InventorySlot slot = bootsInv;
        
        if (slot.item == null)
            return;

        armorManager.UnequipBoots();

        //Instantiar objeto en la posicion del jugador
        if (slot.item.worldObject != null)
        {
            Instantiate(slot.item.worldObject, pivotWeapon.transform.position, Quaternion.identity);
        }
        slot.quantity--;
        if (slot.quantity <= 0)
        {
            slot.item = null;
            slot.quantity = 0;
        }
        UpdateUI();
    }

    /// REMOVE ITEMS //

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
