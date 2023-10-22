using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Bow : RangedWeapon
{
    WaitForSecondsRealtime travelTime;
    Coroutine currentSlowMoRoutine;

    protected override void Awake()
    {
        base.Awake();
        hasNockedAmmo = false;
        knockedAmmo = null;

        travelTime = new WaitForSecondsRealtime(0.1f);
        ammos = new List<Ammo>();
    }

    void Start()
    {
        
    }


    protected override void Update()
    {
        base.Update();
        if ( hasDrawn )
        {
            Vector3 camForward = owner.GetComponent<CameraController>().aimCamera.transform.forward;
            Debug.DrawRay(owner.GetComponent<CameraController>().aimCamera.transform.position, camForward * 100);
        }
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
        ReturnAmmosToInventory();
        base.OnDropped();
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
        if (owner == null)
        {
            Debug.Log("bow has no owner to return ammos");
            return;
        }

        InventoryController inventoryController = owner.GetComponent<InventoryController>();

        ItemDescriptor arrowDescriptor;

        int count = ammos.Count;

        for (int i = 0;i < count;i++)
        {
            Destroy(ammos[i].gameObject);
        }
        ammos.Clear();

        arrowDescriptor = inventoryController.FindDescriptionInPlayerInventory("ArrowLongBow");
        if (arrowDescriptor != null)
        {
            inventoryController.items[arrowDescriptor] += count;
        }
        else
        {
            arrowDescriptor = inventoryController.FindDescriptionInWorldInventory("ArrowLongBow");
            inventoryController.items.Add(arrowDescriptor, count);
            inventoryController.AddSlot(count, arrowDescriptor);
        }
    }

    public override void Reload()
    {
        Debug.Log("Bow reload is called");
        base.Reload();
        if(ammos.Count > 0)
        {
            hasNockedAmmo = true;
            knockedAmmo = (Arrow)ammos[0];
            ammos.Remove(knockedAmmo);
            knockedAmmo.OnEquipped();
            //owner.GetComponent<Animator>().SetTrigger("KnockArrow");
        }
    }

    public override void Aim()
    {
        base.Aim();
        owner.GetComponent<Animator>().SetTrigger(AnimatorController.Instance.DrawArrow);
        GetComponent<Animator>().SetBool(AnimatorController.Instance.Draw, true);
        hasDrawn = true;

        if (currentSlowMoRoutine != null)
        {
            StopCoroutine(currentSlowMoRoutine);
            currentSlowMoRoutine = null;
        }
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    public override void UnAim()
    {
        base.UnAim();
        owner.GetComponent<Animator>().SetTrigger(AnimatorController.Instance.AbortDrawArrow);
        GetComponent<Animator>().SetTrigger(AnimatorController.Instance.AbortDrawString);
        GetComponent<Animator>().SetBool(AnimatorController.Instance.Draw, false);
        hasDrawn = false;
    }

    public override void Loose()
    {
        Vector3 ammoForward;
        if (owner.GetComponent<PlayerAttack>().recentOnCrossObject == null)
        {
            ammoForward = owner.GetComponent<CameraController>().aimCamera.transform.forward;
        }
        else
        {
            float magnitude = (owner.GetComponent<PlayerAttack>().recentOnCrossObjectContactPoint - gameObject.transform.position).magnitude;
            ammoForward = (owner.GetComponent<PlayerAttack>().recentOnCrossObjectContactPoint - gameObject.transform.position) / magnitude;
            //Debug.DrawRay(gameObject.transform.position, ammoForward*100, Color.magenta, 2);
            Debug.DrawLine(gameObject.transform.position, owner.GetComponent<PlayerAttack>().recentOnCrossObjectContactPoint, Color.magenta, 3);
        }
        
        base.Loose();
        owner.GetComponent<Animator>().SetTrigger(AnimatorController.Instance.ReleaseArrow);
        GetComponent<Animator>().SetTrigger(AnimatorController.Instance.ShootArrow);
        GetComponent<Animator>().SetBool(AnimatorController.Instance.Draw, false);

        float skillMultiplier = owner.GetComponent<PlayerController>().rangedSkillLevel / 100f;
        knockedAmmo.GetComponent<Ammo>().Shoot(maximumForce, skillMultiplier, ammoForward);
        knockedAmmo = null;
        hasNockedAmmo = false;
        hasDrawn = false;

        currentSlowMoRoutine = StartCoroutine(nameof(SlowMoOnFireAmmo));
    }

    public void SetDrawFalse()
    {
        GetComponent<Animator>().SetBool(AnimatorController.Instance.Draw, false);
    }

    public IEnumerator SlowMoOnFireAmmo()
    {
        Time.timeScale = 1/2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale / 50f;
        float passedTime = 0;
        
        while (passedTime < 0.2f)
        {
            passedTime += 0.1f;
            yield return travelTime;
        }
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }
}
