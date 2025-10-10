using UnityEngine;

public class Bullet : MonoBehaviour
{

    [Tooltip("�Ѿ��� �̵� �ӵ�")]
    public float speed = 10f;

    [Tooltip("�Ѿ��� ������������ �ð� (��)")]
    public float lifeTime = 5f;

    // �Ѿ��� ���ư� ���� (BulletSpawner�� ������ �� ����)
    // public���� �����ؾ� �ܺ� ��ũ��Ʈ���� ������ �� �ֽ��ϴ�.
    [HideInInspector] // Inspector â������ ������ �ʰ� ����
    public Vector2 direction = Vector2.right;

    // ��ũ��Ʈ�� Ȱ��ȭ�� �� �� �� ȣ��˴ϴ�.
    void Start()
    {
        // lifeTime ���Ŀ� �ڵ����� �� ���� ������Ʈ�� �ı��մϴ�.
        // �Ѿ��� ȭ�� �ۿ� ������ ��� ���̴� ���� �����մϴ�.
        Destroy(gameObject, lifeTime);
    }

    // �� �����Ӹ��� ȣ��˴ϴ�.
    void Update()
    {
        // ������ direction �������� speed�� �ӵ��� ��� �̵���ŵ�ϴ�.
        // Time.deltaTime�� ���� ������ �ӵ��� ������� ������ �ӵ��� �����մϴ�.
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
