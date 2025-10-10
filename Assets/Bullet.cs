using UnityEngine;

public class Bullet : MonoBehaviour
{

    [Tooltip("총알의 이동 속도")]
    public float speed = 10f;

    [Tooltip("총알이 사라지기까지의 시간 (초)")]
    public float lifeTime = 5f;

    // 총알이 날아갈 방향 (BulletSpawner가 설정해 줄 변수)
    // public으로 선언해야 외부 스크립트에서 접근할 수 있습니다.
    [HideInInspector] // Inspector 창에서는 보이지 않게 설정
    public Vector2 direction = Vector2.right;

    // 스크립트가 활성화될 때 한 번 호출됩니다.
    void Start()
    {
        // lifeTime 이후에 자동으로 이 게임 오브젝트를 파괴합니다.
        // 총알이 화면 밖에 나가도 계속 쌓이는 것을 방지합니다.
        Destroy(gameObject, lifeTime);
    }

    // 매 프레임마다 호출됩니다.
    void Update()
    {
        // 지정된 direction 방향으로 speed의 속도로 계속 이동시킵니다.
        // Time.deltaTime을 곱해 프레임 속도에 관계없이 일정한 속도를 보장합니다.
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
