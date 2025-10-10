using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Tooltip("������ �Ѿ��� ������")]
    public GameObject bulletPrefab;

    [Tooltip("�Ѿ� ���� �ֱ� (��)")]
    public float spawnRate = 1f;

    [Tooltip("�Ѿ��� ȭ�� �����ڸ����� �󸶳� �ۿ��� ���������� ���� ������")]
    public float spawnOffset = 1f;

    // ���� �Ѿ� ���������� �ð��� ������ Ÿ�̸�
    private float timer;

    // ���� ī�޶�
    private Camera mainCamera;

    void Start()
    {
        // ������ ���� Camera.main�� �Ź� ȣ������ �ʰ� ������ ������ �Ӵϴ�.
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Ÿ�̸Ӹ� ������Ʈ�մϴ�.
        timer += Time.deltaTime;

        // Ÿ�̸Ӱ� ������ ���� �ֱ⸦ �Ѿ�� �Ѿ��� �����մϴ�.
        if (timer >= spawnRate)
        {
            SpawnBullet();
            // Ÿ�̸Ӹ� �����մϴ�.
            timer = 0f;
        }
    }

    void SpawnBullet()
    {
        if (bulletPrefab == null || mainCamera == null)
        {
            Debug.LogError("Bullet Prefab �Ǵ� Main Camera�� �������� �ʾҽ��ϴ�!");
            return;
        }

        // 0 �Ǵ� 1�� �����ϰ� �����Ͽ� ����(0) �Ǵ� ������(1)���� �������� ����
        bool spawnOnLeft = Random.Range(0, 2) == 0;

        // 1. ���� ��ġ ���
        // ȭ���� ���� ���� (���� �� ~ �Ʒ��� ��)�� ���� ��ǥ �������� ���մϴ�.
        float screenHalfHeight = mainCamera.orthographicSize;
        float randomY = Random.Range(-screenHalfHeight, screenHalfHeight);

        // ȭ���� ���� ���� (���� ��, ������ ��)�� ���� ��ǥ �������� ���մϴ�.
        float screenHalfWidth = screenHalfHeight * mainCamera.aspect;
        float spawnX = spawnOnLeft ? -screenHalfWidth - spawnOffset : screenHalfWidth + spawnOffset;

        Vector2 spawnPosition = new Vector2(spawnX, randomY);

        // 2. �Ѿ� ����
        GameObject newBullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // 3. ������ �Ѿ˿� ���� ����
        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            // ���ʿ��� �����Ǿ��ٸ� ����������, �����ʿ��� �����Ǿ��ٸ� �������� ������ ����
            bulletScript.direction = spawnOnLeft ? Vector2.right : Vector2.left;
        }
    }
}
