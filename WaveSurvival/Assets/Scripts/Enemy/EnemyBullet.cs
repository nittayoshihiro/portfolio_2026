using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private int _damage = 1;

    private Rigidbody2D _rb;
    private Vector2 _direction;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    public void SetDirection(Vector2 dir)
    {
        _direction = dir.normalized;
    }

    void Start()
    {
        _rb.linearVelocity = _direction * _speed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーに当たった
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerController>()?.TakeDamage(_damage);
            Destroy(gameObject);
        }

        // 敵に当たった敵にもダメージ
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyBase>()?.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
