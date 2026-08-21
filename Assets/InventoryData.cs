using UnityEngine;
using System;

public enum ItemType
{
    Weapon,
    Helmet,
    Armor,
    Boots,
    Gloves,
    Consumable,
    Material,
    Object,
    Null
}

[System.Serializable]
public class InventorySlot
{
    public ItemType allowedType;
    public Item item;
    public int quantity;
}
