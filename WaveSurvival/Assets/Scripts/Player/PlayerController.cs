using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _maxHP = 5;
    [SerializeField] private float _invincibleTime = 1f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireCooldown = 0.3f;
    private float _fireTimer;
    private int _currentHP;
    private bool _isInvincible;
    private Rigidbody2D _rb;
    private Vector2 _input;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _currentHP = _maxHP;
    }

    void Update()
    { 
        //à⁄ìÆèàóù
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        _input = _input.normalized;
        _rb.linearVelocity = _input * _moveSpeed;

        //íeî≠éÀèàóù
        _fireTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && _fireTimer <= 0f)
        {
            Fire();
            _fireTimer = _fireCooldown;
        }
    }

    //íeî≠éÀèàóù
    private void Fire()
    {
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        Vector2 dir = Vector2.right; 
        bullet.GetComponent<Bullet>().Init(dir);
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        _currentHP -= damage;

        if (_currentHP <= 0)
        {
            Debug.Log("Player Dead");
        }

        StartCoroutine(InvincibleCoroutine());
    }

    private System.Collections.IEnumerator InvincibleCoroutine()
    {
        _isInvincible = true;

        yield return new WaitForSeconds(_invincibleTime);

        _isInvincible = false;
    }
}
