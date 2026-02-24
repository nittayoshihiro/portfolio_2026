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
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private ParticleSystem _muzzleEffect;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _shootSE;
    [SerializeField] private AudioClip _damageSE;
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
        if (GameManager.Instance.CurrentGameState() != GameState.Playing)
            return;

        RotateToMouse();

        //à⁄ìÆèàóù
        _input.x = Input.GetAxisRaw("Horizontal");
        _input.y = Input.GetAxisRaw("Vertical");
        _input = _input.normalized;
        _rb.linearVelocity = _input * _moveSpeed;

        //íeî≠éÀèàóù
        _fireTimer -= Time.deltaTime;

        if (Input.GetMouseButtonDown(0) &&_fireTimer <= 0f)
        {
            Fire();
            _muzzleEffect.Play();
            _fireTimer = _fireCooldown;
        }
    }

    //íeî≠éÀèàóù
    private void Fire()
    {
        _audioSource.PlayOneShot(_shootSE);
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = mousePos - _firePoint.position;

        //íeÇê∂ê¨
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(direction);
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        _currentHP -= damage;
        _audioSource.PlayOneShot(_damageSE);
        StartCoroutine(FlashRed(this.GetComponent<SpriteRenderer>()));
        UpdateHPUI();
        

        if (_currentHP <= 0)
        {
            Debug.Log("Player Dead");
            GameManager.Instance.GameOver();
        }

        StartCoroutine(InvincibleCoroutine());
    }

    private System.Collections.IEnumerator InvincibleCoroutine()
    {
        _isInvincible = true;

        yield return new WaitForSeconds(_invincibleTime);

        _isInvincible = false;
    }

    void UpdateHPUI()
    {
        _hpText.text = "HP: " + _currentHP;
    }

    public void ResetPlayer()
    {
        _currentHP = _maxHP;
        UpdateHPUI();
        transform.position = Vector3.zero;
    }

    IEnumerator FlashRed(SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void RotateToMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = mousePos - transform.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
