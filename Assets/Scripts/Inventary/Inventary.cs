using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject inventoryWindow;
    public Button bagButton;
    public Transform slotsParent;
    public GameObject slotPrefab;

    [Header("Settings")]
    public int inventorySize = 16;

    private InventorySlot[] slots;
    private Item selectedForCombine;
    private InventorySlot selectedSlot;
    private bool isCombineMode = false;

    void Start()
    {
        // Создаём ячейки
        slots = new InventorySlot[inventorySize];
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slotObj = Instantiate(slotPrefab, slotsParent);
            slots[i] = slotObj.GetComponent<InventorySlot>();
        }

        // Назначаем кнопку сумки
        bagButton.onClick.AddListener(ToggleInventory);

        // Инвентарь скрыт по умолчанию
        inventoryWindow.SetActive(false);

        // Добавляем тестовые предметы
        AddTestItems();
    }

    void ToggleInventory()
    {
        inventoryWindow.SetActive(!inventoryWindow.activeSelf);
        if (!inventoryWindow.activeSelf)
        {
            CancelCombine();
        }
    }

    void AddTestItems()
    {
        // Временно закомментировано, добавим позже
        // AddItem(yourItem, 1);
    }

    public bool AddItem(Item item, int amount = 1)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == item && slots[i].itemCount < 99)
            {
                slots[i].itemCount += amount;
                slots[i].SetItem(item, slots[i].itemCount);
                return true;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].currentItem == null)
            {
                slots[i].SetItem(item, amount);
                return true;
            }
        }

        Debug.Log("Инвентарь полон!");
        return false;
    }

    public void OnItemUse(Item item, InventorySlot slot)
    {
        if (isCombineMode && selectedForCombine != null)
        {
            CombineItems(selectedForCombine, selectedSlot, item, slot);
            CancelCombine();
        }
        else
        {
            UseItem(item, slot);
        }
    }

    void UseItem(Item item, InventorySlot slot)
    {
        Debug.Log("Использован предмет: " + item.itemName);

        switch (item.itemType)
        {
            case ItemType.Consumable:
                slot.itemCount--;
                if (slot.itemCount <= 0)
                {
                    slot.ClearSlot();
                }
                else
                {
                    slot.SetItem(item, slot.itemCount);
                }
                break;

            case ItemType.Key:
                Debug.Log("Это ключ! Открывает двери...");
                break;

            default:
                Debug.Log("Использовать нельзя или нет эффекта");
                break;
        }
    }

    void CombineItems(Item itemA, InventorySlot slotA, Item itemB, InventorySlot slotB)
    {
        if (itemA.canCombine && itemB.canCombine && itemA.combineResult == itemB)
        {
            Debug.Log($"Объединяем {itemA.itemName} и {itemB.itemName}");

            slotA.ClearSlot();
            slotB.ClearSlot();
            AddItem(itemA.combineResult, 1);
        }
        else
        {
            Debug.Log("Эти предметы нельзя объединить!");
        }
    }

    public void SelectForCombine(Item item, InventorySlot slot)
    {
        if (!isCombineMode)
        {
            isCombineMode = true;
            selectedForCombine = item;
            selectedSlot = slot;
            Debug.Log($"Выбран предмет для объединения: {item.itemName}. Теперь кликните по другому предмету.");
            HighlightSlot(slot, true);
        }
    }

    void CancelCombine()
    {
        if (selectedSlot != null)
        {
            HighlightSlot(selectedSlot, false);
        }
        isCombineMode = false;
        selectedForCombine = null;
        selectedSlot = null;
    }

    void HighlightSlot(InventorySlot slot, bool highlight)
    {
        Image slotImage = slot.GetComponent<Image>();
        slotImage.color = highlight ? Color.yellow : new Color(0.8f, 0.8f, 0.8f);
    }
}