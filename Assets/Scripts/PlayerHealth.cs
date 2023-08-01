using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float headHealth;
    public float torsoHealth;
    public float armsHealth;
    public float legsHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(string bodyPart, float damage)
    {
        switch (bodyPart)
        {
            case "head":
                headHealth -= damage;
                break;
            case "torso":
                torsoHealth -= damage;
                break;
            case "arm":
                armsHealth -= damage;
                break;
            case "leg":
                legsHealth -= damage;
                break;
        }
        if (headHealth < 0)
        {
            headHealth = 0;
            Die();
            return;
        }
        else if (torsoHealth < 0)
        {
            torsoHealth = 0;
            Die();
            return;
        }
        else if (armsHealth < 0)
        {
            armsHealth = 0;
            Die();
            return;
        }
        else if (legsHealth < 0)
        {
            legsHealth = 0;
            Die();
            return;
        }
    }

    private void Die()
    {

    }

    public void LoadStats()
    {
        //should work with savesystem script, fetch the values and assign them to body part health values. 
    }
}
