using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IPointerClickHandler
{
    public Image itemIcon;
    public Text countText;

    public Item currentItem;
    public int itemCount;

    private Inventory inventory;

    private void Awake()
    {
        inventory = FindObjectOfType<Inventory>();

        // Находим дочерние объекты по имени
        itemIcon = transform.Find("ItemIcon").GetComponent<Image>();
        countText = transform.Find("ItemCount").GetComponent<Text>();
    }

    public void SetItem(Item item, int count)
    {
        currentItem = item;
        itemCount = count;

        if (item != null)
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
        if (currentItem != null)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                inventory.OnItemUse(currentItem, this);
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                inventory.SelectForCombine(currentItem, this);
            }
        }
    }
}