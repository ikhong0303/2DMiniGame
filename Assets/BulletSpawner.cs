using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Tooltip("생성할 탄환 프리팹")]
    public GameObject bulletPrefab;

    [Tooltip("기본 탄환 생성 간격 (초)")]
    public float baseSpawnInterval = 1f;

    [Tooltip("최소 탄환 생성 간격 (초)")]
    public float minSpawnInterval = 0.2f;

    [Tooltip("초당 간격 감소량")]
    public float spawnAcceleration = 0.01f;

    [Tooltip("카메라 경계 밖으로 얼마만큼 띄워서 생성할지")]
    public float spawnOffset = 1f;

    [Tooltip("탄환 기본 속도")]
    public float baseBulletSpeed = 5f;

    [Tooltip("초당 탄환 속도 증가량")]
    public float speedAcceleration = 0.1f;

    private float timer;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGameRunning)
        {
            return;
        }

        timer += Time.deltaTime;
        float interval = GetCurrentInterval();
        if (timer < interval)
        {
            return;
        }

        timer = 0f;
        SpawnBullet(GetCurrentSpeed());
    }

    private float GetCurrentInterval()
    {
        float elapsed = GameManager.Instance != null ? GameManager.Instance.ElapsedTime : Time.timeSinceLevelLoad;
        float interval = baseSpawnInterval - elapsed * spawnAcceleration;
        return Mathf.Max(minSpawnInterval, interval);
    }

    private float GetCurrentSpeed()
    {
        float elapsed = GameManager.Instance != null ? GameManager.Instance.ElapsedTime : Time.timeSinceLevelLoad;
        return baseBulletSpeed + elapsed * speedAcceleration;
    }

    private void SpawnBullet(float bulletSpeed)
    {
        if (bulletPrefab == null || mainCamera == null)
        {
            Debug.LogWarning("BulletPrefab 또는 Camera가 설정되지 않았습니다.");
            return;
        }

        float halfHeight = mainCamera.orthographicSize;
        float halfWidth = halfHeight * mainCamera.aspect;

        int edge = Random.Range(0, 4);
        Vector2 spawnPos;
        Vector2 direction;

        switch (edge)
        {
            case 0: // Left
                spawnPos = new Vector2(-halfWidth - spawnOffset, Random.Range(-halfHeight, halfHeight));
                direction = Vector2.right;
                break;
            case 1: // Right
                spawnPos = new Vector2(halfWidth + spawnOffset, Random.Range(-halfHeight, halfHeight));
                direction = Vector2.left;
                break;
            case 2: // Top
                spawnPos = new Vector2(Random.Range(-halfWidth, halfWidth), halfHeight + spawnOffset);
                direction = Vector2.down;
                break;
            default: // Bottom
                spawnPos = new Vector2(Random.Range(-halfWidth, halfWidth), -halfHeight - spawnOffset);
                direction = Vector2.up;
                break;
        }

        GameObject newBullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        if (newBullet.TryGetComponent(out Bullet bullet))
        {
            bullet.direction = direction;
            bullet.speed = bulletSpeed;
        }
    }
}
