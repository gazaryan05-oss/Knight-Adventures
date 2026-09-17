using UnityEngine;
using System;
public class SwordSlashVisual : MonoBehaviour
{
    [SerializeField] private Sword sword;

    private const string ATTACK = "Attack";
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        sword.OnSwordSwing += Sword_OnSwordSwing;
    }

    private void Sword_OnSwordSwing(object sender, EventArgs e)
    {
        animator.SetTrigger(ATTACK);
    }

}
