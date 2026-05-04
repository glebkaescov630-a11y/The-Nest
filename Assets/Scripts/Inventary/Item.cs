using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public bool canCombine;
    public Item combineResult;
}

public enum ItemType
{
    Default,
    Consumable,
    Weapon,
    Quest,
    Key
}