using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("UI Элементы")]
    public GameObject inventoryWindow;
    public Button bagButton;
    public Transform slotsParent;
    public GameObject slotPrefab;

    [Header("Настройки")]
    public int inventorySize = 16;

    [Header("Компоненты")]
    public InventoryActions actions;

    private InventorySlot[] slots;
    public System.Action OnInventoryClosed;

    private InventoryPersistence persistence;
    private bool isLoading = false;

    void Start()
    {
        persistence = InventoryPersistence.Instance;

        if (actions == null)
            actions = GetComponent<InventoryActions>();

        slots = new InventorySlot[inventorySize];
        if (slotsParent != null && slotPrefab != null)
        {
            for (int i = 0; i < inventorySize; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, slotsParent);
                slots[i] = slotObj.GetComponent<InventorySlot>();
            }
        }
        else
        {
            Debug.LogError("slotsParent или slotPrefab не назначены в Inventory!");
        }

        if (bagButton != null)
            bagButton.onClick.AddListener(ToggleInventory);

        if (inventoryWindow != null)
            inventoryWindow.SetActive(false);

        LoadInventoryFromSave();
    }

    private void LoadInventoryFromSave()
    {
        if (persistence != null)
        {
            persistence.LoadInventory(this);
        }
    }

    private void SaveInventoryData()
    {
        if (persistence != null && !isLoading)
        {
            persistence.SaveInventory(slots);
        }
    }

    void ToggleInventory()
    {
        if (inventoryWindow != null)
        {
            bool isOpen = !inventoryWindow.activeSelf;
            inventoryWindow.SetActive(isOpen);

            if (actions != null)
            {
                if (isOpen)
                    actions.CancelAllModes();
            }

            if (!isOpen)
            {
                if (actions != null)
                    actions.CancelAllModes();
                OnInventoryClosed?.Invoke();
            }
        }
    }

    public bool AddItem(Item item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("item = null!");
            return false;
        }

        Debug.Log($"Добавляем: {item.itemName}, количество: {amount}");

        if (slots == null)
        {
            Debug.LogError("slots = null! Слоты не созданы!");
            return false;
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogWarning($"Слот {i} = null!");
                continue;
            }

            if (slots[i].currentItem == item && slots[i].itemCount < 99)
            {
                slots[i].itemCount += amount;
                slots[i].SetItem(item, slots[i].itemCount);
                Debug.Log($"Добавлено в слот {i}, теперь {slots[i].itemCount}");
                SaveInventoryData(); // ДОБАВЛЕНО
                return true;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == null)
            {
                Debug.LogWarning($"Слот {i} = null!");
                continue;
            }

            if (slots[i].currentItem == null)
            {
                slots[i].SetItem(item, amount);
                Debug.Log($"Добавлено в пустой слот {i}");
                SaveInventoryData(); 
                return true;
            }
        }

        Debug.Log("Инвентарь полон! Нет свободных слотов");
        return false;
    }

    public bool RemoveItem(Item item, int amount = 1)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem == item)
            {
                slots[i].itemCount -= amount;
                if (slots[i].itemCount <= 0)
                {
                    slots[i].ClearSlot();
                }
                else
                {
                    slots[i].SetItem(item, slots[i].itemCount);
                }
                SaveInventoryData(); 
                return true;
            }
        }
        return false;
    }

    public bool HasItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem == item && slots[i].itemCount > 0)
                return true;
        }
        return false;
    }

    public int GetItemCount(Item item)
    {
        int total = 0;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem == item)
                total += slots[i].itemCount;
        }
        return total;
    }

    public void QuickUseItem(Item item, InventorySlot slot)
    {
        if (item.isConsumable)
        {
            UseItemOnPlayer(item, slot);
        }
        else
        {
            if (actions != null)
                actions.ShowItemDescription(item);
        }
    }

    public void UseItemOnPlayer(Item item, InventorySlot slot)
    {
        Debug.Log($"Использован предмет на игроке: {item.itemName}");

        if (item.isConsumable)
        {
            slot.DecreaseCount();
            SaveInventoryData(); 
        }
    }

    public bool UseItemOnObject(Item item, InventorySlot slot, GameObject target)
    {
        IUsable usable = target.GetComponent<IUsable>();
        if (usable != null)
        {
            bool success = usable.OnUse(item);
            if (success && item.isConsumable)
            {
                slot.DecreaseCount();
                SaveInventoryData(); 
            }
            return success;
        }

        Debug.Log($"Нельзя использовать {item.itemName} на {target.name}");
        return false;
    }

    public void CombineItems(Item itemA, InventorySlot slotA, Item itemB, InventorySlot slotB)
    {
        if (itemA == null || itemB == null) return;

        if (itemA.canCombine && itemA.combineResult != null && itemA.combineResult == itemB)
        {
            PerformCombine(slotA, slotB, itemA.combineResult);
            return;
        }

        if (itemB.canCombine && itemB.combineResult != null && itemB.combineResult == itemA)
        {
            PerformCombine(slotB, slotA, itemB.combineResult);
            return;
        }

        Debug.Log($"Нельзя объединить {itemA.itemName} и {itemB.itemName}");
    }

    private void PerformCombine(InventorySlot slotA, InventorySlot slotB, Item result)
    {
        Debug.Log($"Объединение успешно! Получен {result.itemName}");
        slotA.ClearSlot();
        slotB.ClearSlot();
        AddItem(result, 1);
    }

    public void ResetSlotHighlights()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
                slot.Highlight(false);
        }
    }

    public void ClearInventory()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
                slot.ClearSlot();
        }
        SaveInventoryData();
    }
}