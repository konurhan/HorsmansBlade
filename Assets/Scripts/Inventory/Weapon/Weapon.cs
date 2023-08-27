using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : InventoryItem
{
    [SerializeField] protected float requiredSkillLevel;

    void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();
    }

    /*public override void OnEquipped()
    {
        base.OnEquipped();
    }

    public override void OnDropped()
    {
        base.OnDropped();
    }*/
}
