using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] protected float maximumDamage;//multiplied by (user_skill/100)
    [SerializeField] protected float attackRange;//use this to determine npc attackRadius

    public float MaxDamage { get { return maximumDamage; }}
    public float AttackRange { get {  return attackRange; }}

    protected override void Update()
    {
        base.Update();
    }

    public override void OnEquipped()
    {
        base.OnEquipped();
        gameObject.GetComponent<MeleeWeaponDamageController>().OnEquippedMelee();
    }

    public override void OnDropped()
    {
        gameObject.GetComponent<MeleeWeaponDamageController>().OnDroppedMelee();
        base.OnDropped();
    }
}
