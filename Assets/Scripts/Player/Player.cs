using UnityEngine;
using System.Collections;
using System;

[SelectionBase]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(KnockBack))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    public event EventHandler OnPlayerDeath;
    public event EventHandler OnFlashBlink;

    [SerializeField] private float _movingSpeed = 10f;
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private float _minMovingSpeed = 0.1f;
    [SerializeField] private float _damageRecoveryTime = 0.5f;
    [Header("Dash Settings")]

    [SerializeField] private int _dashSpeed = 4;
    [SerializeField] private float _dashTime = 0.2f;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private float _dashCoolDownTime = 0.25f;
    private Vector2 inputVector;

    private bool _isRunning;
    private bool _isAlive;
    private bool _canTakeDamage;
    private bool _isDashing;

    private int _currentHealth;

    private float _initialMovingSpeed;
    private float _originalSpeed;

    private Rigidbody2D _rb;
    private KnockBack _knockBack;
    private Camera _mainCamera;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        _rb = GetComponent<Rigidbody2D>();
        _knockBack = GetComponent<KnockBack>();

        _mainCamera = Camera.main;

        _initialMovingSpeed = _movingSpeed;
    }

    private void Start()
    {
        _currentHealth = _maxHealth;

        _canTakeDamage = true;
        _isAlive = true;

        _originalSpeed = _movingSpeed;

        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPlayerDash += GameInput_OnPlayerDash;
            GameInput.Instance.OnPlayerAttack += GameInput_OnPlayerAttack;
        }
    }

    private void OnDestroy()
    {
        if (GameInput.Instance != null)
        {
            GameInput.Instance.OnPlayerAttack -= GameInput_OnPlayerAttack;
            GameInput.Instance.OnPlayerDash -= GameInput_OnPlayerDash;
        }
    }

    private void Update()
    {
        if (GameInput.Instance == null)
            return;

        inputVector = GameInput.Instance.GetMovementVector();
    }

    private void FixedUpdate()
    {
        if (_knockBack != null && _knockBack.IsGettingKnockedBack)
            return;

        HandleMovement();
    }

    public bool IsAlive()
    {
        return _isAlive;
    }

    public void TakeDamage(Transform damageSource, int damage)
    {
        if (_knockBack == null)
            return;

        if (_canTakeDamage && _isAlive)
        {
            _canTakeDamage = false;

            _currentHealth -= damage;
            _currentHealth = Mathf.Max(0, _currentHealth);

            Debug.Log(_currentHealth);

            _knockBack.GetKnockBack(damageSource);

            OnFlashBlink?.Invoke(this, EventArgs.Empty);

            StartCoroutine(DamageRecoveryRoutine());
        }

        DetectDeath();
    }

    private void DetectDeath()
    {
        if (_currentHealth <= 0 && _isAlive)
        {
            _canTakeDamage = false;
            _isAlive = false;

            _knockBack.StopKnockBackMovement();

            GameInput.Instance.DisableMovement();

            OnPlayerDeath?.Invoke(this, EventArgs.Empty);
        }
    }

    private void GameInput_OnPlayerDash(object sender, System.EventArgs e)
    {
        Dash();
    }

    private void Dash()
    {
        if (!_isDashing)
        {
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        _isDashing = true;


        _movingSpeed = _originalSpeed * _dashSpeed;

        trailRenderer.emitting = true;

        yield return new WaitForSeconds(_dashTime);

        _movingSpeed = _originalSpeed;

        trailRenderer.emitting = false;
        yield return new WaitForSeconds(_dashCoolDownTime);
        _isDashing = false;

    }

    private IEnumerator DamageRecoveryRoutine()
    {
        yield return new WaitForSeconds(_damageRecoveryTime);

        _canTakeDamage = true;
    }

    public bool IsRunning()
    {
        return _isRunning;
    }

    private void GameInput_OnPlayerAttack(object sender, System.EventArgs e)
    {
        if (ActiveWeapon.Instance == null)
            return;

        ActiveWeapon.Instance.GetActiveWeapon().Attack();
    }

    private void HandleMovement()
    {
        Vector2 movement =
            inputVector * (_movingSpeed * Time.fixedDeltaTime);

        _rb.MovePosition(_rb.position + movement);

        _isRunning =
            inputVector.sqrMagnitude >
            (_minMovingSpeed * _minMovingSpeed);
    }

    public Vector3 GetPlayerPosition()
    {
        return _mainCamera.WorldToScreenPoint(transform.position);
    }
}