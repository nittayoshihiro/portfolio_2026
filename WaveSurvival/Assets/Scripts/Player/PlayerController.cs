using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireCooldown = 0.3f;
    private float _fireTimer;
    private Rigidbody2D _rb;
    private Vector2 _input;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    { 
        //ˆÚ“®ˆ—
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        _input = _input.normalized;
        _rb.linearVelocity = _input * _moveSpeed;

        //’e”­Ëˆ—
        _fireTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space) && _fireTimer <= 0f)
        {
            Fire();
            _fireTimer = _fireCooldown;
        }
    }

    //’e”­Ëˆ—
    private void Fire()
    {
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        Vector2 dir = Vector2.right; 
        bullet.GetComponent<Bullet>().Init(dir);
    }

}
