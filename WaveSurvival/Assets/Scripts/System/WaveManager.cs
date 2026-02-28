using UnityEngine;
using static GameManager;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;
    [SerializeField] private float _timeBetweenWaves = 5f;

    private int _currentWave = 0;
    private float _timer;

    void Update()
    {
        if (GameManager.Instance.CurrentGameState() != GameState.Playing)
        {
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= _timeBetweenWaves)
        {
            StartNextWave();
            _timer = 0f;
        }
    }

    void StartNextWave()
    {
        _currentWave++;

        int enemyCount = _currentWave + 2;

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }
    }

    void SpawnEnemy()
    {
        int index = Random.Range(0, _spawnPoints.Length);
        Instantiate(_enemyPrefab, _spawnPoints[index].position, Quaternion.identity);
    }
}
