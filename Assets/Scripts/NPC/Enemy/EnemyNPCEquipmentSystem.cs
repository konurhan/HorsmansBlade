using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class EnemyNPCEquipmentSystem : MonoBehaviour
{
    public GameObject meleeOneHanded;
    public GameObject meleeTwoHanded;
    public GameObject shield;
    
    public Transform MeleeHandTransform;
    public Transform MeleeSheatTransform;
    public Transform ShieldHandTransform;

    private Animator animator;

    [SerializeField] private GameObject recentMeleeWeapon;

    WaitForSecondsRealtime waitTime = new WaitForSecondsRealtime(0.01f);

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void Start()
    {
        
    }

    void Update()
    {
        //HandleCombatActions();
    }

    public void EquipShield(ItemDescriptor itemDesc)
    {
        GameObject shieldNew = Instantiate(Resources.Load(itemDesc.itemPrefabPath + itemDesc.itemName)) as GameObject;
        
        if(shield != null)
        {
            shield.GetComponent<InventoryItem>().OnDropped();
            Destroy(shield);
        }

        shield = shieldNew;

        shield.GetComponent<InventoryItem>().SetOwnerReference(gameObject);
        shield.GetComponentInChildren<Rigidbody>().isKinematic = true;
        shield.GetComponentInChildren<BoxCollider>().isTrigger = true;

        shield.gameObject.transform.SetParent(ShieldHandTransform, false);
        shield.gameObject.SetActive(false);
    }

    public void EquipMeleeWeapon(ItemDescriptor itemDesc)
    {
        GameObject meleeWeapon = Instantiate(Resources.Load(itemDesc.itemPrefabPath+itemDesc.itemName), MeleeSheatTransform) as GameObject;
        if (meleeWeapon.GetComponent<MeleeWeapon>() == null)//might be a ranged weapon, NPCs won't be able to equip ranged weapons
        {
            Destroy(meleeWeapon);
            return;
        }
        if (meleeWeapon.GetComponent<IOneHanded>() != null)
        {
            if (meleeOneHanded != null)
            {
                meleeOneHanded.GetComponent<InventoryItem>().OnDropped();
                Destroy(meleeOneHanded);
            }
            meleeOneHanded = meleeWeapon;
        }
        else
        {
            if (meleeTwoHanded != null)
            {
                meleeTwoHanded.GetComponent<InventoryItem>().OnDropped();
                Destroy(meleeTwoHanded);
            }
            meleeTwoHanded = meleeWeapon;
        }
        meleeWeapon.GetComponent<InventoryItem>().SetOwnerReference(gameObject);
        meleeWeapon.GetComponentInChildren<Rigidbody>().isKinematic = true;
        meleeWeapon.GetComponentInChildren<BoxCollider>().isTrigger = true;

        meleeWeapon.gameObject.transform.SetParent(MeleeSheatTransform, false);
        meleeWeapon.gameObject.transform.localPosition = Vector3.zero;
        meleeWeapon.gameObject.transform.localEulerAngles = Vector3.zero;
        meleeWeapon.gameObject.SetActive(true);

        meleeWeapon.GetComponent<InventoryItem>().OnEquipped();

        recentMeleeWeapon = meleeWeapon;
    }

    public void DestroyEquippedWeapons()
    {
        if(meleeOneHanded != null)
        {
            Destroy(meleeOneHanded);
        }
        if (meleeTwoHanded != null)
        {
            Destroy(meleeTwoHanded);
        }
        if (shield != null)
        {
            Destroy(shield);
        }
    }

    public void UnequipWeapon(Weapon weapon)
    {
        Destroy(weapon.gameObject);
    }

    public void EquipArmour(ItemDescriptor itemDesc)
    {
        GameObject armourObj = Instantiate(Resources.Load(itemDesc.itemPrefabPath + itemDesc.itemName)) as GameObject;

        armourObj.GetComponent<InventoryItem>().SetOwnerReference(gameObject);

        armourObj.GetComponent<Rigidbody>().isKinematic = true;
        BoxCollider[] colls = armourObj.GetComponents<BoxCollider>();
        foreach (BoxCollider col in colls)
        {
            col.enabled = false;
        }
        armourObj.GetComponent<MeshRenderer>().enabled = false;

        Armour armour = armourObj.GetComponent<Armour>();
        ArmourType type = armour.GetArmourType();
        EnemyController controller = GetComponent<EnemyController>();

        switch (type)
        {
            case ArmourType.Head:
                if (controller.ArmourSlots.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipAndDestroyArmour(controller.head.GetComponent<BodyPart>().armour);
                }
                controller.ArmourSlots.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/" + itemDesc.itemName);
                controller.head.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
            case ArmourType.Torso:
                if (controller.ArmourSlots.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipAndDestroyArmour(controller.torso.GetComponent<BodyPart>().armour);
                }
                controller.NakedParts.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.ArmourSlots.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/" + itemDesc.itemName);
                controller.torso.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.leftUpperArm.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.rightUpperArm.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
            case ArmourType.Feet:
                if (controller.ArmourSlots.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipAndDestroyArmour(controller.legLeftLower.GetComponent<BodyPart>().armour);
                }
                controller.NakedParts.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.ArmourSlots.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/" + itemDesc.itemName);
                controller.legLeftLower.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.legRightLower.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
            case ArmourType.Hands:
                if (controller.ArmourSlots.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipAndDestroyArmour(controller.leftForearm.GetComponent<BodyPart>().armour);
                }
                controller.NakedParts.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.ArmourSlots.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/" + itemDesc.itemName);
                controller.leftForearm.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.rightForearm.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
            case ArmourType.Legs:
                if (controller.ArmourSlots.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipAndDestroyArmour(controller.legLeftUpper.GetComponent<BodyPart>().armour);
                }
                controller.NakedParts.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.ArmourSlots.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/" + itemDesc.itemName);
                controller.legLeftUpper.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.legRightUpper.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
        }
    }

    public void UnEquipAllArmours()
    {
        EnemyController controller = GetComponent<EnemyController>();
        if (controller.head.GetComponent<BodyPart>().armour != null)//helmet
        {
            UnequipAndDestroyArmour(controller.head.GetComponent<BodyPart>().armour);
        }
        if (controller.torso.GetComponent<BodyPart>().armour)//body
        {
            UnequipAndDestroyArmour(controller.torso.GetComponent<BodyPart>().armour);
        }
        if (controller.legLeftLower.GetComponent<BodyPart>().armour)//boots
        {
            UnequipAndDestroyArmour(controller.legLeftLower.GetComponent<BodyPart>().armour);
        }
        if (controller.leftForearm.GetComponent<BodyPart>().armour)//guantlets
        {
            UnequipAndDestroyArmour(controller.leftForearm.GetComponent<BodyPart>().armour);
        }
        if (controller.legLeftUpper.GetComponent<BodyPart>().armour)//pants
        {
            UnequipAndDestroyArmour(controller.legLeftUpper.GetComponent<BodyPart>().armour);
        }
    }

    public void UnequipAndDestroyArmour(Armour armour)
    {
        ArmourType type = armour.GetArmourType();
        EnemyController controller = GetComponent<EnemyController>();

        switch (type)
        {
            case ArmourType.Head:
                controller.ArmourSlots.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.head.GetComponent<BodyPart>().RemoveArmour();
                break;
            case ArmourType.Torso:
                controller.NakedParts.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.torso.GetComponent<BodyPart>().RemoveArmour();
                controller.leftUpperArm.GetComponent<BodyPart>().RemoveArmour();
                controller.rightUpperArm.GetComponent<BodyPart>().RemoveArmour();
                break;
            case ArmourType.Feet:
                controller.NakedParts.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.legLeftLower.GetComponent<BodyPart>().RemoveArmour();
                controller.legRightLower.GetComponent<BodyPart>().RemoveArmour();
                break;
            case ArmourType.Hands:
                controller.NakedParts.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.leftForearm.GetComponent<BodyPart>().RemoveArmour();
                controller.rightForearm.GetComponent<BodyPart>().RemoveArmour();
                break;
            case ArmourType.Legs:
                controller.NakedParts.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.legLeftUpper.GetComponent<BodyPart>().RemoveArmour();
                controller.legRightUpper.GetComponent<BodyPart>().RemoveArmour();
                break;
        }

        Destroy(armour.gameObject);//destroying the object before returning it back to the inventory ==>> BUT HOW TO SAVE CONDITION, SHOULDN'T BE BROUGHT BACK IN PERFECT CONDITION !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    public IEnumerator Sheat()
    {
        Debug.Log("Sheat coroutine has started");
        float duration = 3f;
        float time = 0f;
        while (time < duration)
        {
            Debug.Log("Sheat coroutine is running");
            if (CanSheatWeapon())
            {
                Debug.Log("Sheat is triggered");
                animator.SetTrigger(AnimatorController.Instance.Sheat);
                break;
            }
            time += 0.01f;
            yield return waitTime;
        }
    }

    public IEnumerator Draw()
    {
        Debug.Log("Draw coroutine has started");
        float duration = 3f;
        float time = 0f;
        while (time < duration)
        {
            Debug.Log("Draw coroutine is running");
            if (CanDrawWeapon())
            {
                animator.ResetTrigger(AnimatorController.Instance.InwardSlash);
                animator.ResetTrigger(AnimatorController.Instance.OutwardSlash);
                animator.SetTrigger(AnimatorController.Instance.Draw);
                animator.SetLayerWeight(1, 1f);
                animator.SetLayerWeight(2, 1f);

                Debug.Log("Draw is triggered");
                break;
            }
            time += 0.01f;
            yield return waitTime;
        }
    }

    public bool CanDrawWeapon()
    {
        if (animator.GetLayerWeight(1) == 1f)
        {
            return false;
        }
        else
        {
            AnimatorStateInfo info1 = animator.GetCurrentAnimatorStateInfo(1);// state info of the combat layer
            AnimatorStateInfo info2 = animator.GetCurrentAnimatorStateInfo(2);// state info of the combat-arms layer
            if (info1.IsName("Idle") && info2.IsName("Idle"))
            {
                Debug.Log("draw will be allowed");
                //Debug.Break();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool CanSheatWeapon()
    {
        if (animator.GetLayerWeight(1) == 0f)
        {
            return false;
        }
        else
        {
            AnimatorStateInfo info1 = animator.GetCurrentAnimatorStateInfo(1);// state info of the combat layer
            AnimatorStateInfo info2 = animator.GetCurrentAnimatorStateInfo(2);// state info of the combat-arms layer
            if (info1.IsName("CombatLocomotionBlendTree") && info2.IsName("Combat"))
            {
                Debug.Log("sheat will be allowed");
                //Debug.Break();
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool IsHoldingMelee()
    {
        if (recentMeleeWeapon.transform.parent == MeleeHandTransform)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void HandleShield()
    {
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool(AnimatorController.Instance.Shield, true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool(AnimatorController.Instance.Shield, false);
        }
    }

    #region Animation Events
    public void GoInToMovementLayer()
    {
        //animator.ResetTrigger("Sheat");//improper fix: 
        animator.SetLayerWeight(1, 0f);
        animator.SetLayerWeight(2, 0f);
        animator.SetLayerWeight(0, 1f);
    }

    public void MeleeStartDealingDamage()//animation event
    {
        if (recentMeleeWeapon == null) return;
        recentMeleeWeapon.GetComponent<MeleeWeaponDamageController>().StartDealingDamage();
    }

    public void MeleeEndDealingDamage()//animation event
    {
        if (recentMeleeWeapon == null) return;
        recentMeleeWeapon.GetComponent<MeleeWeaponDamageController>().EndDealingDamage();
    }

    public void DrawMeleeWeapon()//animation event
    {
        if (recentMeleeWeapon == null) return;
        recentMeleeWeapon.transform.SetParent(MeleeHandTransform, false);
    }

    public void SheatMeleeWeapon()//animation event
    {
        if (recentMeleeWeapon == null) return;
        recentMeleeWeapon.transform.SetParent(MeleeSheatTransform, false);
    }

    public void ActivateShield()
    {
        if (shield == null) return;
        shield.SetActive(true);
    }

    public void DeactivateShield()
    {
        if (shield == null) return;
        shield.SetActive(false);
    }
    #endregion
}
