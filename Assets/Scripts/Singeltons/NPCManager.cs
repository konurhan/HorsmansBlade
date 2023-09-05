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

    //public GameObject target;
    //public float surroundRadius = 2f;
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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            //GameObject target = npc.GetComponent<EnemyNPCMovement>().target;
            float radius = enemyNPCs[i].GetComponent<EnemyNPCMovement>().surroundingRadius;
            float angle = i/count * npcSpan + startAngle;
            Debug.Log("enemy will position it self at an angle of: " + angle + " degrees");
            float radians = angle * Mathf.Deg2Rad;
            enemyNPCs[i].GetComponent<EnemyNPCMovement>().destinationOffset = new Vector3(radius*Mathf.Cos(radians), 0, radius * Mathf.Sin(radians));
            Debug.Log("dest offset: " + enemyNPCs[i].GetComponent<EnemyNPCMovement>().destinationOffset);
        }
    }
}
