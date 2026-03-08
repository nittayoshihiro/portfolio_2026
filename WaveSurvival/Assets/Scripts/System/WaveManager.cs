using UnityEngine;
using static GameManager;

public class WaveManager : MonoBehaviour
{
    [Header("Enemy Prefabs")]
    [SerializeField] private GameObject _smallEnemy;
    [SerializeField] private GameObject _mediumEnemy;
    [SerializeField] private GameObject _largeEnemy;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] _spawnPoints;

    [Header("Wave Settings")]
    [SerializeField] private float _waveDuration = 30f;
    [SerializeField] private float _spawnInterval = 2f;

    private float _waveTimer;
    private float _spawnTimer;
    private int _currentWave = 1;
    private bool _waveActive = false;
    private float _enemyCheckDelay = 2f;
    //private bool _waitingForNextWave = false;

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

        // 敵生成
        if (_spawnTimer >= _spawnInterval)
        {
            SpawnEnemy();
            _spawnTimer = 0f;
        }

        // Wave時間終了
        if (_waveTimer >= _waveDuration)
        {
            EndWave();
        }

        // 敵全滅チェック
        if (_waveTimer > _enemyCheckDelay)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length == 0)
            {
                EndWave();
            }
        }
    }

    public float GetWaveTimer()
    {
        return Mathf.Max(0, _waveDuration - _waveTimer);
    }

    void StartWave()
    {
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
        int bonus = Mathf.Max(0, Mathf.RoundToInt(remainTime * 10));

        GameManager.Instance.WaveClear(bonus);
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
            GameManager.Instance.GameOver();
            return;
        }

        StartWave();
    }

    void SpawnEnemy()
    {
        int spawnIndex = Random.Range(0, _spawnPoints.Length);
        Transform spawn = _spawnPoints[spawnIndex];

        GameObject enemyToSpawn;

        int rand = Random.Range(0, 100);

        if (_currentWave <= 2)
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
}
