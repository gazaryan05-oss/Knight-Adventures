using System;
using UnityEngine;
using UnityEngine.AI;
using Utils;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private State _startingState = State.Roaming;

    [SerializeField] private float _roamingDistanceMax = 7f;
    [SerializeField] private float _roamingDistanceMin = 3f;
    [SerializeField] private float _roamingTimerMax = 2f;

    [SerializeField] private bool _isChasingEnemy = false;
    [SerializeField] private float _chasingDistance = 4f;
    [SerializeField] private float _chasingSpeedMultiplier = 2f;

    [SerializeField] private bool _isAttackingEnemy = false;
    [SerializeField] private float _attackingDistance = 2f;
    [SerializeField] private float _attackRate = 2f;

    private float _nextAttackTime = 0f;
    private NavMeshAgent _navMeshAgent;
    private State _currentState;
    private float _roamingTimer;
    private Vector3 _roamPosition;
    private Vector3 _startingPosition;
    private float _roamingSpeed;
    private float _chasingSpeed;
    private float _nextChangeDirectionTime = 0f;
    private float _checkDirectionDuration = 0.1f;
    private Vector3 _lastPosition;

    public event EventHandler OnEnemyAttack;

    public bool IsRunning
    {
        get
        {
            return _navMeshAgent != null && _navMeshAgent.velocity != Vector3.zero;
        }
    }

    public enum State
    {
        Idle,
        Roaming,
        Chasing,
        Attacking,
        Death
    }

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        if (_navMeshAgent == null)
        {
            Debug.LogError("NavMeshAgent missing on " + gameObject.name);
            return;
        }

        _navMeshAgent.updateRotation = false;
        _navMeshAgent.updateUpAxis = false;

        _currentState = _startingState;
        _roamingSpeed = _navMeshAgent.speed;
        _chasingSpeed = _navMeshAgent.speed * _chasingSpeedMultiplier;
    }

    private void Start()
    {
        _roamingTimer = _roamingTimerMax;
    }

    private void Update()
    {
        if (_navMeshAgent == null) return;
        StateHandler();
        MovementDirection();
    }

    private void StateHandler()
    {
        switch (_currentState)
        {
            case State.Roaming://randomqayl
                _roamingTimer -= Time.deltaTime;
                if (_roamingTimer < 0f)
                {
                    Roaming();
                    _roamingTimer = _roamingTimerMax;
                }
                CheckCurrentState();
                break;

            case State.Chasing://playerihetevic
                ChasingTarget();
                CheckCurrentState();
                break;

            case State.Attacking:
                AttackTarget();
                CheckCurrentState();
                break;

            case State.Death:
                break;

            case State.Idle:
            default:
                break;
        }
    }

    private bool IsAgentReady()//stugumeenemynpatrasteqaylelu
    {
        return _navMeshAgent != null && _navMeshAgent.enabled && _navMeshAgent.isOnNavMesh;
    }

    private void ChasingTarget()//etechkaplayerchisharjvum
    {
        if (Player.Instance == null || !IsAgentReady()) return;
        _navMeshAgent.SetDestination(Player.Instance.transform.position);
    }

    public float GetRoamingAnimationSpeed()
    {
        if (_navMeshAgent == null) return 1f;
        return _navMeshAgent.speed / _roamingSpeed;
    }

    private void CheckCurrentState()
    {
        if (Player.Instance == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, Player.Instance.transform.position);
        State newState = State.Roaming;

        if (_isChasingEnemy && distanceToPlayer < _chasingDistance)
        {
            newState = State.Chasing;
        }

        if (_isAttackingEnemy && distanceToPlayer <= _attackingDistance)
        {
            if (Player.Instance.IsAlive())
            {
                newState = State.Attacking;
            }
            else
            {
                newState = State.Roaming;
            }
        }

        if (newState != _currentState)
        {
            switch (newState)
            {
                case State.Chasing:
                    if (IsAgentReady()) _navMeshAgent.ResetPath();  // ← проверка
                    _navMeshAgent.speed = _chasingSpeed;
                    break;

                case State.Roaming:
                    _roamingTimer = 0f;
                    _navMeshAgent.speed = _roamingSpeed;
                    break;

                case State.Attacking:
                    if (IsAgentReady()) _navMeshAgent.ResetPath();  // ← проверка
                    break;
            }
            _currentState = newState;
        }
    }

    private void AttackTarget()
    {
        if (Time.time > _nextAttackTime)
        {
            OnEnemyAttack?.Invoke(this, EventArgs.Empty);
            _nextAttackTime = Time.time + _attackRate;
        }
    }

    private void MovementDirection()
    {
        if (Time.time > _nextChangeDirectionTime)
        {
            if (IsRunning)
            {
                ChangeFacingDirection(_lastPosition, transform.position);
            }
            else if (_currentState == State.Attacking && Player.Instance != null)
            {
                ChangeFacingDirection(transform.position, Player.Instance.transform.position);
            }
            _lastPosition = transform.position;
            _nextChangeDirectionTime = Time.time + _checkDirectionDuration;
        }
    }

    private void Roaming()//randomqayl
    {
        if (!IsAgentReady()) return;

        _startingPosition = transform.position;
        _roamPosition = GetRoamingPosition();
        _navMeshAgent.SetDestination(_roamPosition);
    }

    private Vector3 GetRoamingPosition()
    {
        return _startingPosition + Helper.GetRandomDir() * UnityEngine.Random.Range(_roamingDistanceMin, _roamingDistanceMax);
    }

    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void SetDeathState()
    {
        _currentState = State.Death;
        if (IsAgentReady())
        {
            _navMeshAgent.ResetPath();
            _navMeshAgent.enabled = false;
        }
        this.enabled = false;
    }
}