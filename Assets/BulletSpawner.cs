using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [Tooltip("생성할 총알의 프리팹")]
    public GameObject bulletPrefab;

    [Tooltip("총알 생성 주기 (초)")]
    public float spawnRate = 1f;

    [Tooltip("총알이 화면 가장자리에서 얼마나 밖에서 생성될지에 대한 여유값")]
    public float spawnOffset = 1f;

    // 다음 총알 생성까지의 시간을 추적할 타이머
    private float timer;

    // 메인 카메라
    private Camera mainCamera;

    void Start()
    {
        // 성능을 위해 Camera.main을 매번 호출하지 않고 변수에 저장해 둡니다.
        mainCamera = Camera.main;
    }

    void Update()
    {
        // 타이머를 업데이트합니다.
        timer += Time.deltaTime;

        // 타이머가 설정된 생성 주기를 넘어서면 총알을 생성합니다.
        if (timer >= spawnRate)
        {
            SpawnBullet();
            // 타이머를 리셋합니다.
            timer = 0f;
        }
    }

    void SpawnBullet()
    {
        if (bulletPrefab == null || mainCamera == null)
        {
            Debug.LogError("Bullet Prefab 또는 Main Camera가 설정되지 않았습니다!");
            return;
        }

        // 0 또는 1을 랜덤하게 선택하여 왼쪽(0) 또는 오른쪽(1)에서 생성할지 결정
        bool spawnOnLeft = Random.Range(0, 2) == 0;

        // 1. 스폰 위치 계산
        // 화면의 세로 범위 (위쪽 끝 ~ 아래쪽 끝)를 월드 좌표 기준으로 구합니다.
        float screenHalfHeight = mainCamera.orthographicSize;
        float randomY = Random.Range(-screenHalfHeight, screenHalfHeight);

        // 화면의 가로 범위 (왼쪽 끝, 오른쪽 끝)를 월드 좌표 기준으로 구합니다.
        float screenHalfWidth = screenHalfHeight * mainCamera.aspect;
        float spawnX = spawnOnLeft ? -screenHalfWidth - spawnOffset : screenHalfWidth + spawnOffset;

        Vector2 spawnPosition = new Vector2(spawnX, randomY);

        // 2. 총알 생성
        GameObject newBullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity);

        // 3. 생성된 총알에 방향 설정
        Bullet bulletScript = newBullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            // 왼쪽에서 생성되었다면 오른쪽으로, 오른쪽에서 생성되었다면 왼쪽으로 방향을 설정
            bulletScript.direction = spawnOnLeft ? Vector2.right : Vector2.left;
        }
    }
}
