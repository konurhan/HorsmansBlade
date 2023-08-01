using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamageController : MonoBehaviour
{
    [SerializeField] private bool canDealDamage;
    [SerializeField] private List<GameObject> cutThroughObjects;
    public float damage;//provide in the editor
    private Rigidbody rBody;

    [SerializeField] private GameObject weaponOwner;
    private Animator animator;
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
    }

    void Update()
    {

    }

    private void OnTriggerEnter(Collider target)
    {
        if (canDealDamage)
        {
            Debug.Log("collided object layer is: " + target.gameObject.layer);
            //Debug.Log("enemy layer: " + LayerMask.GetMask("Enemy"));
            if(target.gameObject.layer == 9)
            {
                if (!target.isTrigger) return;//temporary solution for the bug caused by the player capsule collider
                //also check if collided object is of weapon or shield layer,
                //if so attack anim should be stopped and parry animation should be triggered
                //and EndDealingDamage should be called immediately
                if (!cutThroughObjects.Contains(target.gameObject)) 
                { 
                    cutThroughObjects.Add(target.gameObject);
                    Debug.Log("sword hit to object: " + target.gameObject.name);
                }
            }
            else if(target.gameObject.layer == 10 || target.gameObject.layer == 11)//attack will get parried
            {
                EndDealingDamage();
                float currentFrame = animator.GetCurrentAnimatorStateInfo(1).normalizedTime;//combat layer index is 1
                //animator.SetTrigger("GetParried");//no need for trigger, directly play the anim from desired frame
                if (animator.GetCurrentAnimatorStateInfo(1).IsName("InwardSlash"))
                {
                    animator.Play("InwardSlashGetParried",1,currentFrame);
                    Debug.Log("Inward slash got parried on frametime: " + currentFrame);
                }else if (animator.GetCurrentAnimatorStateInfo(1).IsName("OutwardSlash"))
                {
                    Debug.Log("Outward slash got parried on frametime: " + currentFrame);
                    animator.Play("OutwardSlashGetParried", 1, currentFrame);
                }
            }
        }
    }

    public void StartDealingDamage()
    {
        rBody.collisionDetectionMode = CollisionDetectionMode.Continuous;//for performance
        canDealDamage = true;
        cutThroughObjects.Clear();
    }

    public void EndDealingDamage()
    {
        rBody.collisionDetectionMode = CollisionDetectionMode.Discrete;//for performance
        canDealDamage = false;
        DealDamage();
        cutThroughObjects.Clear();
    }

    public void DealDamage()
    {
        foreach (GameObject obj in cutThroughObjects)
        {
            Debug.Log(obj.name);
            obj.GetComponent<BodyPart>().TakeDamage(damage);
        }
    }

    public void SetOwnerReference(GameObject owner)
    {
        weaponOwner = owner;
    }
}
