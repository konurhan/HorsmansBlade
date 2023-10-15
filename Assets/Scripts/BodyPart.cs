using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    [SerializeField] private string bodyPartName;//provide in the editor
    public int damageMultiplier;//provide in the editor
    public Armour armour;

    public GameObject player { get; private set;}//player reference: i.e. owner of the bodypart. assigned in the awake of Player script
    [SerializeField] private PlayerHealth health;

    private void Awake()
    {
        
    }

    void Start()
    {
        health = player.GetComponent<PlayerHealth>();//getting the script reference from the Player object
        //PutOnCurrentArmour();
    }

    void Update()
    {
        
    }

    public void TakeDamage(float incomingDamage) 
    {
        float effectiveDamage = incomingDamage * damageMultiplier;
        Debug.Log("Take damage is called for " + bodyPartName + ", effectiveDamage: " + effectiveDamage);
        if (armour != null) //first, the damage has to pass through the armour
        {
            float remainingDamage = armour.TakeDamage(effectiveDamage);
            effectiveDamage = remainingDamage;

            GameObject sparkParticle = Instantiate(Resources.Load("Prefabs/Particles/Spark"), gameObject.transform) as GameObject;
            sparkParticle.transform.localPosition = gameObject.GetComponentInChildren<BoxCollider>().center;
        }

        if(effectiveDamage > 0)//now the damage will affect the health
        {
            health.TakeDamage(bodyPartName, effectiveDamage);
        }
    }

    public void SetPlayerReference(GameObject owner)
    {
        player = owner;
    }

    public void PutOnArmour(Armour armour)
    {
        this.armour = armour;
    }

    public void RemoveArmour()
    {
        armour = null;
    }

    public void PutOnCurrentArmour()//call this in inventory code
    {
        //string name = get current armor name from save file

        //Instantiate(Resources.Load("Prefabs/Armour/"+name), transform);
    }
}
