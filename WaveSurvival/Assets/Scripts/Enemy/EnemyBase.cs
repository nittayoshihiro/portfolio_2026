using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [SerializeField] protected int _maxHP = 3;
    protected int _currentHP;

    protected virtual void Start()
    {
        _currentHP = _maxHP;
    }

    public virtual void TakeDamage(int damage)
    {
        _currentHP -= damage;

        if (_currentHP <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}