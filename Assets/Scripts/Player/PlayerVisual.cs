using UnityEngine;
using System;

public class PlayerVisual : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer _spriteRenderer;
    private FlashBlink _flashBlink;
    private const string IS_RUNNING = "isRunning";
    private const string IS_DIE = "IsDie";

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _flashBlink = GetComponent<FlashBlink>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDeath += Player_OnPlayerDeath;
        }
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
        {
            Player.Instance.OnPlayerDeath -= Player_OnPlayerDeath;
        }
    }

    private void Player_OnPlayerDeath(object sender, EventArgs e)
    {
        animator.SetBool(IS_DIE, true);
        if (_flashBlink != null)
        {
            _flashBlink.StopBlinking();
        }
    }

    private void Update()
    {
        if (Player.Instance == null) return;

        animator.SetBool("IsRunning", Player.Instance.IsRunning());

        if (Player.Instance.IsAlive())
        {
            AdjustPlayerFacingDirection();
        }
    }

    private void AdjustPlayerFacingDirection()
    {
        if (GameInput.Instance == null || Player.Instance == null) return;

        Vector3 mousePosition = GameInput.Instance.GetMousePosition();
        Vector3 playerPosition = Player.Instance.GetPlayerPosition();

        if (mousePosition.x < playerPosition.x)
        {
            _spriteRenderer.flipX = true;
        }
        else
        {
            _spriteRenderer.flipX = false;
        }
    }
}
