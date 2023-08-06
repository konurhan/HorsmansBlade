using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponDamageController : MonoBehaviour
{
    [SerializeField] private bool canDealDamage;
    [SerializeField] private List<GameObject> cutThroughObjects;
    [SerializeField] private List<GameObject> cutThroughEnemies;
    public float damage;//provide in the editor
    private Rigidbody rBody;

    [SerializeField] private GameObject weaponOwner;
    private Animator animator;
    private int targetLayer;
    private void Awake()
    {
        canDealDamage = false;
        cutThroughObjects = new List<GameObject>();
        rBody = GetComponent<Rigidbody>();
        rBody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        
    }

    void Start()
    {
        animator = weaponOwner.GetComponent<Animator>();
        GetTargetLayer();
    }

    void Update()
    {

    }

    private void GetTargetLayer()
    {
        if (weaponOwner.layer == 8) targetLayer = 9;
        else if (weaponOwner.layer == 9) targetLayer = 8;
    }

    private void OnTriggerEnter(Collider target)
    {
        if (canDealDamage)
        {
            Debug.Log("collided object layer is: " + target.gameObject.layer);
            //Debug.Log("enemy layer: " + LayerMask.GetMask("Enemy"));
            if(target.gameObject.layer == targetLayer)
            {
                if (!target.isTrigger) return;//temporary solution for the bug caused by the player capsule collider
                if (!cutThroughObjects.Contains(target.gameObject)) 
                { 
                    cutThroughObjects.Add(target.gameObject);
                    Debug.Log("sword hit to object: " + target.gameObject.name);
                }
                GameObject enemy = target.gameObject.GetComponent<BodyPart>().player;
                if (!cutThroughEnemies.Contains(enemy))
                {
                    cutThroughEnemies.Add(enemy);
                    Debug.Log("cutting through the enemy object with name: " + enemy.name);
                }
            }
            else if(target.gameObject.layer == 10 || target.gameObject.layer == 11)//attack will get parried with sword and shield
            {
                //shouldn't get parried by it's own sword and shield, cover that case
                float currentFrame = animator.GetCurrentAnimatorStateInfo(1).normalizedTime;//combat layer index is 1
                if (animator.GetCurrentAnimatorStateInfo(1).IsName("InwardSlash"))
                {
                    animator.Play("InwardSlashGetParried",1,1-currentFrame);
                    Debug.Log("Inward slash got parried on frametime: " + currentFrame);
                }
                else if (animator.GetCurrentAnimatorStateInfo(1).IsName("OutwardSlash"))
                {
                    Debug.Log("Outward slash got parried on frametime: " + currentFrame);
                    animator.Play("OutwardSlashGetParried", 1, 1-currentFrame);
                }
                else if (animator.GetCurrentAnimatorStateInfo(1).IsName("DownswardSlash"))
                {
                    Debug.Log("Outward slash got parried on frametime: " + currentFrame);
                    animator.Play("DownwardSlashGetParried", 1, 1 - currentFrame);
                }
                EndDealingDamage();
            }
        }
    }

    public void StartDealingDamage()
    {
        //rBody.collisionDetectionMode = CollisionDetectionMode.Continuous;//for performance
        canDealDamage = true;
        cutThroughObjects.Clear();
    }

    public void EndDealingDamage()
    {
        //rBody.collisionDetectionMode = CollisionDetectionMode.Discrete;//for performance
        canDealDamage = false;
        DealDamage();
        cutThroughObjects.Clear();
        cutThroughEnemies.Clear();
        if (weaponOwner.GetComponent<EquipmentSystem>())
        {
            EquipmentSystem equipmentSystem = weaponOwner.GetComponent<EquipmentSystem>();
            equipmentSystem.outwardSlash = false;
            equipmentSystem.inwardSlash = false;
            equipmentSystem.downwardSlash = false;
        }
    }

    public void DealDamage()
    {
        foreach (GameObject obj in cutThroughObjects)
        {
            Debug.Log(obj.name);
            obj.GetComponent<BodyPart>().TakeDamage(damage);
        }
        foreach (GameObject enemy in cutThroughEnemies)
        {
            enemy.GetComponent<Animator>().SetTrigger("TakeHit");
            //enemy.GetComponent<Animator>().SetTrigger("TakeHit");
        }
    }

    public void SetOwnerReference(GameObject owner)
    {
        weaponOwner = owner;
    }
}
