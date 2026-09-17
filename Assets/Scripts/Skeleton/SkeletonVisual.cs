using UnityEngine;
using System;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class SkeletonVisual : MonoBehaviour
{
    [SerializeField] private EnemyAI _enemyAI;
    [SerializeField] private EnemyEntity _enemyEntity;
    [SerializeField] private GameObject _enemyShadow;
    private Animator _animator;

    private static readonly int IS_RUNNING = Animator.StringToHash("IsRunning");
    private const string TAKEHIT = "TakeHit";
    private const string IS_DIE = "IsDie";
    private static readonly int CHASING_SPEED_MULTIPLIER = Animator.StringToHash("ChasingSpeedMultiplier");
    private static readonly int ATTACK = Animator.StringToHash("Attack");

    SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

    }

    private void Start()
    {
        _enemyAI.OnEnemyAttack += OnEnemyAttack;
        _enemyEntity.OnTakeHit += _enemyEntity_OnTakeHit;
        _enemyEntity.OnDeath += _enemyEntity_OnDeath;
    }


    private void _enemyEntity_OnTakeHit(object sender, EventArgs e)
    {
        if (_animator == null) return;

        _animator.SetTrigger(TAKEHIT);
    }

    private void _enemyEntity_OnDeath(object sender, EventArgs e)
    {
        if (_enemyAI != null)
        {
            _animator.SetBool(IS_DIE, true);
            _spriteRenderer.sortingOrder = -1;
            _enemyShadow.SetActive(false);
        }
    }

    private void Update()
    {
        if (_enemyAI == null) return;

        _animator.SetBool(IS_RUNNING, _enemyAI.IsRunning);
        _animator.SetFloat(CHASING_SPEED_MULTIPLIER, _enemyAI.GetRoamingAnimationSpeed());
    }

    // Animation Event: начало удара (включить хитбокс)
    public void TriggerAttackAnimationTurnOff()
    {
        if (_enemyEntity == null) return;
        _enemyEntity.PolygonColliderTurnOff();
    }

    // Animation Event: конец удара (выключить хитбокс)
    public void TriggerAttackAnimationTurnOn()
    {
        if (_enemyEntity == null) return;
        _enemyEntity.PolygonColliderTurnOn();
    }

    private void OnEnemyAttack(object sender, EventArgs e)
    {
        if (_animator == null) return;

        _animator.SetTrigger(ATTACK);
    }
}