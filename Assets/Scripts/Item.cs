using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    
    public int id;

    public string itemName;
    public Sprite icon;
    public int maxStack;
    public ItemType itemType;


}
