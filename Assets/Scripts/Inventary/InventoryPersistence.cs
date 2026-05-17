using UnityEngine;
using System.Collections.Generic;

public class InventoryPersistence : MonoBehaviour
{
    public static InventoryPersistence Instance;

    // Статический список для хранения предметов (сохраняется даже между сценами)
    public static List<SavedItem> savedItems = new List<SavedItem>();

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
            Debug.Log("Нет сохранённых предметов");
            return;
        }

        Debug.Log($"Загружаем {savedItems.Count} предметов...");

        foreach (var savedItem in savedItems)
        {
            // Ищем предмет по имени среди всех Item
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
                Debug.LogWarning($"Предмет не найден: {savedItem.itemName}");
            }
        }
    }
}