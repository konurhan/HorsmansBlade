using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armour : MonoBehaviour
{
    public float protection;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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
}
