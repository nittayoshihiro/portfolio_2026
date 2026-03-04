using UnityEngine;

public class LargeEnemy : EnemyBase
{
    [SerializeField] private float _moveSpeed = 1.2f;
    [SerializeField] private GameObject _smallEnemyPrefab;
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private Transform _spawnPoint1;
    [SerializeField] private Transform _spawnPoint2;
    [SerializeField] private int _maxChildCount = 6;
    [SerializeField] private int _maxSpawnCount = 5;
    [SerializeField] private GameObject _deathEffect;

    private float _spawnTimer;
    private int _spawnedCount;
    private Rigidbody2D _rb;
    private Transform _player;

    protected override void Start()
    {
        base.Start();

        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_player == null) return;

        // ゆっくり追尾
        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // ↓向きスプライト用補正
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

        // 小敵生成
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval && _spawnedCount < _maxSpawnCount)
        {
            SpawnSmallEnemy();
            _spawnTimer = 0f;
            _spawnedCount++;
        }
    }

    void SpawnSmallEnemy()
    {
        // 子の数制限（小敵のみカウント）
        int childCount = 0;
        foreach (Transform child in transform)
        {
            if (child.GetComponent<SmallEnemy>() != null)
                childCount++;
        }

        if (childCount >= _maxChildCount) return;

        GameObject small = Instantiate(
            _smallEnemyPrefab,
            _spawnPoint1.position,
            Quaternion.identity
        );

        //子にする
        small.transform.parent = transform;

        small = Instantiate(
            _smallEnemyPrefab,
            _spawnPoint2.position,
            Quaternion.identity
        );

        //子にする
        small.transform.parent = transform;
    }

    protected override void Die()
    {
        GameObject effect = Instantiate(_deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1f);

        GameManager.Instance.PlayEnemyDeath();
        GameManager.Instance.AddScore(100);
        base.Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject
                .GetComponent<PlayerController>()?
                .TakeDamage(1);
        }
    }
}
