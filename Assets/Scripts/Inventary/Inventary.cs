using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [Header("UI Элементы")]
    public GameObject inventoryWindow;  // Окно инвентаря
    public Button bagButton;            // Кнопка-сумка
    public Transform slotsParent;       // Родительский объект для слотов (ItemGrid)
    public GameObject slotPrefab;       // Префаб слота

    [Header("Настройки")]
    public int inventorySize = 16;      // Количество слотов (4x4 = 16)

    [Header("Компоненты")]
    public InventoryActions actions;    // Ссылка на скрипт действий

    private InventorySlot[] slots;

    // Событие для оповещения о закрытии инвентаря
    public System.Action OnInventoryClosed;

    void Start()
    {
        // Находим InventoryActions, если не назначен
        if (actions == null)
            actions = GetComponent<InventoryActions>();

        // Создаём ячейки
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

        // Назначаем кнопку сумки
        if (bagButton != null)
            bagButton.onClick.AddListener(ToggleInventory);

        // Инвентарь скрыт по умолчанию
        if (inventoryWindow != null)
            inventoryWindow.SetActive(false);

        // Добавляем тестовые предметы (закомментировано, раскомментируйте для теста)
        // Invoke(nameof(AddTestItems), 0.5f);
    }

    void ToggleInventory()
    {
        if (inventoryWindow != null)
        {
            bool isOpen = !inventoryWindow.activeSelf;
            inventoryWindow.SetActive(isOpen);

            if (!isOpen)
            {
                // Выходим из режимов при закрытии
                if (actions != null)
                    actions.CancelAllModes();
                OnInventoryClosed?.Invoke();
            }
        }
    }

    // Добавление тестовых предметов (для отладки)
    void AddTestItems()
    {
        // Пример: создайте предметы через Assets/Create/Inventory/Item
        // Item testItem = Resources.Load<Item>("Items/TestItem");
        // if (testItem != null) AddItem(testItem, 3);
    }

    // Добавление предмета в инвентарь
    public bool AddItem(Item item, int amount = 1)
    {
        if (item == null)
        {
            Debug.LogWarning("Попытка добавить null предмет!");
            return false;
        }

        // Сначала ищем стак такого же предмета
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem == item && slots[i].itemCount < 99)
            {
                slots[i].itemCount += amount;
                slots[i].SetItem(item, slots[i].itemCount);
                return true;
            }
        }

        // Ищем пустой слот
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem == null)
            {
                slots[i].SetItem(item, amount);
                return true;
            }
        }

        Debug.Log($"Инвентарь полон! Не удалось добавить {item.itemName}");
        return false;
    }

    // Удалить предмет
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
                return true;
            }
        }
        return false;
    }

    // Проверить, есть ли предмет в инвентаре
    public bool HasItem(Item item)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem == item && slots[i].itemCount > 0)
                return true;
        }
        return false;
    }

    // Получить количество предметов
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

    // Быстрое использование (ПКМ)
    public void QuickUseItem(Item item, InventorySlot slot)
    {
        if (item.isConsumable)
        {
            UseItemOnPlayer(item, slot);
        }
        else
        {
            // Если не расходуемый - показываем описание
            if (actions != null)
                actions.ShowItemDescription(item);
        }
    }

    // Использовать предмет на игроке (лечение, бафф)
    public void UseItemOnPlayer(Item item, InventorySlot slot)
    {
        Debug.Log($"Использован предмет на игроке: {item.itemName}");

        // Здесь добавьте логику лечения/баффа
        // Например: PlayerHealth.Heal(20);

        if (item.isConsumable)
        {
            slot.DecreaseCount();
        }
    }

    // Использовать предмет на объекте в мире
    public bool UseItemOnObject(Item item, InventorySlot slot, GameObject target)
    {
        IUsable usable = target.GetComponent<IUsable>();
        if (usable != null)
        {
            bool success = usable.OnUse(item);
            if (success && item.isConsumable)
            {
                slot.DecreaseCount();
            }
            return success;
        }

        Debug.Log($"Нельзя использовать {item.itemName} на {target.name}");
        return false;
    }

    // Объединить предметы
    public void CombineItems(Item itemA, InventorySlot slotA, Item itemB, InventorySlot slotB)
    {
        if (itemA == null || itemB == null) return;

        // Проверяем объединение A + B
        if (itemA.canCombine && itemA.combineResult != null && itemA.combineResult == itemB)
        {
            PerformCombine(slotA, slotB, itemA.combineResult);
            return;
        }

        // Проверяем объединение B + A
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

    // Сбросить подсветку всех слотов
    public void ResetSlotHighlights()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
                slot.Highlight(false);
        }
    }

    // Получить все предметы (для сохранения)
    public InventorySaveData GetSaveData()
    {
        InventorySaveData data = new InventorySaveData();
        data.items = new ItemSaveData[inventorySize];

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null && slots[i].currentItem != null)
            {
                data.items[i] = new ItemSaveData
                {
                    itemName = slots[i].currentItem.name,
                    count = slots[i].itemCount
                };
            }
        }
        return data;
    }

    // Загрузить инвентарь (для сохранения)
    public void LoadSaveData(InventorySaveData data)
    {
        // Очищаем инвентарь
        foreach (var slot in slots)
        {
            if (slot != null)
                slot.ClearSlot();
        }

        // Загружаем предметы
        for (int i = 0; i < data.items.Length && i < slots.Length; i++)
        {
            if (data.items[i] != null && !string.IsNullOrEmpty(data.items[i].itemName))
            {
                Item item = Resources.Load<Item>($"Items/{data.items[i].itemName}");
                if (item != null)
                {
                    slots[i].SetItem(item, data.items[i].count);
                }
            }
        }
    }
}

// Классы для сохранения
[System.Serializable]
public class InventorySaveData
{
    public ItemSaveData[] items;
}

[System.Serializable]
public class ItemSaveData
{
    public string itemName;
    public int count;
}