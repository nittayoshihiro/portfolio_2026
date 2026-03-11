using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;
using TMPro;
using static GameManager;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _maxHP = 5;
    [SerializeField] private float _invincibleTime = 1f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireCooldown = 0.3f;
    [SerializeField] private ParticleSystem _muzzleEffect;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _shootSE;
    [SerializeField] private AudioClip _damageSE;
    [SerializeField] private int _maxBulletCount = 3;
    private int _currentBulletCount = 0;
    private float _fireTimer;
    private int _currentHP;
    private bool _isInvincible;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rb;
    private Vector2 _input;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
       _spriteRenderer = GetComponent<SpriteRenderer>();
        _currentHP = _maxHP;
    }

    void Update()
    {

        if (GameManager.Instance.CurrentGameState() != GameState.Playing)
        {
            return;
        }
        //É}ÉEÉX
        RotateToMouse();

        //à⁄ìÆèàóù
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        _input = _input.normalized;
        _rb.linearVelocity = _input * _moveSpeed;

        //íeî≠éÀèàóù
        _fireTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) && _currentBulletCount < _maxBulletCount)
        {
            Fire();
            _fireTimer = _fireCooldown;
        }
    }

    public int GetHP()
    {
        return _currentHP;
    }

    //íeî≠éÀèàóù
    private void Fire()
    {
        _currentBulletCount++;
        _muzzleEffect.Play();
        _audioSource.PlayOneShot(_shootSE);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = mousePos - _firePoint.position;

        //íeÇê∂ê¨
        Bullet bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity).GetComponent<Bullet>();
        bullet.Init(direction,this);
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        _currentHP -= damage;
        _audioSource.PlayOneShot(_damageSE);
        StartCoroutine(FlashRed(_spriteRenderer));
        

        if (_currentHP <= 0)
        {
            GameManager.Instance.GameOver();
        }

        StartCoroutine(InvincibleCoroutine());
    }

    private IEnumerator InvincibleCoroutine()
    {
        _isInvincible = true;

        yield return new WaitForSeconds(_invincibleTime);

        _isInvincible = false;
    }

    public void ResetPlayer()
    {
        _currentHP = _maxHP;
        transform.position = Vector3.zero;
        _spriteRenderer.color = Color.white;
    }

    IEnumerator FlashRed(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        switch (_currentHP)
        {
            case 0:
            case 1:
                spriteRenderer.color = Color.red;
                break;
            case 2:
                spriteRenderer.color = Color.yellow;
                break;
            case 3:
                spriteRenderer.color = Color.white;
                break;
        }
    }

    private void RotateToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = mousePos - transform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle-90f);
    }

    public void OnBulletDestroyed()
    {
        _currentBulletCount--;
    }
}
