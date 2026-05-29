using UnityEngine;
using System.Collections;

public class PermanentDestroyer : MonoBehaviour
{
    public string objectID;
    private bool isChecked = false;

    void Awake()
    {
        // 1. МГНОВЕННО скрываем объект, как только он загрузился (в Awake, до появления кадра)
        if (GetComponent<SpriteRenderer>() != null)
            GetComponent<SpriteRenderer>().enabled = false;

        // Отключаем коллайдер, чтобы нельзя было кликнуть, пока идет проверка
        if (GetComponent<Collider2D>() != null)
            GetComponent<Collider2D>().enabled = false;
    }

    void Start()
    {
        StartCoroutine(SafeCheck());
    }

    IEnumerator SafeCheck()
    {
        // Ждем совсем чуть-чуть, пока загрузится инвентарь
        yield return new WaitForSeconds(0.05f);

        bool isInInventory = IsInInventory(objectID);

        if (isInInventory)
        {
            // Если в инвентаре - удаляем окончательно
            Destroy(gameObject);
        }
        else
        {
            // Если в инвентаре нет - показываем предмет обратно
            if (GetComponent<SpriteRenderer>() != null)
                GetComponent<SpriteRenderer>().enabled = true;

            if (GetComponent<Collider2D>() != null)
                GetComponent<Collider2D>().enabled = true;
        }
    }

    private bool IsInInventory(string id)
    {
        if (InventoryPersistence.savedItems == null) return false;
        foreach (var savedItem in InventoryPersistence.savedItems)
        {
            if (savedItem.itemName == id) return true;
        }
        return false;
    }

    public void MarkAsDestroyed()
    {
        Destroy(gameObject);
    }
}