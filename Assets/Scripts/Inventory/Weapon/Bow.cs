using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bow : RangedWeapon
{

    private void Awake()
    {
        hasNockedAmmo = false;
        knockedAmmo = null;

        ammos = new List<Ammo>();
    }

    void Start()
    {
        
    }


    protected override void Update()
    {
        base.Update();
    }

    private void OnEnable()
    {
        InitializeAmmoList();
    }

    private void OnDisable()
    {
        ReturnAmmosToInventory();
    }

    private void InitializeAmmoList()
    {
        InventoryController inventoryController = owner.GetComponent<InventoryController>();

        foreach (string itemName in inventoryController.items.Keys)
        {
            if (itemName == "Arrow")
            {
                foreach (InventoryItem arrowItem in inventoryController.items[itemName].ToList())
                {
                    ammos.Add(arrowItem.GetComponent<Arrow>());
                }
                inventoryController.items.Remove(itemName);
                inventoryController.RemoveSlot(itemName);
            }
        }
    }

    private void ReturnAmmosToInventory()
    {
        InventoryController inventoryController = owner.GetComponent<InventoryController>();
        if (inventoryController.items.ContainsKey("Arrow"))
        {
            foreach (Arrow arrow in ammos.ToList())
            {
                inventoryController.items["Arrow"].Add(arrow);
            }
            ammos.Clear();
        }
        else
        {
            inventoryController.items.Add("Arrow", new List<InventoryItem>());
            foreach (Arrow arrow in ammos.ToList())
            {
                inventoryController.items["Arrow"].Add(arrow);
            }
            inventoryController.AddSlot(ammos.Count,"Arrow", ammos[0]);
            ammos.Clear() ;
        }
    }

    public override void Reload()
    {
        base.Reload();
        //if (hasNockedAmmo) return;
        if(ammos.Count > 0)
        {
            hasNockedAmmo = true;
            knockedAmmo = (Arrow)ammos[0];
            ammos.Remove(knockedAmmo);
            owner.GetComponent<Animator>().SetTrigger("KnockArrow");
        }
    }

    public override void Aim()
    {
        base.Aim();
        owner.GetComponent<Animator>().SetTrigger("DrawArrow");
        hasDrawn = true;
    }

    public override void UnAim()
    {
        base.UnAim();
        owner.GetComponent<Animator>().SetTrigger("UnDrawArrow");
        hasDrawn = false;
    }

    public override void Loose()
    {
        base.Loose();
        owner.GetComponent<Animator>().SetTrigger("ReleaseArrow");
        knockedAmmo.GetComponent<AmmoDamageController>().Loose();
        knockedAmmo = null;
        hasNockedAmmo = false;
    }
}
