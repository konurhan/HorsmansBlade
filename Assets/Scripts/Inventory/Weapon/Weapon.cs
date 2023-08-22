using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : InventoryItem
{
    /*[SerializeField] protected string wName;
    [SerializeField] protected float weight;
    [SerializeField] protected float price;
    public string Name { get { return wName; } set { wName = value; } }
    public float Weight { get { return weight; } set { weight = value; } }
    public float Price { get { return price; } set { price = value; } }*/

    

    [SerializeField] protected float requiredSkillLevel;

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
