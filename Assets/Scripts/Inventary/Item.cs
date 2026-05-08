using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Основная информация")]
    public string itemName;        
    public Sprite icon;            
    public ItemType itemType;      

    [Header("Описание")]
    [TextArea(2, 5)]
    public string description;    

    [Header("Объединение")]
    public bool canCombine;    
    public Item combineResult;

    [Header("Использование")]
    public bool isConsumable;
    public string targetTag; 
}

public enum ItemType
{
    Default,
    Consumable,
    Weapon,
    Quest,
    Key
}