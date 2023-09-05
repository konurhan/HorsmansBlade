using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedWeapon : Weapon
{
    [SerializeField] protected float maximumForce;//maximum force out of 100
    protected List<Ammo> ammos;
    public bool hasNockedAmmo;
    public bool hasDrawn;
    public Ammo knockedAmmo;
    void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();
    }

    public virtual void Reload()
    {

    }

    public virtual void Aim()
    {

    }

    public virtual void UnAim()
    {

    }

    public virtual void Loose()
    {

    }
}
