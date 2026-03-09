using System.Collections;
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected int _maxHP = 3;
    [SerializeField] protected SpriteRenderer _spriteRenderer;
    [SerializeField] protected Sprite _normalSprite;
    [SerializeField] protected Sprite _damageSprite;
    protected int _currentHP;

    protected virtual void Start()
    {
        _currentHP = _maxHP;
        WaveManager.Instance.RegisterEnemy();
    }

    public virtual void TakeDamage(int damage)
    {
        _currentHP -= damage;
        StartCoroutine(DamageFlash());
        UpdateColor();
        if (_currentHP <= 0)
        {
            Die();
        }
    }

    IEnumerator DamageFlash()
    {
        GameManager.Instance.PlayEnemyDamage();
        _spriteRenderer.sprite = _damageSprite;

        yield return new WaitForSeconds(0.1f);

        _spriteRenderer.sprite = _normalSprite;
    }

    void UpdateColor()
    {
        float hpRate = (float)_currentHP / _maxHP;

        if (hpRate > 0.7f)
            _spriteRenderer.color = Color.white;
        else if (hpRate > 0.4f)
            _spriteRenderer.color = Color.yellow;
        else
            _spriteRenderer.color = Color.red;
    }

    protected virtual void Die()
    {
        WaveManager.Instance.RemoveEnemy();
        Destroy(gameObject);
    }
    
}