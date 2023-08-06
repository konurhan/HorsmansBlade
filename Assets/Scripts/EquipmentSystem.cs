using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EquipmentSystem : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    [SerializeField] private GameObject shield;
    public Transform SwordHandTransform;
    public Transform SwordSheatTransform;
    public Transform ShieldHandTransform;

    private Animator animator;

    #region AI signaling flags
    public bool inwardSlash;
    public bool outwardSlash;
    public bool downwardSlash;
    #endregion

    private void Awake()
    {
        animator = GetComponent<Animator>();

        sword = Instantiate(Resources.Load("Prefabs/LongSword"), SwordSheatTransform) as GameObject;
        sword.transform.GetChild(0).GetComponent<WeaponDamageController>().SetOwnerReference(gameObject);

        shield = Instantiate(Resources.Load("Prefabs/Shield"), ShieldHandTransform) as GameObject;
        shield.SetActive(false);

        inwardSlash = false;
        outwardSlash = false;
        downwardSlash = false;
    }
    void Start()
    {
        
    }

    void Update()
    {
        HandleCombatActions();
    }

    public void HandleCombatActions()
    {
        if (Input.GetKeyDown(KeyCode.F) && CanDrawWeapon())
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
    }

    public void HandleAttack()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FindDirectionAndHit();
        }
        /*if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("InwardSlash");
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("OutwardSlash");
        }*/
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
        if(animator.GetCurrentAnimatorStateInfo(1).IsName("OutwardSlash")) outwardSlash = true;
        else if(animator.GetCurrentAnimatorStateInfo(1).IsName("InwardSlash")) inwardSlash = true;
        else if (animator.GetCurrentAnimatorStateInfo(1).IsName("DownwardSlash")) downwardSlash = true;
    }

    public void EndDealingDamage()//animation event, not called when swing is blocked
    {
        sword.transform.GetChild(0).GetComponent<WeaponDamageController>().EndDealingDamage();
        inwardSlash = false;
        outwardSlash = false;
        downwardSlash = false;
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
