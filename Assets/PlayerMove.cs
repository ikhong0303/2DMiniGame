using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 이동 속도를 Inspector 창에서 조절할 수 있도록 public으로 선언합니다.
    [Tooltip("캐릭터의 이동 속도를 설정합니다.")]
    public float moveSpeed = 5f;

    // Rigidbody 2D 컴포넌트를 담을 변수
    private Rigidbody2D rb;

    // 이동 방향을 저장할 변수
    private Vector2 movement;

    // 게임이 시작될 때 한번만 호출되는 Awake 함수
    void Awake()
    {
        // 스크립트가 붙어있는 게임 오브젝트에서 Rigidbody 2D 컴포넌트를 찾아 rb 변수에 할당합니다.
        // GetComponent<T>()는 비용이 많이 들 수 있으므로, Update보다는 Awake나 Start에서 한번만 호출하는 것이 좋습니다.
        rb = GetComponent<Rigidbody2D>();
    }

    // 매 프레임마다 호출되는 Update 함수
    void Update()
    {
        // 키보드 입력을 받아 이동 방향을 설정합니다.
        // Input.GetAxisRaw는 -1, 0, 1 세 가지 값만 반환하여 즉각적인 반응을 보여줍니다. (GetAixs는 가속/감속 효과가 있음)
        movement.x = Input.GetAxisRaw("Horizontal"); // "Horizontal"은 기본적으로 A, D 키와 좌우 화살표 키에 매핑되어 있습니다.
        movement.y = Input.GetAxisRaw("Vertical");   // "Vertical"은 기본적으로 W, S 키와 위아래 화살표 키에 매핑되어 있습니다.
    }

    // 고정된 시간 간격으로 물리 계산과 함께 호출되는 FixedUpdate 함수
    void FixedUpdate()
    {
        // Rigidbody의 위치를 이동시킵니다.
        // rb.position: 현재 위치
        // movement.normalized: 이동 방향 벡터의 크기를 1로 만들어 대각선 이동 시 속도가 빨라지는 것을 방지합니다.
        // moveSpeed: 설정한 이동 속도
        // Time.fixedDeltaTime: FixedUpdate의 호출 간격 시간. 프레임 속도에 관계없이 일정한 속도로 움직이게 합니다.
        rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.deltaTime);
    }
}
