using UnityEngine;
using static GameManager;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject _smallEnemy;
    [SerializeField] private GameObject _mediumEnemy;
    [SerializeField] private GameObject _largeEnemy;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] _spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private float _waveDuration = 30f;
    [SerializeField] private float _spawnInterval = 2f;
    [SerializeField] private float _spawnStopTime = 15f;

    private float _waveTimer;
    private float _spawnTimer;
    private int _currentWave = 1;
    private bool _waveActive = false;
    private bool _survivePhase = false;
    private int _enemyCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 2‚Â–ÚˆÈ~‚ðÁ‚·
        }
    }

    void Start()
    {
        StartWave();
    }

    void Update()
    {

        if (GameManager.Instance.CurrentGameState() != GameState.Playing)
            return;

        if (!_waveActive)
            return;

        _waveTimer += Time.deltaTime;
        _spawnTimer += Time.deltaTime;

        // “G¶¬
        float remainingTime = _waveDuration - _waveTimer;

        if (remainingTime > _spawnStopTime)
        {
            if (_spawnTimer >= _spawnInterval)
            {
                SpawnEnemy();
                _spawnTimer = 0f;
            }
        }

        if (remainingTime <= _spawnStopTime && !_survivePhase)
        {
            _survivePhase = true;
            GameManager.Instance.ShowSurviveMessage();
        }

        // WaveŽžŠÔI—¹
        if (_waveTimer >= _waveDuration)
        {
            EndWave();
        }

        // “G‘S–Åƒ`ƒFƒbƒN
        if (_waveTimer > _spawnStopTime)
        {
            if (_enemyCount == 0)
            {
                EndWave();
            }
        }
    }

    public float GetWaveTimer()
    {
        return Mathf.Max(0, _waveDuration - _waveTimer);
    }

    public void StartWave()
    {
        _survivePhase = false;
        _waveActive = true;
        _waveTimer = 0f;
        _spawnTimer = 0f;

        GameManager.Instance.NextWave();
    }

    void EndWave()
    {
        if (!_waveActive) return;

        _waveActive = false;

        float remainTime = _waveDuration - _waveTimer;
        int timeBonus = Mathf.Max(0, Mathf.RoundToInt(remainTime * 10));

        // “G‘Síœ
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
        _enemyCount = 0;

        GameManager.Instance.WaveClear(timeBonus);
    }

    public void StartNextWaveFromGameManager()
    {
        StartNextWave();
    }

    void StartNextWave()
    {
        _currentWave++;

        if (_currentWave > 5)
        {
            GameManager.Instance.GameClear();
            return;
        }

        StartWave();
    }

    void SpawnEnemy()
    {
        int spawnIndex = Random.Range(0, _spawnPoints.Length);
        Transform spawn = _spawnPoints[spawnIndex];

        GameObject enemyToSpawn;
        float remainingTime = _waveDuration - _waveTimer;
        int rand = Random.Range(0, 100);

        if (_currentWave < 2)
        {
            enemyToSpawn = _smallEnemy;
        }
        else if (rand < 70)
        {
            enemyToSpawn = _smallEnemy;
        }
        else if (rand < 90)
        {
            enemyToSpawn = _mediumEnemy;
        }
        else
        {
            enemyToSpawn = _largeEnemy;
        }

        Instantiate(enemyToSpawn, spawn.position, Quaternion.identity);
    }
    public void RegisterEnemy()
    {
        _enemyCount++;
    }

    public void RemoveEnemy()
    {
        _enemyCount--;
    }

    public void ResetWave()
    {
        // “G‘Síœ
        foreach (var enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Destroy(enemy);
        }
        _enemyCount = 0;
        _currentWave = 0;
        _waveTimer = 0;
        _waveActive = false;
    }
}
