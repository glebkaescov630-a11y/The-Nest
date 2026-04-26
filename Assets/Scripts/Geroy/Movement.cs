using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;

    private Vector2 targetPosition;
    private bool hasTarget = false;

    void Update()
    {
        // Проверяем нажатие левой кнопки мыши
        if (Input.GetMouseButtonDown(0))
        {
            // Преобразуем экранные координаты мыши в мировые (для 2D)
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Пускаем луч в 2D пространстве
            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Ground"))
            {
                targetPosition = hit.point;
                hasTarget = true;
            }
        }

        // Движение к цели
        if (hasTarget)
        {
            // Перемещаем персонажа
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            // Если дошли до цели - останавливаемся
            if (Vector2.Distance(transform.position, targetPosition) < stoppingDistance)
            {
                hasTarget = false;
            }
        }
    }
}