using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Компоненты")]
    public Image itemIcon;      
    public Text countText;      
                                

    [Header("Данные слота")]
    public Item currentItem;    
    public int itemCount;       

    private Inventory inventory;
    private InventoryActions actions;
    private Image slotBackground; 

    private void Awake()
    {
        inventory = FindFirstObjectByType<Inventory>();
        actions = FindFirstObjectByType<InventoryActions>();
        slotBackground = GetComponent<Image>();

        Transform iconTransform = transform.Find("ItemIcon");
        Transform countTransform = transform.Find("ItemCount");

        if (iconTransform != null)
            itemIcon = iconTransform.GetComponent<Image>();

        if (countTransform != null)
            countText = countTransform.GetComponent<Text>();
    }

    public void SetItem(Item item, int count)
    {
        currentItem = item;
        itemCount = count;

        if (item != null && item.icon != null)
        {
            itemIcon.sprite = item.icon;
            itemIcon.enabled = true;
            countText.text = count > 1 ? count.ToString() : "";
            countText.enabled = count > 1;
        }
        else
        {
            ClearSlot();
        }
    }

    public void ClearSlot()
    {
        currentItem = null;
        itemCount = 0;
        itemIcon.sprite = null;
        itemIcon.enabled = false;
        countText.text = "";
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (currentItem == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (actions != null)
                actions.SelectItem(currentItem, this);
            else
                Debug.LogWarning("InventoryActions не найден!");
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (inventory != null)
                inventory.QuickUseItem(currentItem, this);
        }
    }

    public void Highlight(bool highlight)
    {
        if (slotBackground != null)
        {
            slotBackground.color = highlight ? new Color(1f, 0.9f, 0.3f, 1f) : new Color(0.8f, 0.8f, 0.8f, 1f);
        }
    }

    public void DecreaseCount(int amount = 1)
    {
        itemCount -= amount;
        if (itemCount <= 0)
        {
            ClearSlot();
        }
        else
        {
            SetItem(currentItem, itemCount);
        }
    }
}