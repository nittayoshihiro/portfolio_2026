using UnityEngine;

public class MediumEnemy : EnemyBase
{
    [SerializeField] private float _moveSpeed = 1.8f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private float _fireInterval = 2f;
    [SerializeField] private GameObject _deathEffect;
    [SerializeField] private Transform _firePoint;

    private float _fireTimer;
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

        // 追尾移動
        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // ↓向きスプライト用補正
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);

        // 弾発射
        _fireTimer += Time.deltaTime;
        if (_fireTimer >= _fireInterval)
        {
            Shoot();
            _fireTimer = 0f;
        }
    }

    void Shoot()
    {
        Vector2 direction = (_player.position - transform.position).normalized;
        GameObject bullet = Instantiate(_bulletPrefab,_firePoint.position,Quaternion.identity);
        bullet.GetComponent<EnemyBullet>().SetDirection(direction);
    }

    protected override void Die()
    {
        GameObject effect = Instantiate(_deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1f);

        GameManager.Instance.PlayEnemyDeath();
        GameManager.Instance.AddScore(30);

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
