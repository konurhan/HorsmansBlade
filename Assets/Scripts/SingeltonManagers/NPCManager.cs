using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[DefaultExecutionOrder(0)]
public class NPCManager : MonoBehaviour
{
    private static NPCManager instance;
    public static NPCManager Instance 
    { 
        get 
        { 
            return instance; 
        }
        private set
        {
            instance = value;
        }
    }

    public List<EnemyController> enemyNPCs = new List<EnemyController>();
    

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        LoadNPCHealth();

        InventoryUI.Instance.onGameSaved += SaveNPCHealth;

        Menu.onGamePaused += ConfigureForPause;
        Menu.onGameResumed += ConfigureForResume;

        //SetDestinationLocalPositions();
        //SetNumberOfDestinations(10);
    }

    /*public void SetDestinationLocalPositions()
    {
        int count = enemyNPCs.Count;
        float radius = enemyNPCs[0].GetComponent<EnemyNPCMovement>().surroundingRadius;
        float angleStep = 360f / count;
        float startAngle = angleStep / 2;

        for (int i = 0; i < count; i++)
        {
            SurroundingDest newDest = new SurroundingDest();
            float angle = startAngle + i * angleStep;
            float radians = angle * Mathf.Deg2Rad;
            newDest.relativePosToPlayer = new Vector3(radius * Mathf.Cos(radians), 0, radius * Mathf.Sin(radians));
            relativePos.Add(newDest);
        }
    }*/

    /*public void SetEnemyDestinationOffsets()//call it every time an enemy is added, through enemy controller
    {
        int count = enemyNPCs.Count;
        float npcSpan = 60 * count;// the angle in which the NPCs group in around the player, equally spaced
        if (npcSpan > 360) npcSpan = 360;

        float middleNPC = 90; //angle in which the npc in the middle of the array will face the enemy
        float startAngle = middleNPC - npcSpan / 2;
        
        for (int i=0; i < count; i++)
        {
            float radius = enemyNPCs[i].GetComponent<EnemyNPCMovement>().surroundingRadius;
            float angle = i/count * npcSpan + startAngle;
            float radians = angle * Mathf.Deg2Rad;
            enemyNPCs[i].GetComponent<EnemyNPCMovement>().destinationOffset = new Vector3(radius*Mathf.Cos(radians), 0, radius * Mathf.Sin(radians));
        }
    }*/


    public void ConfigureForPause()
    {
        foreach(EnemyController enemyController in enemyNPCs)
        {
            GameObject enemy = enemyController.gameObject;
            enemy.GetComponent<Animator>().enabled = false;
            enemy.GetComponent<EnemyNPCMovement>().enabled = false;
            enemy.GetComponent<EnemyNPCAttack>().enabled = false;
            enemy.GetComponent<Rigidbody>().isKinematic = true;
            enemy.GetComponent<PlayerHealth>().enabled = false;
            enemy.GetComponent<NavMeshAgent>().enabled = false;
            enemy.GetComponent<ItemContainer>().enabled = false;
            enemy.GetComponent<EnemyNPCEquipmentSystem>().enabled = false;
            enemy.GetComponent<EnemyController>().enabled = false;
        }
    }

    public void ConfigureForResume()
    {
        foreach (EnemyController enemyController in enemyNPCs)
        {
            GameObject enemy = enemyController.gameObject;
            enemy.GetComponent<Animator>().enabled = true;
            enemy.GetComponent<EnemyNPCMovement>().enabled = true;
            enemy.GetComponent<EnemyNPCAttack>().enabled = true;
            enemy.GetComponent<Rigidbody>().isKinematic = false;
            enemy.GetComponent<PlayerHealth>().enabled = true;
            enemy.GetComponent<NavMeshAgent>().enabled = true;
            enemy.GetComponent<ItemContainer>().enabled = true;
            enemy.GetComponent<EnemyNPCEquipmentSystem>().enabled = true;
            enemy.GetComponent<EnemyController>().enabled = true;
        } 
    }

    #region save/load methods

    public void LoadNPCHealth()
    {
        HealthData healthData;

        NPCHealthCollection npcHealthDatas = SaveSystem.LoadData<NPCHealthCollection>("/NPCHealthDataCollection.json");
        if (npcHealthDatas == null) return;

        foreach (EnemyController controller in enemyNPCs)
        {
            int npcID = controller.npcID;
            healthData = npcHealthDatas.healthDatas[npcID];
            controller.gameObject.GetComponent<PlayerHealth>().SetHealthData(healthData);
        }
    }

    public void SaveNPCHealth()
    {
        NPCHealthCollection npcHealthCollection = new NPCHealthCollection();
        
        foreach (EnemyController controller in enemyNPCs)
        {
            int npcID = controller.npcID;
            HealthData healthData = new HealthData(controller.gameObject.GetComponent<PlayerHealth>());
            npcHealthCollection.healthDatas.Add(npcID, healthData);
        }
        
        SaveSystem.SaveData("/NPCHealthDataCollection.json", npcHealthCollection);
    }

    #endregion
}

[System.Serializable]
public class NPCHealthCollection
{
    public Dictionary<int, HealthData> healthDatas;
    public NPCHealthCollection()
    {
        healthDatas = new Dictionary<int, HealthData>();
    }
}