using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : InventoryItem //don't inherit from armour but write similar methods to takedamage of armour class
{
    void Start()
    {
        
    }


    protected override void Update()
    {
        base.Update();
    }

    /*public void SetOwnerReference(GameObject owner)
    {
        this.owner = owner;
    }*/
}
