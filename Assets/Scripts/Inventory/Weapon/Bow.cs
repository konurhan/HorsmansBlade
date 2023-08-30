using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Bow : RangedWeapon
{

    protected override void Awake()
    {
        base.Awake();
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

    private void OnDestroy()
    {
        ReturnAmmosToInventory();
    }

    public override void OnEquipped()
    {
        base.OnEquipped();
        InitializeAmmoList();
    }

    public override void OnDropped()
    {
        base.OnDropped();
        ReturnAmmosToInventory();
    }

    private void InitializeAmmoList()
    {
        InventoryController inventoryController = owner.GetComponent<InventoryController>();

        foreach (ItemDescriptor itemDesc in inventoryController.items.Keys)
        {
            if (itemDesc.itemName == "ArrowLongBow")
            {
                int count = inventoryController.items[itemDesc];
                for (int i = 0; i < count; i++)
                {
                    Debug.Log(itemDesc.itemPrefabPath + itemDesc.itemName);
                    GameObject arrow = Instantiate(Resources.Load(itemDesc.itemPrefabPath+ itemDesc.itemName)) as GameObject;
                    arrow.GetComponent<InventoryItem>().SetOwnerReference(owner);
                    arrow.GetComponent<Rigidbody>().isKinematic = true;
                    arrow.GetComponent<BoxCollider>().isTrigger = true;
                    arrow.SetActive(false);
                    ammos.Add(arrow.GetComponent<Arrow>());
                }
                inventoryController.items.Remove(itemDesc);
                inventoryController.RemoveSlot(itemDesc);

                break;
            }
        }
    }

    private void ReturnAmmosToInventory()//arrow are already created gameobjects
    {
        InventoryController inventoryController = owner.GetComponent<InventoryController>();

        ItemDescriptor arrowDescriptor;

        int count = ammos.Count;

        for (int i = 0;i < count;i++)
        {
            Destroy(ammos[i].gameObject);
        }
        ammos.Clear();

        arrowDescriptor = inventoryController.FindDescriptionInPlayerInventory("Arrow");
        if (arrowDescriptor != null)
        {
            inventoryController.items[arrowDescriptor] += count;
        }
        else
        {
            arrowDescriptor = inventoryController.FindDescriptionInWorldInventory("Arrow");
            inventoryController.items.Add(arrowDescriptor, count);
            inventoryController.AddSlot(count, arrowDescriptor);
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
            //owner.GetComponent<Animator>().SetTrigger("KnockArrow");
        }
    }

    public override void Aim()
    {
        base.Aim();
        owner.GetComponent<Animator>().SetTrigger("DrawArrow");
        GetComponent<Animator>().SetBool("Draw", true);
        hasDrawn = true;
    }

    public override void UnAim()
    {
        base.UnAim();
        owner.GetComponent<Animator>().SetTrigger("AbortDrawArrow");
        GetComponent<Animator>().SetBool("Draw", false);
        hasDrawn = false;
    }

    public override void Loose()
    {
        Vector3 camForward = owner.GetComponent<CameraController>().aimCamera.transform.forward;

        base.Loose();
        owner.GetComponent<Animator>().SetTrigger("ReleaseArrow");
        GetComponent<Animator>().SetTrigger("ShootArrow");
        Invoke(nameof(SetDrawFalse), 0.1f);//so that animator doesn't go into abort draw state

        float skill = owner.GetComponent<PlayerController>().rangedSkillLevel / 100f;
        knockedAmmo.GetComponent<Ammo>().Shoot(maximumForce*skill, camForward);
        knockedAmmo = null;
        hasNockedAmmo = false;
        hasDrawn = false;
    }

    public void SetDrawFalse()
    {
        GetComponent<Animator>().SetBool("Draw", false);
    }
}
