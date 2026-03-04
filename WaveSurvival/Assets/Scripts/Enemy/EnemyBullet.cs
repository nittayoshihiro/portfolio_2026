using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;
    [SerializeField] private int _damage = 1;

    private Rigidbody2D _rb;

    public void Init(Vector2 direction)
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.linearVelocity = direction.normalized * _speed;
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
