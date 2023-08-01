using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EquipmentSystem : MonoBehaviour
{
    [SerializeField] private GameObject sword;
    public Transform SwordHandTransform;
    public Transform SwordSheatTransform;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        sword = Instantiate(Resources.Load("Prefabs/LongSword"), SwordSheatTransform) as GameObject;
        sword.transform.GetChild(0).GetComponent<WeaponDamageController>().SetOwnerReference(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
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
    }

    public void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("InwardSlash");
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("OutwardSlash");
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
    #endregion
}
