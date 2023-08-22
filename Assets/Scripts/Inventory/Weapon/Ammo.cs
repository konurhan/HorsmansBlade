using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : InventoryItem
{
    [SerializeField] protected float damage;
    [SerializeField] protected float range;

    void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();
    }
}
