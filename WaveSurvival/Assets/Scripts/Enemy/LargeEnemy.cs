using UnityEngine;

public class LargeEnemy : EnemyBase
{
    [SerializeField] private float _moveSpeed = 1.2f;
    [SerializeField] private GameObject _smallEnemyPrefab;
    [SerializeField] private float _spawnInterval = 3f;
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

        // ‚ä‚Á‚­‚è’Ç”ö
        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;

        // ¬“G¶¬
        _spawnTimer += Time.deltaTime;

        if (_spawnTimer >= _spawnInterval && _spawnedCount < _maxSpawnCount)
        {
            Instantiate(_smallEnemyPrefab, transform.position, Quaternion.identity);
            _spawnTimer = 0f;
            _spawnedCount++;
        }
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
