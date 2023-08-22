using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    [SerializeField] protected float maximumDamage;//multiplied by (user_skill/100)
    [SerializeField] protected float attackRange;//use this to determine npc attackRadius

    public float MaxDamage { get { return maximumDamage; }}
    public float AttackRange { get {  return attackRange; }}
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }
}
