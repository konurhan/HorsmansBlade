using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.SocialPlatforms.Impl;
using Unity.Burst.CompilerServices;

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

    public bool usingOneHanded;
    public bool usingTwoHanded;
    public bool usingRanged;

    #region AI signaling flags
    public bool inwardSlash;
    public bool outwardSlash;
    public bool downwardSlash;
    public bool aiming;
    public bool shooting;
    #endregion

    private GameObject recentMeleeWeapon;
    private GameObject recentRangedWeapon;
    public GameObject recentOnCrossObject;
    public Vector3 recentOnCrossObjectContactPoint;

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
        
        /*AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(1);
        if (info.IsName("CombatLocomotionBlendTree"))
        {
            Debug.Log("CombatLocomotionBlendTree is the current state");
        }
        else if (info.IsName("Idle"))
        {
            Debug.Log("Idle is the current state");
        }*/
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
            animator.SetBool("usingWeapon", true);
            animator.ResetTrigger("InwardSlash");
            animator.ResetTrigger("OutwardSlash");
            animator.SetTrigger("Draw");
            animator.SetLayerWeight(1, 1f);
            animator.SetLayerWeight(2, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1) && CanSheatWeaponOneHanded())
        {
            usingOneHanded = false;
            animator.SetBool("usingWeapon", false);
            animator.SetTrigger("Sheat");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2) && CanDrawWeaponTwoHanded())
        {
            usingTwoHanded = true;
            recentMeleeWeapon = meleeWeaponTwoHanded;
            animator.SetBool("usingWeapon", true);
            animator.ResetTrigger("InwardSlash");
            animator.ResetTrigger("OutwardSlash");
            animator.SetTrigger("Draw");
            animator.SetLayerWeight(3, 1f);
            animator.SetLayerWeight(4, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && CanSheatWeaponTwoHanded())
        {
            usingTwoHanded = false;
            animator.SetBool("usingWeapon", false);
            animator.SetTrigger("Sheat");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3) && CanDrawWeaponRanged())
        {
            usingRanged = true;
            recentRangedWeapon = rangedWeapon;
            animator.SetBool("usingWeapon", true);
            animator.SetTrigger("EquipBow");
            animator.SetLayerWeight(5, 1f);
            animator.SetLayerWeight(6, 1f);

            LimitSpeedWhileAiming();
            Crosshair.gameObject.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && CanSheatWeaponRanged())
        {
            usingRanged = false;
            animator.SetBool("usingWeapon", false);
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
            if (!rw.hasNockedAmmo) rw.Reload();
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
        GameObject mainCam = cameraController.mainVCamera.gameObject;

        spineLookAt.position = aimCam.transform.position + cameraController.target.transform.forward * 100;

        RaycastHit hit;
        int layerMask = ~(1 << 15 + 1 << 14);
        if (Physics.Raycast(aimCam.transform.position, aimCam.transform.forward, out hit, 100, layerMask))
        {
            recentOnCrossObject = hit.collider.gameObject;
            recentOnCrossObjectContactPoint = hit.point;
        }
        else
        {
            recentOnCrossObject = null;
        }

        Debug.Log("Recent on cross object is "+ recentOnCrossObject);
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
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(1);// state info of the combat layer
            if (info.IsName("Idle"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public bool CanSheatWeaponOneHanded()//cannot sheat while swinging
    {
        if (usingOneHanded)
        {
            AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(1);// state info of the combat layer
            if (info.IsName("CombatLocomotionBlendTree"))
            {
                return true;
            }
            else
            {
                return false;
            }
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
        recentMeleeWeapon.GetComponent<MeleeWeaponDamageController>().StartDealingDamage();
        if (animator.GetCurrentAnimatorStateInfo(1).IsName("OutwardSlash")) outwardSlash = true;
        else if (animator.GetCurrentAnimatorStateInfo(1).IsName("InwardSlash")) inwardSlash = true;
        else if (animator.GetCurrentAnimatorStateInfo(1).IsName("DownwardSlash")) downwardSlash = true;
    }

    public void MeleeEndDealingDamage()//call in both one handed and two handed layers
    {
        recentMeleeWeapon.GetComponent<MeleeWeaponDamageController>().EndDealingDamage();
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
        if (recentMeleeWeapon == null) return;
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
        if (shield == null) return;
        shield.SetActive(true);
    }

    public void DeactivateShield()//only call in one handed layer
    {
        if (shield == null) return;
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
