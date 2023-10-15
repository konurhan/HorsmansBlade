using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : InventoryItem
{
    [SerializeField] protected float damage;//maximum damage of an ammo out of 100
    [SerializeField] protected float launchForce;

    [SerializeField] private bool canDealDamage;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private BoxCollider boxCollider;

    [SerializeField] private float effectiveDamage;

    private Animator animator;
    [SerializeField] private int targetLayer;

    [SerializeField] private GameObject lastOwner;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
    }

    void Start()
    {
        
    }

    private void FixedUpdate()
    {
        if (canDealDamage)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);//making arrow head follow the projectile motion of the arrow
        }
    }

    protected override void Update()
    {
        base.Update();
        /*if (canDealDamage)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);//making arrow head follow the projectile motion of the arrow
        }*/
    }

    public void Shoot(float bowMaxForce , float skillMultiplier, Vector3 shootingDir)//skillMultiplier: float b/w [0,1] determined by the skill level of attacker and the maximum force of the ranged weapon
    {
        gameObject.GetComponent<InventoryItem>().OnDropped();
        transform.rotation = Quaternion.LookRotation(shootingDir);

        Debug.Log("aim camera forward: " + shootingDir);
        Debug.Log("arrow forward: " + gameObject.transform.forward);

        canDealDamage = true;

        launchForce = bowMaxForce * skillMultiplier;//multiply this with maximum force of the bow
        effectiveDamage = damage * skillMultiplier * bowMaxForce/100;//best archer can deal the full damage while using the best bow -> assuming that the maximum possible force of a ranged weapon can be 100

        rb.isKinematic = false;

        float ammoFineTune = 0.01f;

        rb.AddForce(shootingDir * launchForce * ammoFineTune, ForceMode.Impulse);

        /*Time.timeScale = 0.1f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;*/

        
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (canDealDamage)
        {
            if (collider.gameObject.layer == targetLayer)//if hit to enemy
            {
                //Debug.LogError("stop");
                Debug.Log("arrow collided to " + collider.gameObject.name);
                if (!collider.gameObject.GetComponent<BodyPart>()) return;
                BodyPart part = collider.gameObject.GetComponent<BodyPart>();
                DealDamage(part, effectiveDamage);
                //Debug.Break();
                StickIntoEnemy(part);
            }
            else if (collider.gameObject.layer == 11 || collider.gameObject.layer == 10)//attack will get parried with shield
            {
                //shouldn't get parried by it's own sword and shield, cover that case
                if (collider.gameObject.GetComponent<InventoryItem>().owner == lastOwner) return;
                GetDeflected();
                GameObject sparkParticle = Instantiate(Resources.Load("Prefabs/Particles/Spark"), collider.gameObject.transform) as GameObject;
                sparkParticle.transform.localPosition = collider.gameObject.GetComponentInChildren<BoxCollider>().center;
            }
            else if (collider.gameObject.layer == 12)
            {
                StickIntoMapSurface();
            }
        }
    }

    public override void OnEquipped()
    {
        animator = owner.GetComponent<Animator>();
        GetTargetLayer();
        lastOwner = owner;
    }

    public override void OnDropped()
    {
        base.OnDropped();
        canDealDamage = false;
        animator = null;
    }

    private void GetTargetLayer()
    {
        if (owner.layer == 8) targetLayer = 15;
        else if (owner.layer == 9) targetLayer = 14;
    }

    private void DealDamage(BodyPart part, float damageDealt) 
    {
        if (lastOwner.GetComponent<PlayerController>() != null)
        {
            lastOwner.GetComponent<PlayerController>().GainRangedXP();
        }

        part.TakeDamage(damageDealt);
        part.player.GetComponent<Animator>().SetTrigger("TakeHit");//play take hit anim

        GameObject particle = Instantiate(Resources.Load("Prefabs/Particles/BloodSprayFX"), part.gameObject.transform) as GameObject;
        Destroy(particle, 0.5f);
    }

    private void StickIntoEnemy(BodyPart part)
    {
        //Debug.Break();
        canDealDamage = false;
        rb.isKinematic = true;
        boxCollider.enabled = false;

        //stay on the enmy only as a mesh
        GameObject arrowMesh = Instantiate(Resources.Load("Meshes/Weapon/Ranged/ArrowLongBowMesh")) as GameObject;
        arrowMesh.transform.SetParent(part.gameObject.transform);
        arrowMesh.transform.position = gameObject.transform.position;
        arrowMesh.transform.rotation = Quaternion.LookRotation(gameObject.transform.forward);

        //Debug.Break();
        Destroy(gameObject);
    }

    private void StickIntoMapSurface()
    {
        canDealDamage = false;
        rb.isKinematic = true;
        boxCollider.enabled = false;
    }

    private void GetDeflected()
    {
        canDealDamage = false;
        boxCollider.isTrigger = false;
        rb.isKinematic = false;
    }

    
}
