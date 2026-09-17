using UnityEngine;

public class FlashBlink : MonoBehaviour
{
    [SerializeField] private MonoBehaviour _damagableObject;
    [SerializeField] private Color _blinkColor = Color.red;
    [SerializeField] private float _blinkDuration = 0.2f;

    private float _blinkTimer;
    private Color _defaultColor;
    private SpriteRenderer _spriteRenderer;
    private bool _isBlinking;
    private Player _cachedPlayer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _defaultColor = _spriteRenderer.color;
        _isBlinking = false;

        if (_damagableObject is Player player)
        {
            _cachedPlayer = player;
            _cachedPlayer.OnFlashBlink += DamagableObject_OnFlashBlink;
        }
    }

    private void OnDestroy()
    {
        if (_cachedPlayer != null)
        {
            _cachedPlayer.OnFlashBlink -= DamagableObject_OnFlashBlink;
        }
    }

    private void DamagableObject_OnFlashBlink(object sender, System.EventArgs e)
    {
        SetBlinkingColor();
    }

    private void Update()
    {
        if (_isBlinking)
        {
            _blinkTimer -= Time.deltaTime;
            if (_blinkTimer < 0)
            {
                SetDefaultColor();
            }
        }
    }

    private void SetBlinkingColor()
    {
        _blinkTimer = _blinkDuration;
        _isBlinking = true;
        _spriteRenderer.color = _blinkColor;
    }

    private void SetDefaultColor()
    {
        _spriteRenderer.color = _defaultColor;
        _isBlinking = false;
    }

    public void StopBlinking()
    {
        SetDefaultColor();
        _isBlinking = false;
    }
}
