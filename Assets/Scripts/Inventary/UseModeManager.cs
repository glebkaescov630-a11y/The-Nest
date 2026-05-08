using UnityEngine;

public class UseModeManager : MonoBehaviour
{
    public static UseModeManager Instance;

    public enum GameMode { Normal, UseMode }
    public GameMode currentMode = GameMode.Normal;

    private Item selectedItem;
    private InventorySlot selectedSlot;
    private Inventory inventory;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        inventory = FindFirstObjectByType<Inventory>();
    }

    public void EnterUseMode(Item item, InventorySlot slot)
    {
        selectedItem = item;
        selectedSlot = slot;
        currentMode = GameMode.UseMode;

        // Меняем курсор (опционально)
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        Debug.Log($"Кликните на объект, чтобы использовать {selectedItem.itemName}");
    }

    public void UseOnObject(GameObject target)
    {
        if (currentMode != GameMode.UseMode)
            return;

        if (target == null)
        {
            Debug.Log("Цель не найдена!");
            ExitUseMode();
            return;
        }

        // Проверяем, можно ли использовать предмет на цели
        IUsable usable = target.GetComponent<IUsable>();
        if (usable != null)
        {
            bool success = usable.OnUse(selectedItem);

            if (success && selectedItem.isConsumable && inventory != null && selectedSlot != null)
            {
                selectedSlot.DecreaseCount();
            }

            if (success)
            {
                Debug.Log($"Использован {selectedItem.itemName} на {target.name}");
            }
        }
        else
        {
            Debug.Log($"Нельзя использовать {selectedItem.itemName} на {target.name}");
        }

        ExitUseMode();
    }

    void ExitUseMode()
    {
        currentMode = GameMode.Normal;
        selectedItem = null;
        selectedSlot = null;

        // Возвращаем обычный курсор
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    void Update()
    {
        // Клик по объекту в режиме использования
        if (currentMode == GameMode.UseMode && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit2D.collider != null)
            {
                UseOnObject(hit2D.collider.gameObject);
            }
            else
            {
                // Клик в пустоту - отмена
                Debug.Log("Режим использования отменён");
                ExitUseMode();
            }
        }
    }
}