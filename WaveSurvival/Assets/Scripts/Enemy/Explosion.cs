using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float _radius = 2f;
    [SerializeField] int _damage = 1;
    [SerializeField] float _lifeTime = 1f;
    [SerializeField] LayerMask _damageLayer;

    void Start()
    {
        Explode();
        Destroy(gameObject, _lifeTime);
    }

    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            _radius,
            _damageLayer
        );

        foreach (Collider2D hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damage);
            }


            PlayerController player = hit.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(_damage);
            }
               
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _radius);
    }
}