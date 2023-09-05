using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float headHealth;
    public float torsoHealth;
    public float armsHealth;
    public float legsHealth;

    public bool isDead;
    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        LoadPlayerHealth();
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
        isDead = true;
        GetComponent<Animator>().SetBool("Died", true);
    }

    public void LoadPlayerHealth()
    {

    }
}
