using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    private InventoryItem closest;
    public Transform collectText;
    
    public GameObject Player;
    
    public Transform InventorySlotsTransform;
    public List<InventorySlot> slots;

    private void Awake()
    {
        Instance = this;
        slots = new List<InventorySlot>();
        LoadSlots();
        collectText.gameObject.SetActive(false);
    }

    private void Update()
    {
        CollectClosestItem();
    }

    public void AddSlot(int amount, ItemDescriptor itemDesc)
    {
        int rowCount = InventorySlotsTransform.childCount;
        for(int i = 0; i < rowCount; i++)
        {
            if (InventorySlotsTransform.GetChild(i).childCount == 4) continue;
            GameObject newSlot = Instantiate(Resources.Load("Prefabs/UI/InventorySlot")) as GameObject;
            newSlot.transform.SetParent(InventorySlotsTransform.GetChild(i), false);
            newSlot.GetComponent<InventorySlot>().Initialize(amount, Player.GetComponent<InventoryController>(), itemDesc);
            //newSlot.GetComponent<InventorySlot>().AddActionCallback(Player,itemClass);
            slots.Add(newSlot.GetComponent<InventorySlot>());
            break;
        }
    }

    public void DestroySlot(ItemDescriptor itemDesc)//call FindSlotByItemDescriptor
    {
        foreach (InventorySlot slot in slots.ToList())
        {
            if (slot.itemDescriptor.itemName == itemDesc.itemName)
            {
                GameObject slotObj = slot.gameObject;
                slots.Remove(slot);
                Destroy(slotObj);
                return;
            }
        }
        
    }

    public void UpdateSlotAmount(int newVal, ItemDescriptor itemDesc)
    {
        InventorySlot inventorySlot = FindSlotByItemDescriptor(itemDesc);
        if (inventorySlot == null)
        {
            Debug.LogError("Trying to update the Amount of a non-existent slot");
        }
        inventorySlot.UpdateAmount(newVal);
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

        collectText.gameObject.SetActive(true);
        collectText.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Press (F) to collect "+closest.Name;
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

    public InventorySlot FindSlotByItemDescriptor(ItemDescriptor itemDescriptor)
    {
        foreach (InventorySlot inventorySlot in slots)
        {
            if (inventorySlot.itemDescriptor == itemDescriptor)
            {
                return inventorySlot;
            }
        }

        return null;
    }
    
}
