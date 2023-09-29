using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour : InventoryItem
{
    [SerializeField] protected ArmourType armourType;
    public float protection;//provide in the editor for a prefab

    public float TakeDamage(float damage)
    {
        float remainingDamage;
        if (damage <= protection) 
        {
            remainingDamage = 0;
            protection -= damage;
        }
        else
        {
            remainingDamage = damage - protection;
            protection = 0;
        }
        
        if (protection == 0)
        { 
            Shatter();
        }

        return remainingDamage;
    }

    public void Shatter()
    {

    }

    public ArmourType GetArmourType()
    {
        return armourType;
    }

    /*private void OnDestroy()
    {
        Debug.Log(Name + " is going to be destroyed");
        Debug.Break();
    }*/
}

public enum ArmourType
{
    Head,
    Torso,
    Hands,
    Legs,
    Feet
}
