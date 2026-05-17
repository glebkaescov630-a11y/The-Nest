using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item itemToPickup;
    public int amount = 1;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        Debug.Log("PickupItem готов на объекте: " + gameObject.name);
        if (itemToPickup == null)
        {
            Debug.LogError("ОШИБКА: itemToPickup не назначен на " + gameObject.name);
        }
        else
        {
            Debug.Log("Предмет для подбора: " + itemToPickup.itemName);
        }
    }

    void OnMouseEnter()
    {
        Debug.Log("Мышь наведена на: " + gameObject.name);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.yellow;
        }
    }

    void OnMouseExit()
    {
        Debug.Log("Мышь ушла с: " + gameObject.name);
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    void OnMouseDown()
    {
        Debug.Log("=== КЛИК по предмету: " + gameObject.name + " ===");

        if (itemToPickup == null)
        {
            Debug.LogError("НЕЛЬЗЯ ПОДОБРАТЬ: itemToPickup = NULL!");
            return;
        }

        Debug.Log("Предмет для подбора: " + itemToPickup.itemName);

        Inventory inventory = FindObjectOfType<Inventory>();

        if (inventory == null)
        {
            Debug.LogError("Inventory НЕ НАЙДЕН на сцене!");
            Debug.Log("Проверьте, что на InventoryCanvas есть компонент Inventory");
            return;
        }

        Debug.Log("Inventory найден на объекте: " + inventory.gameObject.name);

        bool success = inventory.AddItem(itemToPickup, amount);
        Debug.Log("AddItem вернул: " + success);

        if (success)
        {
            Debug.Log("УСПЕХ! Предмет добавлен, удаляем объект со сцены");
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("НЕ УСПЕХ! Инвентарь полон или другая проблема");
        }
    }
}