using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorButton : MonoBehaviour
{
    [Header("Настройки двери")]
    [SerializeField] private string nextSceneName = "LeftRoom";
    [SerializeField] private float interactionRange = 2f;

    [Header("Настройки движения")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float stoppingDistance = 1f;

    private Transform player;
    private bool isMovingToButton = false;
    private Camera mainCamera;

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        else Debug.LogError("Player not found!");

        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (player == null || mainCamera == null) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                TryInteract();
            }
        }

        if (isMovingToButton)
        {
            Vector2 direction = (transform.position - player.position).normalized;
            player.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, player.position) <= stoppingDistance)
            {
                isMovingToButton = false;
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private void TryInteract()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= interactionRange)
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            isMovingToButton = true;
        }
    }
}