using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldInventory : MonoBehaviour
{
    public static WorldInventory Instance;

    public List<ItemDescriptor> worldItemDescriptions;

    private void Awake()
    {
        Instance = this;
        worldItemDescriptions = new List<ItemDescriptor>();
        LoadItemDescriptions();
    }
    
    void Start()
    {
        
    }

    public void LoadItemDescriptions()
    {
        //load from save file
    }
}
