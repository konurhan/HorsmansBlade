using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    private InventoryItem closest;
    public Transform collectText;
    
    public GameObject Player;
    
    public Transform InventorySlotsTransform;
    public Dictionary<string, InventorySlot> slots;

    private void Awake()
    {
        Instance = this;
        slots = new Dictionary<string, InventorySlot>();
        LoadSlots();
        collectText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CollectClosestItem();
    }

    public void AddSlot(int amount, string itemName, InventoryItem itemClass)
    {
        int rowCount = InventorySlotsTransform.childCount;
        for(int i = 0; i < rowCount; i++)
        {
            if (InventorySlotsTransform.GetChild(i).childCount == 4) continue;
            GameObject newSlot = Instantiate(Resources.Load("Prefabs/UI/InventorySlot")) as GameObject;
            newSlot.transform.SetParent(InventorySlotsTransform.GetChild(i), false);
            newSlot.GetComponent<InventorySlot>().Initialize(itemName, amount, Player.GetComponent<InventoryController>(), itemClass);
            //newSlot.GetComponent<InventorySlot>().AddActionCallback(Player,itemClass);
            slots.Add(itemName, newSlot.GetComponent<InventorySlot>());
            break;
        }
    }

    public void DestroySlot(string itemName)
    {
        GameObject slotObj = slots[itemName].gameObject;
        slots.Remove(itemName);
        Destroy(slotObj);
    }

    public void UpdateSlotAmount(int newVal, string itemName)
    {
        slots[itemName].UpdateAmount(newVal);
    }

    public void LoadSlots()
    {
        //implement after savesystem
    }

    public void MakeClosestCollectibleItemVisible()//call when the character moves
    {
        List<InventoryItem> collectibles = Player.GetComponent<InventoryController>().collectibleItems;

        if (collectibles.Count == 0)
        {
            collectText.gameObject.SetActive(false);
            closest = null;
            return;
        }

        closest = collectibles[0];
        float dist = (closest.gameObject.transform.position - Player.transform.position).magnitude;

        for (int i = 1; i < collectibles.Count; i++)
        {
            float newDist = (collectibles[i].gameObject.transform.position - Player.transform.position).magnitude;
            if (newDist < dist)
            {
                closest = collectibles[i];
                dist = newDist;
            }
        }

        /*if (dist > Player.GetComponent<InventoryController>().collectRadius) 
        { 
            closest = null;
            return;
        }*/
        

        collectText.gameObject.SetActive(true);
        collectText.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Press (F) to collect "+closest.Name;
        //find closest collectible item from the list in inventory controller script.
        //print a message to the screen with the name of the item and the button to collect it
        //after button trigger collect that item
    }

    public void CollectClosestItem()
    {
        if (closest == null) return;
        if (!collectText.gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Player.GetComponent<InventoryController>().CollectSingleItem(closest.gameObject);
            collectText.gameObject.SetActive(false);
        }
    }

    
}
