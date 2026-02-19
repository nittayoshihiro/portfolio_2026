using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private Transform[] _spawnPoints;

    [SerializeField] private float _timeBetweenWaves = 5f;

    private int currentWave = 0;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= _timeBetweenWaves)
        {
            StartNextWave();
            timer = 0f;
        }
    }

    void StartNextWave()
    {
        currentWave++;

        int enemyCount = currentWave + 2;

        for (int i = 0; i < enemyCount; i++)
        {
            SpawnEnemy();
        }

        Debug.Log("Wave: " + currentWave);
    }

    void SpawnEnemy()
    {
        int index = Random.Range(0, _spawnPoints.Length);
        Instantiate(_enemyPrefab, _spawnPoints[index].position, Quaternion.identity);
    }
}
