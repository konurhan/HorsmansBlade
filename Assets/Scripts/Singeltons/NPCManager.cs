using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    }

    public void SetEnemyDestinationOffsets()//call it every time an enemy is added, through enemy controller
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
            HealthData healthData = new HealthData(gameObject.GetComponent<PlayerHealth>());
            npcHealthCollection.healthDatas.Add(npcID, healthData);
        }
        
        SaveSystem.SaveData("/NPCHealthDataCollection.json", npcHealthCollection);
    }

    #endregion
}

public class NPCHealthCollection
{
    public Dictionary<int, HealthData> healthDatas;
    public NPCHealthCollection()
    {
        healthDatas = new Dictionary<int, HealthData>();
    }
}