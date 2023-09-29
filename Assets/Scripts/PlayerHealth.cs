using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class PlayerHealth : MonoBehaviour
{
    public float headHealth;
    public float torsoHealth;
    public float armsHealth;
    public float legsHealth;

    public bool isDead;

    PlayerController controller;
    EnemyController enemyController;

    // Start is called before the first frame update
    void Start()
    {
        isDead = false;
        LoadPlayerHealth();

        InventoryUI.Instance.onGameSaved += SaveHealth;

        if (GetComponent<PlayerController>() != null ) controller = GetComponent<PlayerController>();
        if (GetComponent<EnemyController>() != null) enemyController = GetComponent<EnemyController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(string bodyPart, float damage)
    {
        if (enemyController != null)
        {
            switch (bodyPart)
            {
                case "head":

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
        }
        else
        {
            switch (bodyPart)
            {
                case "head":

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
        }
        
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
        GetComponent<CapsuleCollider>().isTrigger = true;
        
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        rigidbody.freezeRotation = true;
        
        if (GetComponent<PlayerAttack>())//if player died
        {
            GetComponent<PlayerAttack>().enabled = false;
            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerController>().enabled = false;
        }
        else
        {
            GetComponent<EnemyNPCAttack>().enabled = false;
            GetComponent<EnemyNPCMovement>().enabled = false;
            
            EnemyNPCEquipmentSystem enemyNPCEquipmentSystem = GetComponent<EnemyNPCEquipmentSystem>();
            enemyNPCEquipmentSystem.DestroyEquippedWeapons();
            enemyNPCEquipmentSystem.UnEquipAllArmours();
            enemyNPCEquipmentSystem.enabled = false;

            GetComponent<EnemyController>().enabled = false;
            GetComponent<NavMeshAgent>().enabled = false;
        }
    }

    public void SetHealthData(HealthData healthData)
    {
        headHealth = healthData.headHealth;
        torsoHealth = healthData.torsoHealth;
        armsHealth = healthData.armsHealth;
        legsHealth = healthData.legsHealth;
    }

    public void LoadPlayerHealth()
    {
        HealthData healthData;

        if (GetComponent<PlayerAttack>())//if player
        {
            healthData = SaveSystem.LoadData<HealthData>("/PlayerHealthData.json");
            if (healthData == null) return;
            headHealth = healthData.headHealth;
            torsoHealth = healthData.torsoHealth;
            armsHealth = healthData.armsHealth;
            legsHealth = healthData.legsHealth;
        }        
    }

    public void SaveHealth()
    {
        if (GetComponent<PlayerAttack>())//if player
        {
            HealthData healthData = new HealthData(this);
            SaveSystem.SaveData("/PlayerHealthData.json", healthData);
        }
    }
}

[System.Serializable]
public class HealthData
{
    public float headHealth;
    public float torsoHealth;
    public float armsHealth;
    public float legsHealth;

    public HealthData()
    {
        headHealth = 100;
        torsoHealth = 100;
        armsHealth = 100;
        legsHealth = 100;
    }
    public HealthData(PlayerHealth health)
    {
        headHealth = health.headHealth;
        torsoHealth= health.torsoHealth;
        armsHealth = health.armsHealth;
        legsHealth = health.legsHealth;
    }
}
