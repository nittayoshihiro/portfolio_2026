using UnityEngine;

public class SmallEnemy : EnemyBase
{
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private GameObject _deathEffect;

    private Rigidbody2D _rb;
    private Transform _player;

    protected override void Start()
    {
        base.Start(); // HP初期化

        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_player == null) return;

        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // ↓向きスプライト用補正
        transform.rotation = Quaternion.Euler(0, 0, angle + 90f);
    }

    protected override void Die()
    {
        GameObject effect = Instantiate(_deathEffect, transform.position, Quaternion.identity);
        Destroy(effect, 1f);

        GameManager.Instance.PlayEnemyDeath();
        GameManager.Instance.AddScore(10);

        base.Die(); //最後にDestroy
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
