using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class KnockBack : MonoBehaviour
{
    [SerializeField] private float _knockBackForce = 3f;
    [SerializeField] private float _knockBackMovingTimeMax = 1f;

    private float _knockBackMovingTime;
    private Rigidbody2D _rb;

    public bool IsGettingKnockedBack { get; private set; }

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        _knockBackMovingTime -= Time.deltaTime;

        if (_knockBackMovingTime <= 0f)
        {
            StopKnockBackMovement();
        }
    }

    public void GetKnockBack(Transform damageSource)
    {
        IsGettingKnockedBack = true;

        _knockBackMovingTime = _knockBackMovingTimeMax;

        Vector2 difference =
            (transform.position - damageSource.position).normalized;

        _rb.linearVelocity = Vector2.zero;

        _rb.AddForce(difference * _knockBackForce, ForceMode2D.Impulse);
    }

    public void StopKnockBackMovement()
    {
        _rb.linearVelocity = Vector2.zero;

        IsGettingKnockedBack = false;
    }
}