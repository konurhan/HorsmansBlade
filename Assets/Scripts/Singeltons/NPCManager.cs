using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        for (int i=0; i < count; i++)
        {
            //GameObject target = npc.GetComponent<EnemyNPCMovement>().target;
            float radius = enemyNPCs[i].GetComponent<EnemyNPCMovement>().surroundingRadius;
            float angle = 360f * i / count;
            enemyNPCs[i].GetComponent<EnemyNPCMovement>().destinationOffset = new Vector3(radius*Mathf.Cos(angle), 0, radius * Mathf.Sin(angle));
        }
    }
}
