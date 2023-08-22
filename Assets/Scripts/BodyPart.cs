using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    

    public string bodyPartName;//provide in the editor
    public int damageMultiplier;//provide in the editor
    public GameObject armour;

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
        if (armour != null) //first, the damage has to pass through the armour
        {
            float remainingDamage = armour.GetComponent<Armour>().TakeDamage(effectiveDamage);
            effectiveDamage = remainingDamage;
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

    public void PutOnArmour()
    {

    }

    public void PutOnCurrentArmour()//call this in inventory code
    {
        //string name = get current armor name from save file

        //Instantiate(Resources.Load("Prefabs/Armour/"+name), transform);
    }
}
