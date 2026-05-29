using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public Item itemToPickup;
    public int amount = 1;

    void OnMouseDown()
    {
        Debug.Log("=== КЛИК по предмету: " + gameObject.name + " ===");

        if (itemToPickup == null)
        {
            Debug.LogError("ОШИБКА: itemToPickup не назначен на " + gameObject.name);
            return;
        }

        Inventory inventory = FindFirstObjectByType<Inventory>();
        if (inventory == null) return;

        // 1. Пытаемся добавить предмет в инвентарь
        bool success = inventory.AddItem(itemToPickup, amount);

        if (success)
        {
            Debug.Log("Предмет успешно подобран!");

            // 2. Ищем скрипт PermanentDestroyer на этом же объекте
            PermanentDestroyer destroyer = GetComponent<PermanentDestroyer>();

            if (destroyer != null)
            {
                // Если он есть, помечаем его как удаленный навсегда
                destroyer.MarkAsDestroyed();
            }
            else
            {
                // Если его нет, просто удаляем объект
                Destroy(gameObject);
            }
        }
    }
}