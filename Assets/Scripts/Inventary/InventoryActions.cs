using UnityEngine;
using UnityEngine.UI;

public class InventoryActions : MonoBehaviour
{
    [Header("Кнопки действий")]
    public Button useButton;       // Кнопка "Использовать"
    public Button combineButton;   // Кнопка "Объединить"
    public Button examineButton;   // Кнопка "Изучить"

    [Header("UI для описания")]
    public GameObject descriptionPanel;   // Панель с описанием
    public Text descriptionText;          // Текст описания

    [Header("Состояния")]
    public Item selectedItem;             // Выбранный предмет
    public bool isCombineMode = false;    // Режим объединения

    private Inventory inventory;
    private InventorySlot selectedSlot;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        if (useButton != null)
            useButton.onClick.AddListener(OnUseClick);

        if (combineButton != null)
            combineButton.onClick.AddListener(OnCombineClick);

        if (examineButton != null)
            examineButton.onClick.AddListener(OnExamineClick);

        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }

    public void SelectItem(Item item, InventorySlot slot)
    {
        if (isCombineMode && selectedItem != null && selectedSlot != null)
        {
            TryCombine(selectedItem, selectedSlot, item, slot);
            CancelCombineMode();
        }
        else if (isCombineMode && selectedItem == null)
        {
            CancelCombineMode();
        }
        else
        {
            SetSelectedItem(item, slot);
        }
    }

    private void SetSelectedItem(Item item, InventorySlot slot)
    {
        if (selectedSlot != null)
            selectedSlot.Highlight(false);

        selectedItem = item;
        selectedSlot = slot;

        if (selectedSlot != null)
            selectedSlot.Highlight(true);

        Debug.Log($"Выбран предмет: {item.itemName}");
    }

    private void TryCombine(Item itemA, InventorySlot slotA, Item itemB, InventorySlot slotB)
    {
        if (inventory != null)
            inventory.CombineItems(itemA, slotA, itemB, slotB);
        else
            Debug.LogError("Inventory не найден!");
    }

    void OnUseClick()
    {
        if (selectedItem == null)
        {
            Debug.Log("Сначала выберите предмет!");
            return;
        }

        Debug.Log($"Режим использования: выберите цель для {selectedItem.itemName}");

        if (isCombineMode)
            CancelCombineMode();

        UseModeManager.Instance?.EnterUseMode(selectedItem, selectedSlot);
    }

    void OnCombineClick()
    {
        if (selectedItem == null)
        {
            Debug.Log("Сначала выберите предмет для объединения!");
            return;
        }

        if (!selectedItem.canCombine)
        {
            Debug.Log($"Предмет {selectedItem.itemName} нельзя объединить!");
            return;
        }

        isCombineMode = true;
        Debug.Log($"Режим объединения: выберите второй предмет для {selectedItem.itemName}");
    }

    void OnExamineClick()
    {
        if (selectedItem == null)
        {
            Debug.Log("Нечего изучать!");
            return;
        }

        ShowItemDescription(selectedItem);
    }

    public void ShowItemDescription(Item item)
    {
        if (descriptionPanel != null && descriptionText != null)
        {
            descriptionText.text = item.description;
            descriptionPanel.SetActive(true);

            Invoke(nameof(HideDescription), 3f);
        }
        else
        {
            Debug.Log($"Описание предмета {item.itemName}: {item.description}");
        }
    }

    void HideDescription()
    {
        if (descriptionPanel != null)
            descriptionPanel.SetActive(false);
    }

    public void CancelCombineMode()
    {
        isCombineMode = false;
        Debug.Log("Режим объединения отменён");
    }

    public void CancelAllModes()
    {
        CancelCombineMode();

        if (selectedSlot != null)
        {
            selectedSlot.Highlight(false);
            selectedSlot = null;
        }
        selectedItem = null;
    }

    public void ClearSelectedItem()
    {
        if (selectedSlot != null)
            selectedSlot.Highlight(false);

        selectedItem = null;
        selectedSlot = null;
    }
}