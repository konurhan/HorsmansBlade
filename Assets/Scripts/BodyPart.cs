using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    private PlayerHealth health;

    public string bodyPartName;//provide in the editor
    public int damageMultiplier;//provide in the editor
    public GameObject armour;

    private void Awake()
    {
        health = transform.parent.gameObject.GetComponent<PlayerHealth>();//getting the script reference from the Player object
    }

    void Start()
    {
        PutOnCurrentArmour();
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

    public void SetPlayerReference(GameObject game)
    {

    }

    public void PutOnCurrentArmour()
    {
        //string name = get current armor name from save file

        //Instantiate(Resources.Load("Prefabs/Armour/"+name), transform);
    }
}
