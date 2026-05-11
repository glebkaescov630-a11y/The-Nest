using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorButton : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "RightRoom";
    [SerializeField] private Transform walkPoint;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stoppingDistance = 0.1f;

    private Transform player;
    private bool isMoving = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                isMoving = true;
            }
        }

        if (isMoving)
        {
            player.position = Vector2.MoveTowards(
                player.position,
                walkPoint.position,
                moveSpeed * Time.deltaTime
            );

            if (Vector2.Distance(player.position, walkPoint.position) <= stoppingDistance)
            {
                isMoving = false;
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}