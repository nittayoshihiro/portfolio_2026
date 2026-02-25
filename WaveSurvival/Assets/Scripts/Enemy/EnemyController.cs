using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private int _maxHP = 3;
    [SerializeField] private GameObject _deathEffect;
    private int _currentHP;
    private Rigidbody2D _rb;
    private Transform _player;


    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;
        _rb = GetComponent<Rigidbody2D>();
        _currentHP = _maxHP;
    }

    void Update()
    {
        if (_player == null)
        {
            return;
        }

        Vector2 direction = (_player.position - transform.position).normalized;
        _rb.linearVelocity = direction * _moveSpeed;
    }

    public void TakeDamage(int damage)
    {
        _currentHP -= damage;

        if (_currentHP <= 0)
        {
            Instantiate(_deathEffect, transform.position, Quaternion.identity);
            GameManager.Instance.PlayEnemyDeath();
            GameManager.Instance.AddScore(10);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //?‚ÍŒ©‚Â‚©‚Á‚½Žž
            collision.gameObject.GetComponent<PlayerController>()?.TakeDamage(1);
        }
    }

}
