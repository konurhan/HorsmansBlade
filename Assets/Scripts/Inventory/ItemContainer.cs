using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainer : MonoBehaviour//attach this script to a chest
{
    public int containerID;
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
    [SerializeField] private List<bool> isEquipped;// use only if container belongs to an NPC
    [SerializeField] private List<string> PrefabPaths;
    [SerializeField] private List<ItemType> ItemTypes;
    [SerializeField] private List<int> amount;

    private void Awake()
    {
        WorldInventory.Instance.worldItemContainers.Add(this);
        LoadContainedItems();
    }

    void Start()
    {
        //LoadContainedItemsTest();

        player = InventoryUI.Instance.Player;
        playerInventory = player.GetComponent<InventoryController>();
        ContainerSlotsTransform = InventoryUI.Instance.ContainerSlotsTransform;

        if(isNPC)
        {
            EquipMarkedItems();
        }
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

    #region NPC specific methods

    public void EquipMarkedItems()
    {
        EnemyNPCEquipmentSystem equipmentSystem = GetComponent<EnemyNPCEquipmentSystem>();
        for (int i = 0; i < isEquipped.Count; i++)
        {
            if (isEquipped[i])
            {
                ItemDescriptor itemDesc = FindDescriptionInWorldInventory(PrefabNames[i]);
                if (itemDesc.itemType == ItemType.Weapon)
                {
                    equipmentSystem.EquipMeleeWeapon(itemDesc);
                }
                else if(itemDesc.itemType == ItemType.Armour)
                {
                    equipmentSystem.EquipArmour(itemDesc);
                }
                else if( itemDesc.itemType == ItemType.Shield)
                {
                    equipmentSystem.EquipShield(itemDesc);
                }
            }
        }
    }

    #endregion


    #region save/load
    private void LoadContainedItems()
    {
        containedItems.Clear();
        for (int i = 0; i < PrefabNames.Count; i++)
        {
            ItemDescriptor worldDesc = FindDescriptionInWorldInventory(PrefabNames[i]);
            if (worldDesc == null)
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

    public void SerializeContainedItems()
    {
        PrefabNames.Clear();
        PrefabPaths.Clear();
        ItemTypes.Clear();
        amount.Clear();

        foreach (ItemDescriptor itemDesc in containedItems.Keys)
        {
            PrefabNames.Add(itemDesc.itemName);
            PrefabPaths.Add(itemDesc.itemPrefabPath);
            ItemTypes.Add(itemDesc.itemType);
            amount.Add(containedItems[itemDesc]);
        }
    }
    #endregion

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
        CreateContainerSlotsForAllItems();
    }

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

    public void CreateContainerSlotsForAllItems()//call when panel is activated
    {
        InventoryUI.Instance.DestroyAllContainerSlots();

        foreach (ItemDescriptor item in containedItems.Keys)
        {
            AddSlot(containedItems[item], item);
        }
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

[System.Serializable]
public class ContainerData
{
    public int containerId;
    public string containerName;
    public List<KeyValuePair<ItemDescriptor, int>> containedItems;

    public ContainerData()
    {
        
    }

    public ContainerData(ItemContainer container)
    {
        containerId = container.containerID;
        containerName = container.Name;
        containedItems = container.containedItems.ToList();
    }
}
