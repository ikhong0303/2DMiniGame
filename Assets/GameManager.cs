using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("플레이어 설정")]
    [Tooltip("게임에서 조작할 플레이어 스크립트")]
    [SerializeField] private PlayerMove player;

    [Tooltip("초기 생명 수")]
    [SerializeField] private int startingLives = 1;

    [Tooltip("피격 후 무적 유지 시간(초)")]
    [SerializeField] private float invincibilityDuration = 1.5f;

    [Header("UI")]
    [Tooltip("생명 수를 표시하는 Text")]
    [SerializeField] private Text lifeText;

    [Tooltip("생존 시간을 표시하는 Text")]
    [SerializeField] private Text timeText;

    [Tooltip("격려 메시지 컨테이너")]
    [SerializeField] private GameObject messagePanel;

    [Tooltip("격려 메시지를 표시할 Text")]
    [SerializeField] private Text messageText;

    [Tooltip("게임 오버 패널")]
    [SerializeField] private GameObject gameOverPanel;

    [Tooltip("게임 오버 시 점수를 표시하는 Text")]
    [SerializeField] private Text gameOverScoreText;

    [Tooltip("랭킹을 표시하는 Text")]
    [SerializeField] private Text rankingText;

    [Header("격려 메시지 설정")]
    [Tooltip("10초마다 순환될 격려 메시지 목록")]
    [SerializeField] private string[] encouragementMessages =
    {
        "좋아요! 계속 버텨봐요!",
        "집중력을 유지하세요!",
        "조금만 더!",
        "리듬을 잃지 마세요!"
    };

    [Tooltip("격려 메시지가 유지되는 시간(초)")]
    [SerializeField] private float encouragementDuration = 2.5f;

    [Header("하트 아이템")]
    [Tooltip("생명을 회복하는 하트 아이템 프리팹")]
    [SerializeField] private GameObject heartItemPrefab;

    [Tooltip("하트 아이템이 등장하는 간격(초)")]
    [SerializeField] private float heartSpawnInterval = 15f;

    [Tooltip("하트 아이템이 사라지기까지 유지되는 시간(초)")]
    [SerializeField] private float heartLifetime = 6f;

    [Header("연동 컴포넌트")]
    [Tooltip("탄환 생성기를 연결해 게임 오버 시 멈춥니다.")]
    [SerializeField] private BulletSpawner bulletSpawner;

    private readonly List<float> leaderboard = new List<float>();
    private const string LeaderboardKey = "GM_LEADERBOARD";
    private const int MaxLeaderboardEntries = 5;

    private float elapsedTime;
    private int lives;
    private bool isGameOver;
    private float nextEncouragementTime = 10f;
    private float encouragementTimer;
    private float heartTimer;
    private bool isInvincible;
    private float invincibleTimer;

    public bool IsGameRunning => !isGameOver;
    public float ElapsedTime => elapsedTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (player == null)
        {
            player = FindObjectOfType<PlayerMove>();
        }

        if (bulletSpawner == null)
        {
            bulletSpawner = FindObjectOfType<BulletSpawner>();
        }

        LoadLeaderboard();
    }

    private void Start()
    {
        StartGame();
    }

    private void Update()
    {
        if (!IsGameRunning)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                RestartGame();
            }

            return;
        }

        elapsedTime += Time.deltaTime;
        UpdateTimerUI();

        HandleEncouragement();
        HandleHeartSpawn();
        UpdateInvincibility();
    }

    private void StartGame()
    {
        Time.timeScale = 1f;
        isGameOver = false;
        elapsedTime = 0f;
        lives = Mathf.Max(1, startingLives);
        nextEncouragementTime = 10f;
        encouragementTimer = 0f;
        heartTimer = 0f;
        isInvincible = false;
        invincibleTimer = 0f;

        UpdateLifeUI();
        UpdateTimerUI();
        UpdateLeaderboardUI();

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (bulletSpawner != null)
        {
            bulletSpawner.enabled = true;
        }
    }

    public void DamagePlayer()
    {
        if (!IsGameRunning || isInvincible)
        {
            return;
        }

        lives = Mathf.Max(0, lives - 1);
        UpdateLifeUI();

        if (lives <= 0)
        {
            HandleGameOver();
        }
        else
        {
            isInvincible = true;
            invincibleTimer = invincibilityDuration;
        }
    }

    public void AddLife(int amount = 1)
    {
        if (!IsGameRunning || amount <= 0)
        {
            return;
        }

        lives += amount;
        UpdateLifeUI();
    }

    private void HandleGameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;

        if (bulletSpawner != null)
        {
            bulletSpawner.enabled = false;
        }

        TryAddScore(elapsedTime);

        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        encouragementTimer = 0f;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (gameOverScoreText != null)
        {
            gameOverScoreText.text = $"생존 시간: {elapsedTime:F1}초";
        }
    }

    private void HandleEncouragement()
    {
        if (elapsedTime >= nextEncouragementTime)
        {
            ShowEncouragement();
            nextEncouragementTime += 10f;
        }

        if (encouragementTimer > 0f)
        {
            encouragementTimer -= Time.deltaTime;
            if (encouragementTimer <= 0f)
            {
                HideEncouragement();
            }
        }
    }

    private void ShowEncouragement()
    {
        if (messagePanel == null || messageText == null || encouragementMessages == null || encouragementMessages.Length == 0)
        {
            return;
        }

        var message = encouragementMessages[Random.Range(0, encouragementMessages.Length)];
        messageText.text = message;
        messagePanel.SetActive(true);
        encouragementTimer = encouragementDuration;
    }

    private void HideEncouragement()
    {
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }
    }

    private void HandleHeartSpawn()
    {
        if (heartItemPrefab == null)
        {
            return;
        }

        heartTimer += Time.deltaTime;
        if (heartTimer < heartSpawnInterval)
        {
            return;
        }

        heartTimer = 0f;

        Camera cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        float halfHeight = cam.orthographicSize;
        float halfWidth = halfHeight * cam.aspect;
        Vector2 randomPosition = new Vector2(Random.Range(-halfWidth * 0.8f, halfWidth * 0.8f), Random.Range(-halfHeight * 0.8f, halfHeight * 0.8f));

        GameObject heart = Instantiate(heartItemPrefab, randomPosition, Quaternion.identity);
        HeartItem item = heart.GetComponent<HeartItem>();
        if (item != null)
        {
            item.Configure(heartLifetime);
        }
        else
        {
            Destroy(heart, heartLifetime);
        }
    }

    private void UpdateInvincibility()
    {
        if (!isInvincible)
        {
            return;
        }

        invincibleTimer -= Time.deltaTime;
        if (invincibleTimer <= 0f)
        {
            isInvincible = false;
        }
    }

    private void UpdateLifeUI()
    {
        if (lifeText == null)
        {
            return;
        }

        if (lives <= 0)
        {
            lifeText.text = "Life : 0";
            return;
        }

        string heartDisplay = new string('\u2665', Mathf.Clamp(lives, 0, 10));
        lifeText.text = $"Life : {lives}  {heartDisplay}";
    }

    private void UpdateTimerUI()
    {
        if (timeText == null)
        {
            return;
        }

        timeText.text = $"Time : {elapsedTime:F1}s";
    }

    private void TryAddScore(float score)
    {
        leaderboard.Add(score);
        leaderboard.Sort((a, b) => b.CompareTo(a));
        if (leaderboard.Count > MaxLeaderboardEntries)
        {
            leaderboard.RemoveRange(MaxLeaderboardEntries, leaderboard.Count - MaxLeaderboardEntries);
        }

        SaveLeaderboard();
        UpdateLeaderboardUI();
    }

    private void LoadLeaderboard()
    {
        leaderboard.Clear();
        string saved = PlayerPrefs.GetString(LeaderboardKey, string.Empty);
        if (string.IsNullOrEmpty(saved))
        {
            return;
        }

        string[] parts = saved.Split('|');
        foreach (string part in parts)
        {
            if (float.TryParse(part, out float value))
            {
                leaderboard.Add(value);
            }
        }

        leaderboard.Sort((a, b) => b.CompareTo(a));
        if (leaderboard.Count > MaxLeaderboardEntries)
        {
            leaderboard.RemoveRange(MaxLeaderboardEntries, leaderboard.Count - MaxLeaderboardEntries);
        }
    }

    private void SaveLeaderboard()
    {
        string data = string.Join("|", leaderboard.Select(v => v.ToString("F3")));
        PlayerPrefs.SetString(LeaderboardKey, data);
        PlayerPrefs.Save();
    }

    private void UpdateLeaderboardUI()
    {
        if (rankingText == null)
        {
            return;
        }

        var builder = new StringBuilder();
        builder.AppendLine("TOP 5");
        for (int i = 0; i < MaxLeaderboardEntries; i++)
        {
            if (i < leaderboard.Count)
            {
                builder.AppendLine($"{i + 1}. {leaderboard[i]:F1}초");
            }
            else
            {
                builder.AppendLine($"{i + 1}. ---");
            }
        }

        rankingText.text = builder.ToString();
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
