using UnityEngine;

public class ClickToMove : MonoBehaviour
{
    public static bool canMove = true;

    [SerializeField] private float speed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;

    private Vector2 targetPosition;
    private bool hasTarget = false;

    void Update()
    {
        if (!canMove) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject.CompareTag("Ground"))
            {
                targetPosition = hit.point;
                hasTarget = true;
            }
        }

        if (hasTarget)
        {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, targetPosition) < stoppingDistance)
            {
                hasTarget = false;
            }
        }
    }
}