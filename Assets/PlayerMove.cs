using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMove : MonoBehaviour
{
    [Tooltip("플레이어 이동 속도")]
    public float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameRunning)
        {
            movement = Vector2.zero;
            return;
        }

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameRunning)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 target = movement.sqrMagnitude > 1f ? movement.normalized : movement;
        rb.MovePosition(rb.position + target * moveSpeed * Time.fixedDeltaTime);
    }
}
