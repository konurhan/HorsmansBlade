using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController : MonoBehaviour
{
    public static AnimatorController Instance { get; private set;}
    Animator animator;
    public int SpeedZ, SpeedX, Jump, CombatJump, Draw, Sheat, Shield, InwardSlash, OutwardSlash,
        DownwardSlash, GetParried, TakeHit, TurnRight, TurnLeft, EquipBow, DisarmBow, DrawArrow,
        ReleaseArrow, AbortDrawArrow, Gather, Died, isDead, usingWeapon, AbortDrawString, ShootArrow;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        animator = GetComponent<Animator>();

        SpeedZ = Animator.StringToHash("SpeedZ");
        SpeedX = Animator.StringToHash("SpeedX");
        Jump = Animator.StringToHash("Jump");
        CombatJump = Animator.StringToHash("CombatJump");
        Draw = Animator.StringToHash("Draw");
        Sheat = Animator.StringToHash("Sheat");
        Shield = Animator.StringToHash("Shield");
        InwardSlash = Animator.StringToHash("InwardSlash");
        OutwardSlash = Animator.StringToHash("OutwardSlash");
        DownwardSlash = Animator.StringToHash("DownwardSlash");
        GetParried = Animator.StringToHash("GetParried");
        TakeHit = Animator.StringToHash("TakeHit");
        TurnRight = Animator.StringToHash("TurnRight");
        TurnLeft = Animator.StringToHash("TurnLeft");
        EquipBow = Animator.StringToHash("EquipBow");
        DisarmBow = Animator.StringToHash("DisarmBow");
        DrawArrow = Animator.StringToHash("DrawArrow");
        ReleaseArrow = Animator.StringToHash("ReleaseArrow");
        AbortDrawArrow = Animator.StringToHash("AbortDrawArrow");
        Gather = Animator.StringToHash("Gather");
        Died = Animator.StringToHash("Died");
        isDead = Animator.StringToHash("isDead");
        usingWeapon = Animator.StringToHash("usingWeapon");

        AbortDrawString = Animator.StringToHash("AbortDrawString");
        ShootArrow = Animator.StringToHash("ShootArrow");
    }
}
