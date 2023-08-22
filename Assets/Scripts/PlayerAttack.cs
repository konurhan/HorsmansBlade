using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SocialPlatforms.Impl;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] public GameObject meleeWeaponOneHanded;
    [SerializeField] public GameObject meleeWeaponTwoHanded;
    [SerializeField] public GameObject rangedWeapon;
    [SerializeField] public GameObject shield;

    public Transform MeleeHandTransform;
    public Transform MeleeSheatTransform;
    public Transform RangedHandTransform;
    public Transform RangedSheatTransform;
    public Transform ShieldHandTransform;

    private Animator animator;

    private bool usingOneHanded;
    private bool usingTwoHanded;
    private bool usingRanged;

    #region AI signaling flags
    public bool inwardSlash;
    public bool outwardSlash;
    public bool downwardSlash;
    public bool shooting;
    #endregion

    private GameObject recentMeleeWeapon;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        //instantiate weapons on player controller or in inventory controller
        /*meleeWeapon = Instantiate(Resources.Load("Prefabs/LongSword"), MeleeSheatTransform) as GameObject;
        meleeWeapon.transform.GetChild(0).GetComponent<MeleeWeaponDamageController>().SetOwnerReference(gameObject);

        shield = Instantiate(Resources.Load("Prefabs/Shield"), ShieldHandTransform) as GameObject;
        shield.SetActive(false);*/

        inwardSlash = false;
        outwardSlash = false;
        downwardSlash = false;
        shooting = false;

        usingOneHanded = false;
        usingTwoHanded = false;
        usingRanged = false;
    }
    void Start()
    {

    }

    void Update()
    {
        HandleCombatActions();
        //UpdateRecentMeleeWeapon();
    }

    private void UpdateRecentMeleeWeapon()
    {
        if (usingOneHanded) recentMeleeWeapon = meleeWeaponOneHanded;
        else if (usingTwoHanded) recentMeleeWeapon = meleeWeaponTwoHanded;
    }

    public void HandleCombatActions()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) && CanDrawWeaponOneHanded())
        {
            usingOneHanded = true;
            recentMeleeWeapon = meleeWeaponOneHanded;
            animator.ResetTrigger("InwardSlash");
            animator.ResetTrigger("OutwardSlash");
            animator.SetTrigger("Draw");
            animator.SetLayerWeight(1, 1f);
            animator.SetLayerWeight(2, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && CanSheatWeaponOneHanded())
        {
            usingOneHanded = false;
            animator.SetTrigger("Sheat");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && CanDrawWeaponTwoHanded())
        {
            usingTwoHanded = true;
            recentMeleeWeapon = meleeWeaponTwoHanded;
            animator.ResetTrigger("InwardSlash");
            animator.ResetTrigger("OutwardSlash");
            animator.SetTrigger("Draw");
            animator.SetLayerWeight(3, 1f);
            animator.SetLayerWeight(4, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && CanSheatWeaponTwoHanded())
        {
            usingTwoHanded = false;
            animator.SetTrigger("Sheat");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && CanDrawWeaponRanged())
        {
            usingRanged = true;
            //animator.ResetTrigger("InwardSlash");
            //animator.ResetTrigger("OutwardSlash");
            animator.SetTrigger("Draw");
            animator.SetLayerWeight(5, 1f);
            animator.SetLayerWeight(6, 1f);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && CanSheatWeaponRanged())
        {
            usingRanged = false;
            animator.SetTrigger("Sheat");
        }
        HandleAttack();
        HandleShield();
    }

    public void HandleAttack()
    {
        
        if(usingOneHanded ||  usingTwoHanded)//if melee
        {
            if (Input.GetMouseButtonDown(0))
            {
                FindDirectionAndHit();
            }
        }
        else if (usingRanged)//if ranged, implement ranged weapon aim look around and camera controls
        {
            RangedWeapon rw = rangedWeapon.GetComponent<RangedWeapon>();
            if (!rw.hasNockedAmmo) rw.Reload();
            if (!rw.hasNockedAmmo) return;
            if (Input.GetMouseButtonDown(1) && !rw.hasDrawn)
            {
                rw.Aim();
            }
            if (Input.GetMouseButtonUp(1) && rw.hasDrawn)
            {
                rw.UnAim();
            }
            if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0) && rw.hasDrawn)
            {
                rw.Loose();
            }
        }
        

    }

    public void FindDirectionAndHit()
    {
        if (Input.GetAxis("Mouse X") < -0.15f && Input.GetAxis("Mouse Y") < 0.15f && Input.GetAxis("Mouse Y") > -0.15f)//coming from right
        {
            animator.SetTrigger("InwardSlash");
        }
        else if (Input.GetAxis("Mouse X") > 0.15f && Input.GetAxis("Mouse Y") < 0.15f && Input.GetAxis("Mouse Y") > -0.15f)//coming from left
        {
            animator.SetTrigger("OutwardSlash");
        }
        else if (Input.GetAxis("Mouse Y") < -0.15f && Input.GetAxis("Mouse X") < 0.15f && Input.GetAxis("Mouse X") > -0.15f)//coming down from above
        {
            animator.SetTrigger("DownwardSlash");
        }
    }

    public bool CanDrawWeaponOneHanded()//layers 1,2
    {
        if (usingOneHanded || usingTwoHanded || usingRanged || meleeWeaponOneHanded == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CanSheatWeaponOneHanded()
    {
        if (usingOneHanded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanDrawWeaponTwoHanded()//layers 3,4
    {
        if (usingOneHanded || usingTwoHanded || usingRanged || meleeWeaponTwoHanded == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CanSheatWeaponTwoHanded()
    {
        if (usingTwoHanded)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanDrawWeaponRanged()//layers 5,6
    {
        if (usingOneHanded || usingTwoHanded || usingRanged || rangedWeapon == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public bool CanSheatWeaponRanged()
    {
        if (usingRanged)
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
        if (usingTwoHanded || usingRanged) return;
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
    public void MeleeStartDealingDamage()//call in both one handed and two handed layers
    {
        recentMeleeWeapon.transform.GetChild(0).GetComponent<MeleeWeaponDamageController>().StartDealingDamage();
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("OutwardSlash")) outwardSlash = true;
        else if (animator.GetCurrentAnimatorStateInfo(1).IsName("InwardSlash")) inwardSlash = true;
        else if (animator.GetCurrentAnimatorStateInfo(1).IsName("DownwardSlash")) downwardSlash = true;
    }

    public void MeleeEndDealingDamage()//call in both one handed and two handed layers
    {
        recentMeleeWeapon.transform.GetChild(0).GetComponent<MeleeWeaponDamageController>().EndDealingDamage();
        inwardSlash = false;
        outwardSlash = false;
        downwardSlash = false;
    }

    public void DrawMeleeWeapon()//call in both one handed and two handed layers
    {
        recentMeleeWeapon.transform.SetParent(MeleeHandTransform, false);
    }

    public void SheatMeleeWeapon()//call in both one handed and two handed layers
    {
        recentMeleeWeapon.transform.SetParent(MeleeSheatTransform, false);
    }

    public void DrawRangedWeapon()//call in ranged layers
    {
        recentMeleeWeapon.transform.SetParent(RangedHandTransform, false);
    }

    public void SheatRangedWeapon()//call in ranged layers
    {
        recentMeleeWeapon.transform.SetParent(RangedSheatTransform, false);
    }

    public void ActivateShield()//only call in one handed layer
    {
        shield.SetActive(true);
    }

    public void DeactivateShield()//only call in one handed layer
    {
        shield.SetActive(false);
    }

    public void GoInToMovementLayer()//call at the 10th second of sheat2
    {
        animator.ResetTrigger("Sheat");//improper fix: 
        
        animator.SetLayerWeight(1, 0f);
        animator.SetLayerWeight(2, 0f);
        animator.SetLayerWeight(3, 0f);
        animator.SetLayerWeight(4, 0f);
        animator.SetLayerWeight(5, 0f);
        animator.SetLayerWeight(6, 0f);
        
        animator.SetLayerWeight(0, 1f);
    }
    #endregion
}
