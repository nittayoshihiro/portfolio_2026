using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed = 10f;
    [SerializeField] private float _lifeTime = 3f;

    private Vector2 _direction;
    private float _timer;
    private PlayerController _player;

    public void Init(Vector2 dir, PlayerController player)
    {
        _player = player;
        _direction = dir.normalized;
        _timer = _lifeTime;
    }

    void Update()
    {
        transform.Translate(_direction * _speed * Time.deltaTime);

        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyController>()?.TakeDamage(1);
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (_player != null)
        {
            _player.OnBulletDestroyed();
        }
    }
}
