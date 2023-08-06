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

        sword = Instantiate(Resources.Load("Prefabs/LongSword"), SwordSheatTransform) as GameObject;
        sword.transform.GetChild(0).GetComponent<WeaponDamageController>().SetOwnerReference(gameObject);

        shield = Instantiate(Resources.Load("Prefabs/Shield"), ShieldHandTransform) as GameObject;
        shield.SetActive(false);
    }
    void Start()
    {
        
    }

    void Update()
    {
        //HandleCombatActions();
    }

    public void HandleCombatActions()
    {
        /*if (Input.GetKeyDown(KeyCode.F) && CanDrawWeapon())
        {
            animator.ResetTrigger("InwardSlash");
            animator.ResetTrigger("OutwardSlash");
            animator.SetTrigger("Draw");
            animator.SetLayerWeight(1, 1f);
            animator.SetLayerWeight(2, 1f);
        }
        if (Input.GetKeyDown(KeyCode.G) && CanSheatWeapon())
        {
            animator.SetTrigger("Sheat");
        }
        HandleAttack();
        HandleShield();
        */
    }

    public void HandleAttack()
    {
        
    }

    public void IsReachedAttackTarget()
    {

    }

    public void FindDirectionAndHit()
    {
        if(Input.GetAxis("Mouse X") < -0.15f && Input.GetAxis("Mouse Y") < 0.15f && Input.GetAxis("Mouse Y") > -0.15f)//coming from right
        {
            animator.SetTrigger("InwardSlash");
        }
        else if(Input.GetAxis("Mouse X") > 0.15f && Input.GetAxis("Mouse Y") < 0.15f && Input.GetAxis("Mouse Y") > -0.15f)//coming from left
        {
            animator.SetTrigger("OutwardSlash");
        }
        else if (Input.GetAxis("Mouse Y") < -0.15f && Input.GetAxis("Mouse X") < 0.15f && Input.GetAxis("Mouse X") > -0.15f)//coming down from above
        {
            animator.SetTrigger("DownwardSlash");
        }
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
    public void StartDealingDamage()//animation event
    {
        sword.transform.GetChild(0).GetComponent<WeaponDamageController>().StartDealingDamage();
    }

    public void EndDealingDamage()//animation event
    {
        sword.transform.GetChild(0).GetComponent<WeaponDamageController>().EndDealingDamage();
    }

    public void DrawWeapon()//animation event
    {
        sword.transform.SetParent(SwordHandTransform, false);
    }

    public void SheatWeapon()//animation event
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