using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : InventoryItem
{
    [SerializeField] protected float damage;
    [SerializeField] protected float launchForce;

    [SerializeField] private bool canDealDamage;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private float effectiveDamage;

    private Animator animator;
    [SerializeField] private int targetLayer;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        
    }

    protected override void Update()
    {
        base.Update();
        //transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    public void Shoot(float force, Vector3 shootingDir)
    {
        gameObject.GetComponent<InventoryItem>().OnDropped();
        transform.rotation = Quaternion.LookRotation(shootingDir);

        Debug.Log("aim camera forward: " + shootingDir);
        Debug.Log("arrow forward: " + gameObject.transform.forward);

        canDealDamage = true;

        launchForce = force;
        effectiveDamage = damage * force;

        rb.isKinematic = false;

        rb.AddForce(shootingDir * launchForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("arrow collided to "+ collider.gameObject.name);
        if (canDealDamage)
        {
            if (collider.gameObject.layer == targetLayer)//if hit to enemy
            {
                if (!collider.isTrigger) return;//temporary solution for the bug caused by the player capsule collider
                
                //GameObject enemy = target.gameObject.GetComponent<BodyPart>().player;
                DealDamage(collider, effectiveDamage);
                StickIntoEnemy();
            }
            else if (collider.gameObject.layer == 11)//attack will get parried with shield
            {
                //shouldn't get parried by it's own sword and shield, cover that case
                GetDeflected();
            }
            else if (collider.gameObject.layer == 12)
            {
                Debug.Log("arrow will be deflected by map surface");
                GetDeflected();
            }
        }
    }

    public override void OnEquipped()
    {
        animator = owner.GetComponent<Animator>();
        GetTargetLayer();
    }

    public override void OnDropped()
    {
        base.OnDropped();
        canDealDamage = false;
        animator = null;
    }

    private void GetTargetLayer()
    {
        if (owner.layer == 8) targetLayer = 9;
        else if (owner.layer == 9) targetLayer = 8;
    }

    private void DealDamage(Collider target, float damageDealt) 
    {
        target.gameObject.GetComponent<BodyPart>().TakeDamage(damageDealt);
        //enemy.GetComponent<Animator>().SetTrigger("TakeHit");//play take hit anim
        //successful hit, level up player skill
    }

    private void StickIntoEnemy()
    {
        canDealDamage = false;
        rb.isKinematic = true;
        boxCollider.enabled = false;

        Destroy(gameObject, 2f);//stay on the enmy for a moment
    }

    private void GetDeflected()
    {
        canDealDamage = false;
        boxCollider.isTrigger = false;
        rb.isKinematic = false;
    }
}
