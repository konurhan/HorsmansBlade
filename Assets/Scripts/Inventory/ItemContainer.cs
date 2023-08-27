using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour//attach this script to a chest
{
    public Dictionary<ItemDescriptor,int> containedItems;

    private void Awake()
    {
        LoadContainedItems();
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void LoadContainedItems()//implement with saveload system
    {

    }
}
