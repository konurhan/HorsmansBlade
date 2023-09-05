using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class EnemyNPCEquipmentSystem : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shield;
    public Transform SwordHandTransform;
    public Transform SwordSheatTransform;
    public Transform ShieldHandTransform;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        sword = Instantiate(Resources.Load("Prefabs/Weapons/Melee/SimpleLongSword"), SwordSheatTransform) as GameObject;
        sword.GetComponent<InventoryItem>().SetOwnerReference(gameObject);

        shield = Instantiate(Resources.Load("Prefabs/Shields/SimpleShield"), ShieldHandTransform) as GameObject;
        shield.GetComponent<InventoryItem>().SetOwnerReference(gameObject);
        shield.SetActive(false);
    }
    void Start()
    {
        
    }

    void Update()
    {
        //HandleCombatActions();
    }

    public void Sheat()
    {
        if (CanSheatWeapon())
        {
            animator.SetTrigger("Sheat");
            //animator.SetBool("Shield", false);
        }
    }

    public void Draw()
    {
        if (CanDrawWeapon())
        {
            animator.ResetTrigger("InwardSlash");
            animator.ResetTrigger("OutwardSlash");
            animator.SetTrigger("Draw");
            //animator.SetBool("Shield", true);
            animator.SetLayerWeight(1, 1f);
            animator.SetLayerWeight(2, 1f);
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
            return true;
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
            return true;
        }
    }

    public void HandleShield()
    {
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetBool("Shield", true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("Shield", false);
        }
    }

    #region Animation Events
    public void MeleeStartDealingDamage()//animation event
    {
        sword.GetComponent<MeleeWeaponDamageController>().StartDealingDamage();
    }

    public void MeleeEndDealingDamage()//animation event
    {
        sword.GetComponent<MeleeWeaponDamageController>().EndDealingDamage();
    }

    public void DrawMeleeWeapon()//animation event
    {
        sword.transform.SetParent(SwordHandTransform, false);
    }

    public void SheatMeleeWeapon()//animation event
    {
        sword.transform.SetParent(SwordSheatTransform, false);
    }

    public void ActivateShield()
    {
        shield.SetActive(true);
    }

    public void DeactivateShield()
    {
        shield.SetActive(false);
    }
    #endregion
}
