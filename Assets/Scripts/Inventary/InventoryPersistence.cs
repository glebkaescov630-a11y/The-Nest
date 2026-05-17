using UnityEngine;
using System.Collections.Generic;

public class InventoryPersistence : MonoBehaviour
{
    public static InventoryPersistence Instance;

    // Статический список для хранения предметов между сценами
    public static List<SavedItem> savedItems = new List<SavedItem>();

    // Флаг, который запретит инвентарю перезаписывать сейв во время загрузки
    public bool IsLoading { get; private set; } = false;

    [System.Serializable]
    public class SavedItem
    {
        public string itemName;
        public int amount;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("InventoryPersistence создан! Хранилище готово.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveInventory(InventorySlot[] slots)
    {
        // Если прямо сейчас идет загрузка, игнорируем попытки инвентаря сохраниться поверх
        if (IsLoading) return;

        savedItems.Clear();

        foreach (var slot in slots)
        {
            if (slot != null && slot.currentItem != null && slot.itemCount > 0)
            {
                SavedItem item = new SavedItem();
                item.itemName = slot.currentItem.itemName;
                item.amount = slot.itemCount;
                savedItems.Add(item);
                Debug.Log($"Сохранён предмет: {item.itemName} x{item.amount}");
            }
        }

        Debug.Log($"Всего сохранено: {savedItems.Count} предметов");
    }

    public void LoadInventory(Inventory inventory)
    {
        if (savedItems.Count == 0)
        {
            Debug.Log("Нет сохранённых предметов для загрузки.");
            return;
        }

        // Включаем защиту: сейчас идет загрузка!
        IsLoading = true;
        Debug.Log($"Загружаем {savedItems.Count} предметов...");

        // Создаем временную копию списка, чтобы избежать багов чтения/записи
        List<SavedItem> itemsToLoad = new List<SavedItem>(savedItems);

        foreach (var savedItem in itemsToLoad)
        {
            // Ищем предмет по имени среди всех Item в папке Resources
            Item[] allItems = Resources.LoadAll<Item>("");
            Item foundItem = null;

            foreach (var item in allItems)
            {
                if (item.itemName == savedItem.itemName)
                {
                    foundItem = item;
                    break;
                }
            }

            if (foundItem != null)
            {
                inventory.AddItem(foundItem, savedItem.amount);
                Debug.Log($"Загружен предмет: {foundItem.itemName} x{savedItem.amount}");
            }
            else
            {
                Debug.LogWarning($"Предмет с внутренним Item Name '{savedItem.itemName}' не найден в папке Resources!");
            }
        }

        // Загрузка полностью завершена, снимаем блокировку сохранения
        IsLoading = false;
        Debug.Log("Восстановление инвентаря успешно завершено!");
    }
}


