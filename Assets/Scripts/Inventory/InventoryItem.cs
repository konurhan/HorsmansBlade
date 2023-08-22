using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    private GameObject player;
    private InventoryController playerInventory;

    public GameObject owner;
    
    [SerializeField] protected string itemName;
    [SerializeField] protected float weight;
    [SerializeField] protected float price;
    public string Name { get { return itemName; } set { itemName = value; } }
    public float Weight { get { return weight; } set { weight = value; } }
    public float Price { get { return price; } set { price = value; } }

    [SerializeField] private bool collectibleFlag;

    private void Awake()
    {
        player = InventoryUI.Instance.Player;
        playerInventory = player.GetComponent<InventoryController>();

        collectibleFlag = false;
    }

    protected virtual void Update()
    {
        HandeleCollection();
    }

    public void HandeleCollection()
    {
        Debug.Log("HandeleCollection is called");
        if (owner != null)
        {
            BecomeNonCollectibleByPlayer();
            return;
        }

        float distance = (transform.position - player.transform.position).magnitude;
        Debug.Log("dist: " + distance);
        if (distance <= 4f && !collectibleFlag)
        {
            BecomeCollectibleByPlayer();
        }
        else if (distance > 4f && collectibleFlag)
        {
            
            BecomeNonCollectibleByPlayer();
        }
    }

    private void BecomeNonCollectibleByPlayer()
    {
        collectibleFlag = false;
        playerInventory.collectibleItems.Remove(this);
    }

    private void BecomeCollectibleByPlayer()
    {
        collectibleFlag = true;
        if (playerInventory.collectibleItems.Contains(this)) return;
        playerInventory.collectibleItems.Add(this);
    }

    public void SetOwnerReference(GameObject owner)
    {
        this.owner = owner;
    }


}