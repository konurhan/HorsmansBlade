using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;
using static UnityEngine.UI.GridLayoutGroup;

public class ItemContainer : MonoBehaviour//attach this script to a chest
{

    [SerializeField] protected string containerName;
    public string Name { get { return containerName; } set { containerName = value; } }

    public Transform ContainerSlotsTransform;
    //public List<ContainerSlot> slots;

    public Dictionary<ItemDescriptor,int> containedItems = new Dictionary<ItemDescriptor, int>();

    [SerializeField] private bool lootableFlag;

    private GameObject player;
    private InventoryController playerInventory;

    public bool isNPC;

    [Header("Item Descriptor Values")]//load these values from the save file
    [SerializeField] private List<string> PrefabNames;
    [SerializeField] private List<string> PrefabPaths;
    [SerializeField] private List<ItemType> ItemTypes;
    [SerializeField] private List<int> amount;

    private void Awake()
    {
    }

    void Start()
    {
        LoadContainedItems();
        LoadContainedItemsTest();

        player = InventoryUI.Instance.Player;
        playerInventory = player.GetComponent<InventoryController>();
        ContainerSlotsTransform = InventoryUI.Instance.ContainerSlotsTransform;
    }

    
    void Update()
    {
        if (isNPC)
        {
            HandleCollectionNPC();
        }
        else
        {
            HandleCollection();
        }
    }

    public void TakeItemFromContainer(ItemDescriptor toBeTaken)
    {
        int curCount = containedItems[toBeTaken];
        ContainerSlot slot = FindSlotByItemDescriptor(toBeTaken);
        if (curCount > 1)
        {
            slot.UpdateAmount(curCount);
            containedItems[toBeTaken]--;
        }
        else
        {
            InventoryUI.Instance.containerSlots.Remove(slot);
            Destroy(slot.gameObject);
            containedItems.Remove(toBeTaken);
        }
        InventoryUI.Instance.Player.GetComponent<InventoryController>().AddItemByDescriptor(toBeTaken);
    }

    public void StoreItemToContainer(ItemDescriptor incoming)
    {
        ItemDescriptor local = FindDescriptionInContainer(incoming.itemName);
        if (local != null)
        {
            containedItems[local]++;
        }
        else
        {
            containedItems.Add(incoming, 1);
            AddSlot(1, incoming);
        }
    }

    private void LoadContainedItems()//implement with saveload system
    {
        //load descriptor values from the save file if any exists
        containedItems.Clear();
        for (int i = 0; i < PrefabNames.Count; i++)
        {
            ItemDescriptor worldDesc = FindDescriptionInWorldInventory(PrefabNames[i]);
            if ( worldDesc == null)
            {
                ItemDescriptor newDesc = new ItemDescriptor(PrefabNames[i], PrefabPaths[i], ItemTypes[i]);
                containedItems.Add(newDesc, amount[i]);
                WorldInventory.Instance.worldItemDescriptions.Add(newDesc);
            }
            else
            {
                containedItems.Add(worldDesc, amount[i]);
            }
            
        }
    }

    private void LoadContainedItemsTest()
    {

    }

    #region Interaction
    //enemy npc containers should become visible only after the death of th npc

    public void HandleCollectionNPC()
    {
        if (!GetComponent<PlayerHealth>().isDead) return;
        float distance = (transform.position - player.transform.position).magnitude;
        if (distance <= 4f && !lootableFlag)
        {
            BecomeLootableByPlayer();
        }
        else if (distance > 4f && lootableFlag)
        {
            BecomeNonLootableByPlayer();
        }
    }

    public void HandleCollection()
    {
        float distance = (transform.position - player.transform.position).magnitude;
        if (distance <= 4f && !lootableFlag)
        {
            BecomeLootableByPlayer();
        }
        else if (distance > 4f && lootableFlag)
        {
            BecomeNonLootableByPlayer();
        }
    }

    private void BecomeNonLootableByPlayer()
    {
        lootableFlag = false;
        playerInventory.lootableContainers.Remove(this);
    }

    private void BecomeLootableByPlayer()
    {
        lootableFlag = true;
        if (playerInventory.lootableContainers.Contains(this)) return;
        playerInventory.lootableContainers.Add(this);
    }

    #endregion


    #region UI Layer

    public void OnOpenedByPlayer()//call from key downs when container message is displayed on the canvas
    {
        ContainerSlotsTransform.parent.gameObject.SetActive(true);
        CreateSlotsForAllItems();
    }

    /*public void OnContainerPanelClosedByPlayer()//call from container close button
    {
        ContainerSlotsTransform.parent.gameObject.SetActive(false);
        DestroyAllSlots();
    }*/

    public void AddSlot(int amount, ItemDescriptor itemDesc)
    {
        int rowCount = ContainerSlotsTransform.childCount;
        for (int i = 0; i < rowCount; i++)
        {
            if (ContainerSlotsTransform.GetChild(i).childCount == 4) continue;
            GameObject newSlot = Instantiate(Resources.Load("Prefabs/UI/ContainerSlot")) as GameObject;
            newSlot.transform.SetParent(ContainerSlotsTransform.GetChild(i), false);
            newSlot.GetComponent<ContainerSlot>().Initialize(amount, this, itemDesc);
            InventoryUI.Instance.containerSlots.Add(newSlot.GetComponent<ContainerSlot>());
            break;
        }
    }

    public void DestroySlotByItemDescription(ItemDescriptor itemDesc)//call FindSlotByItemDescriptor
    {
        foreach (ContainerSlot slot in InventoryUI.Instance.containerSlots.ToList())
        {
            if (slot.itemDescriptor.itemName == itemDesc.itemName)
            {
                GameObject slotObj = slot.gameObject;
                InventoryUI.Instance.containerSlots.Remove(slot);
                Destroy(slotObj);
                return;
            }
        }
    }

    public void CreateSlotsForAllItems()//call when panel is activated
    {
        foreach(ItemDescriptor item in containedItems.Keys)
        {
            AddSlot(containedItems[item], item);
        }
    }

    public void DestroyAllSlots()//same ui panel will be used for all containers in the game so it has to be reset each time the panel is deactivated in the canvas
    {
        foreach(ContainerSlot slot in InventoryUI.Instance.containerSlots.ToList())
        {
            Destroy(slot.gameObject);
        }
        InventoryUI.Instance.containerSlots.Clear();
    }

    #endregion

    #region Utils

    public ItemDescriptor FindDescriptionInContainer(string itemName)
    {
        foreach (ItemDescriptor itemDesc in containedItems.Keys)
        {
            if (itemDesc.itemName == itemName)
            {
                return itemDesc;
            }
        }
        return null;
    }

    public ItemDescriptor FindDescriptionInWorldInventory(string itemName)
    {
        List<ItemDescriptor> items = WorldInventory.Instance.worldItemDescriptions;
        foreach (ItemDescriptor itemDesc in items)
        {
            if (itemDesc.itemName == itemName)
            {
                return itemDesc;
            }
        }
        Debug.LogWarning("Cannot find ItemDescriptor named " + itemName + " in the world inventory");
        return null;
    }

    public ContainerSlot FindSlotByItemDescriptor(ItemDescriptor itemDescriptor)
    {
        foreach (ContainerSlot inventorySlot in InventoryUI.Instance.containerSlots)
        {
            if (inventorySlot.itemDescriptor == itemDescriptor)
            {
                return inventorySlot;
            }
        }

        return null;
    }

    #endregion
}
