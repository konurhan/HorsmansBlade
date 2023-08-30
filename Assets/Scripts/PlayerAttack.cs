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

    public Transform ArrowSlot;
    public Transform MeleeHandTransform;
    public Transform MeleeSheatTransform;
    public Transform RangedHandTransform;
    public Transform RangedSheatTransform;
    public Transform RangedQuiverTransform;
    public Transform ShieldHandTransform;

    public Transform spine;
    public Transform spineLookAt;
    public Transform Crosshair;

    private bool usingOneHanded;
    private bool usingTwoHanded;
    private bool usingRanged;

    #region AI signaling flags
    public bool inwardSlash;
    public bool outwardSlash;
    public bool downwardSlash;
    public bool aiming;
    public bool shooting;
    #endregion

    private GameObject recentMeleeWeapon;
    private GameObject recentRangedWeapon;

    private Animator animator;
    private CameraController cameraController;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        cameraController = GetComponent<CameraController>();

        inwardSlash = false;
        outwardSlash = false;
        downwardSlash = false;
        aiming = false;
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

    private void OnAnimatorIK(int layerIndex)
    {
        if (!aiming)
        {
            animator.SetLookAtWeight(0f);
            return;
        }
        
        animator.SetLookAtWeight(1,1,1,1);
        animator.SetLookAtPosition(spineLookAt.position);
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
        else if (Input.GetKeyDown(KeyCode.Alpha1) && CanSheatWeaponOneHanded())
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
        else if (Input.GetKeyDown(KeyCode.Alpha2) && CanSheatWeaponTwoHanded())
        {
            usingTwoHanded = false;
            animator.SetTrigger("Sheat");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && CanDrawWeaponRanged())
        {
            usingRanged = true;
            recentRangedWeapon = rangedWeapon;
            animator.SetTrigger("EquipBow");
            animator.SetLayerWeight(5, 1f);
            animator.SetLayerWeight(6, 1f);

            LimitSpeedWhileAiming();
            Crosshair.gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && CanSheatWeaponRanged())
        {
            usingRanged = false;
            animator.SetTrigger("DisarmBow");

            RemoveSpeedLimit();
            Crosshair.gameObject.SetActive(false);
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
            //return;//debug later
            RangedWeapon rw = rangedWeapon.GetComponent<RangedWeapon>();
            if (!rw.hasNockedAmmo) rw.Reload();//make active later
            if (!rw.hasNockedAmmo) return;
            if (Input.GetMouseButtonDown(1) && !rw.hasDrawn)
            {
                //HandleSpineLookAt();
                aiming = true;
                rw.Aim();
            }
            if (Input.GetMouseButtonUp(1) && rw.hasDrawn)
            {
                aiming = false;
                rw.UnAim();
            }
            if (Input.GetMouseButton(1) && Input.GetMouseButtonDown(0) && rw.hasDrawn)
            {
                aiming = false;
                rw.Loose();
            }
        }
        

    }

    public void HandleSpineLookAt()
    {
        GameObject aimCam = cameraController.aimCamera.gameObject;

        RaycastHit hit;
        if (Physics.Raycast(aimCam.transform.position,aimCam.transform.forward, out hit))
        {
            spineLookAt.position = hit.point;
            Debug.Log(hit.transform.name+" is on crosshair");
        }
        else
        {
            spineLookAt.position = gameObject.transform.position + aimCam.transform.forward*5;
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

    public void LimitSpeedWhileAiming()
    {
        gameObject.GetComponent<PlayerMovement>().SpeedZUpper = 1f;
        gameObject.GetComponent<PlayerMovement>().SpeedZLower = -1f;
    }

    public void RemoveSpeedLimit()
    {
        gameObject.GetComponent<PlayerMovement>().SpeedZUpper = 2f;
        gameObject.GetComponent<PlayerMovement>().SpeedZLower = -2f;
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
        recentRangedWeapon.transform.SetParent(RangedHandTransform, false);
    }

    public void SheatRangedWeapon()//call in ranged layers
    {
        recentRangedWeapon.transform.SetParent(RangedSheatTransform, false);
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

    public void InstantiateArrowInHand()//call when hand reaches the quiver
    {
        Ammo arrow = recentRangedWeapon.GetComponent<Bow>().knockedAmmo;

        if (animator.GetCurrentAnimatorStateInfo(6).IsName("DrawArrow"))
        {
            arrow.gameObject.SetActive(true);
            arrow.gameObject.transform.SetParent(ArrowSlot, false);
            arrow.gameObject.transform.localPosition = Vector3.zero;
        }
        else if (animator.GetCurrentAnimatorStateInfo(6).IsName("AbortDraw"))
        {
            arrow.gameObject.SetActive(false);
        }


    }

    public void ReturnArrowToQuiver()//not used
    {
        Ammo arrow = recentRangedWeapon.GetComponent<Bow>().knockedAmmo;
        arrow.gameObject.SetActive(false);
    }

    public void ShootArrowLoose()
    {

    }

    #endregion
}
