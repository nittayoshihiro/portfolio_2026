using UnityEngine;
using UnityEngine.UIElements;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private int _maxHP = 5;
    [SerializeField] private float _invincibleTime = 1f;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _fireCooldown = 0.3f;
    [SerializeField] private TMP_Text _hpText;
    [SerializeField] private GameObject _gameOverTextObject;
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

        if (Input.GetMouseButtonDown(0) &&_fireTimer <= 0f)
        {
            Fire();
            _fireTimer = _fireCooldown;
        }
    }

    //íeî≠éÀèàóù
    private void Fire()
    {
        
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        Vector2 direction = mousePos - _firePoint.position;

        //ÉvÉåÉCÉÑÅ[ÇâÒì]Ç≥ÇπÇÈ
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        //íeÇê∂ê¨
        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        bullet.GetComponent<Bullet>().Init(direction);
    }

    public void TakeDamage(int damage)
    {
        if (_isInvincible) return;

        _currentHP -= damage;
        UpdateHPUI();

        if (_currentHP <= 0)
        {
            Debug.Log("Player Dead");
            _gameOverTextObject.SetActive(true);
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

}
