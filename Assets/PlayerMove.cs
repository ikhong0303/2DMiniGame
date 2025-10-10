using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // �̵� �ӵ��� Inspector â���� ������ �� �ֵ��� public���� �����մϴ�.
    [Tooltip("ĳ������ �̵� �ӵ��� �����մϴ�.")]
    public float moveSpeed = 5f;

    // Rigidbody 2D ������Ʈ�� ���� ����
    private Rigidbody2D rb;

    // �̵� ������ ������ ����
    private Vector2 movement;

    // ������ ���۵� �� �ѹ��� ȣ��Ǵ� Awake �Լ�
    void Awake()
    {
        // ��ũ��Ʈ�� �پ��ִ� ���� ������Ʈ���� Rigidbody 2D ������Ʈ�� ã�� rb ������ �Ҵ��մϴ�.
        // GetComponent<T>()�� ����� ���� �� �� �����Ƿ�, Update���ٴ� Awake�� Start���� �ѹ��� ȣ���ϴ� ���� �����ϴ�.
        rb = GetComponent<Rigidbody2D>();
    }

    // �� �����Ӹ��� ȣ��Ǵ� Update �Լ�
    void Update()
    {
        // Ű���� �Է��� �޾� �̵� ������ �����մϴ�.
        // Input.GetAxisRaw�� -1, 0, 1 �� ���� ���� ��ȯ�Ͽ� �ﰢ���� ������ �����ݴϴ�. (GetAixs�� ����/���� ȿ���� ����)
        movement.x = Input.GetAxisRaw("Horizontal"); // "Horizontal"�� �⺻������ A, D Ű�� �¿� ȭ��ǥ Ű�� ���εǾ� �ֽ��ϴ�.
        movement.y = Input.GetAxisRaw("Vertical");   // "Vertical"�� �⺻������ W, S Ű�� ���Ʒ� ȭ��ǥ Ű�� ���εǾ� �ֽ��ϴ�.
    }

    // ������ �ð� �������� ���� ���� �Բ� ȣ��Ǵ� FixedUpdate �Լ�
    void FixedUpdate()
    {
        // Rigidbody�� ��ġ�� �̵���ŵ�ϴ�.
        // rb.position: ���� ��ġ
        // movement.normalized: �̵� ���� ������ ũ�⸦ 1�� ����� �밢�� �̵� �� �ӵ��� �������� ���� �����մϴ�.
        // moveSpeed: ������ �̵� �ӵ�
        // Time.fixedDeltaTime: FixedUpdate�� ȣ�� ���� �ð�. ������ �ӵ��� ������� ������ �ӵ��� �����̰� �մϴ�.
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.deltaTime);
    }
}
