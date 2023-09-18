using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : InventoryItem
{
    [SerializeField] protected float requiredSkillLevel;
    //bool isEquipped;

    protected override void Update()
    {
        base.Update();
    }

    public override void OnEquipped()
    {
        base.OnEquipped();
        //isEquipped = true;
    }

    public override void OnDropped()
    {
        base.OnDropped();
        //isEquipped = false;
    }
}
