using UnityEngine;
using System;

[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(EnemyAI))]
public class EnemyEntity : MonoBehaviour
{
    [SerializeField] private EnemySO enemySO;

    public event EventHandler OnTakeHit;
    public event EventHandler OnDeath;

    private int _currentHealth;
    private bool _isDead;

    private PolygonCollider2D _polygonCollider2D;
    private BoxCollider2D _boxCollider2D;
    private EnemyAI _enemyAI;

    private void Awake()
    {
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _enemyAI = GetComponent<EnemyAI>();
    }

    private void Start()
    {
        if (enemySO == null)
        {
            Debug.LogError("EnemySO is missing on " + gameObject.name);
            return;
        }

        _currentHealth = enemySO.enemyHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Player player))
        {

            int damage = 1;

            player.TakeDamage(transform, damage);
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        OnTakeHit?.Invoke(this, EventArgs.Empty);

        DetectDeath();
    }

    public void PolygonColliderTurnOff()
    {
        _polygonCollider2D.enabled = false;
    }

    public void PolygonColliderTurnOn()
    {
        _polygonCollider2D.enabled = true;
    }

    private void DetectDeath()
    {
        if (_isDead) return;

        if (_currentHealth <= 0)
        {
            _isDead = true;

            _boxCollider2D.enabled = false;
            _polygonCollider2D.enabled = false;

            _enemyAI.enabled = false;

            OnDeath?.Invoke(this, EventArgs.Empty);

            Destroy(gameObject);
        }
    }
}
