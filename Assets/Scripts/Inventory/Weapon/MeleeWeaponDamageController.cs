using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class MeleeWeaponDamageController : MonoBehaviour
{
    [SerializeField] private bool canDealDamage;
    [SerializeField] private bool isOneHanded;

    [SerializeField] private List<GameObject> cutThroughObjects;
    [SerializeField] private List<GameObject> cutThroughEnemies;
    public float damage;//cache from MeleeWeapon component

    private Rigidbody rBody;
    private MeleeWeapon melee;

    //[SerializeField] private GameObject weaponOwner;
    private Animator animator;
    private int targetLayer;

    private void Awake()
    {
        cutThroughObjects = new List<GameObject>();
        cutThroughEnemies = new List<GameObject>();

        rBody = GetComponent<Rigidbody>();
        //rBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        melee = GetComponent<MeleeWeapon>();
        
        canDealDamage = false;
        isOneHanded = GetComponent<IOneHanded>() != null;
    }

    void Start()
    {
        /*animator = weaponOwner.GetComponent<Animator>();
        GetTargetLayer();

        weaponOwner.GetComponent<PlayerController>().onLevelUp += UpdateDamage;//damage will be updated each time the user levels up
        UpdateDamage();//sets the initial damage*/
    }

    public void OnEquippedMelee()
    {
        animator = melee.owner.GetComponent<Animator>();
        GetTargetLayer();

        if (melee.owner.GetComponent<PlayerController>() == null) return;

        melee.owner.GetComponent<PlayerController>().onLevelUp += UpdateDamage;//damage will be updated each time the user levels up
        UpdateDamage();//sets the initial damage
    }

    public void OnDroppedMelee()
    {
        canDealDamage = false;
        animator = null;
        if (melee.owner.GetComponent<PlayerController>() == null) return;
        melee.owner.GetComponent<PlayerController>().onLevelUp -= UpdateDamage;
    }

    public void UpdateDamage()
    {
        float meleeSkill;
        float strength;
        if (isOneHanded)
        {
            meleeSkill = melee.owner.GetComponent<PlayerController>().oneHandedSkillLevel;
        }
        else
        {
            meleeSkill = melee.owner.GetComponent<PlayerController>().twoHandedSkillLevel;
        }
        strength = melee.owner.GetComponent<PlayerController>().strength;

        damage = melee.MaxDamage * (meleeSkill/100) * (strength/100);
    }

    private void GetTargetLayer()
    {
        if (melee.owner.layer == 8) targetLayer = 15;
        else if (melee.owner.layer == 9) targetLayer = 14;
    }

    private void OnTriggerEnter(Collider target)
    {
        if (canDealDamage)
        {
            if(target.gameObject.layer == targetLayer)
            {
                if (!target.isTrigger) return;//temporary solution for the bug caused by the player capsule collider
                if (!cutThroughObjects.Contains(target.gameObject)) 
                { 
                    cutThroughObjects.Add(target.gameObject);
                }
                GameObject enemy = target.gameObject.GetComponent<BodyPart>().player;
                if (!cutThroughEnemies.Contains(enemy))
                {
                    cutThroughEnemies.Add(enemy);
                }
            }
            else if(target.gameObject.layer == 10 || target.gameObject.layer == 11)//attack will get parried with sword and shield
            {
                if (target.gameObject.layer == 11)
                {
                    //instantiate smoke particle
                }
                
                //shouldn't get parried by it's own sword and shield, cover that case
                float currentFrame = animator.GetCurrentAnimatorStateInfo(1).normalizedTime;//combat layer index is 1
                if (animator.GetCurrentAnimatorStateInfo(1).IsName("InwardSlash"))
                {
                    animator.Play("InwardSlashGetParried",1,1-currentFrame);
                }
                else if (animator.GetCurrentAnimatorStateInfo(1).IsName("OutwardSlash"))
                {
                    animator.Play("OutwardSlashGetParried", 1, 1-currentFrame);
                }
                else if (animator.GetCurrentAnimatorStateInfo(1).IsName("DownswardSlash"))
                {
                    animator.Play("DownwardSlashGetParried", 1, 1 - currentFrame);
                }
                EndDealingDamage();
            }

            if (target.gameObject.layer == 10 || target.gameObject.layer == 11 || target.gameObject.layer == 16)
            {
                GameObject sparkParticle = Instantiate(Resources.Load("Prefabs/Particles/Spark"), target.gameObject.transform) as GameObject;
                sparkParticle.transform.localPosition = target.gameObject.GetComponentInChildren<BoxCollider>().center;
            }
        }
    }

    public void StartDealingDamage()//called from animation event method
    {
        canDealDamage = true;
        cutThroughObjects.Clear();
    }

    public void EndDealingDamage()//called from animation event method
    {
        canDealDamage = false;
        DealDamage();
        cutThroughObjects.Clear();
        cutThroughEnemies.Clear();
        if (melee.owner.GetComponent<PlayerAttack>())
        {
            PlayerAttack attack = melee.owner.GetComponent<PlayerAttack>();
            attack.outwardSlash = false;
            attack.inwardSlash = false;
            attack.downwardSlash = false;
        }
    }

    public void DealDamage()
    {
        foreach (GameObject obj in cutThroughObjects)
        {
            //Debug.Log(obj.name);
            obj.GetComponent<BodyPart>().TakeDamage(damage);
            GameObject particle = Instantiate(Resources.Load("Prefabs/Particles/BloodSprayFX"), obj.transform) as GameObject;
            particle.transform.localPosition = obj.GetComponent<BoxCollider>().center;
            Destroy(particle,0.5f);
        }
        foreach (GameObject enemy in cutThroughEnemies)
        {
            if(melee.owner.GetComponent<PlayerController>())
            {
                if (isOneHanded)
                {
                    melee.owner.GetComponent<PlayerController>().GainOneHandedXP();
                }
                else
                {
                    melee.owner.GetComponent<PlayerController>().GainTwoHandedXP();
                }
            }
            enemy.GetComponent<Animator>().SetTrigger("TakeHit");
        }
    }
}