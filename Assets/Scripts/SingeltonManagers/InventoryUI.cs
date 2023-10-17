using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using static UnityEditor.Progress;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    private InventoryItem closest;
    private ItemContainer closestContainer;
    public Transform collectText;
    
    public GameObject Player;
    
    public Transform InventorySlotsTransform;
    public List<InventorySlot> slots;

    public Transform ContainerSlotsTransform;
    public List<ContainerSlot> containerSlots;

    public Transform WeaponSlotsParent;
    public Transform ArmourSlotsParent;
    public Transform ItemInfoParent;
    public Transform FloatingTextParent;

    public delegate void OnGameSaved();
    public event OnGameSaved onGameSaved;//invoke this upon button click

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
        LootClosestContainer();
        OpenCloseInventory();
    }

    public void AddSlot(int amount, ItemDescriptor itemDesc)
    {
        ArrangeSlotPositions();
        int rowCount = InventorySlotsTransform.childCount;
        for(int i = 0; i < rowCount; i++)
        {
            if (InventorySlotsTransform.GetChild(i).childCount == 4) continue;
            GameObject newSlot = Instantiate(Resources.Load("Prefabs/UI/InventorySlot")) as GameObject;
            newSlot.transform.SetParent(InventorySlotsTransform.GetChild(i), false);
            newSlot.GetComponent<InventorySlot>().Initialize(amount, Player.GetComponent<InventoryController>(), itemDesc);
            slots.Add(newSlot.GetComponent<InventorySlot>());
            break;
        }
    }

    public void AddSlot(GameObject slot)
    {
        ArrangeSlotPositions();
        slots.Add(slot.GetComponent<InventorySlot>());
        int rowCount = InventorySlotsTransform.childCount;
        for (int i = 0; i < rowCount; i++)
        {
            if (InventorySlotsTransform.GetChild(i).childCount == 4) continue;
            slot.transform.SetParent(InventorySlotsTransform.GetChild(i), false);
            slot.transform.localPosition = Vector3.zero;
            break;
        }
    }

    public void ArrangeSlotPositions()//call after a slot is destroyed or moved, so that no empty slots remain on top
    {
        int rowCount = InventorySlotsTransform.childCount;
        foreach (InventorySlot slot in slots)
        {
            for(int i = 0; i < rowCount; i++)
            {
                if (InventorySlotsTransform.GetChild(i).childCount == 4) continue;
                slot.gameObject.transform.SetParent(InventorySlotsTransform.GetChild(i), false);
                break;
            }
        }
    }

    public void DestroySlot(ItemDescriptor itemDesc)//call FindSlotByItemDescriptor
    {
        InventorySlot toBeDestroyed = FindSlotByItemDescriptor(itemDesc);
        if (toBeDestroyed != null)
        {
            slots.Remove(toBeDestroyed);
            Destroy(toBeDestroyed.gameObject);
            return;
        }
        /*foreach (InventorySlot slot in slots.ToList())
        {
            if (slot.itemDescriptor.itemName == itemDesc.itemName)
            {
                GameObject slotObj = slot.gameObject;
                slots.Remove(slot);
                Destroy(slotObj);
                return;
            }
        }*/
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
        if (closest == null)
        {
            collectibles.RemoveAt(0);
            closest = null;
            return;
        }

        float dist = (closest.gameObject.transform.position - Player.transform.position).magnitude;//arrow için hata veriyor

        for (int i = 1; i < collectibles.Count; i++)
        {
            if (collectibles[i] == null) continue;
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

    public void MakeClosestLootableContainerVisible()//call when the character moves
    {
        List<ItemContainer> lootables = Player.GetComponent<InventoryController>().lootableContainers;

        if (lootables.Count == 0)
        {
            closestContainer = null;
            return;
        }

        closestContainer = lootables[0];
        float dist = (closestContainer.gameObject.transform.position - Player.transform.position).magnitude;

        for (int i = 1; i < lootables.Count; i++)
        {
            float newDist = (lootables[i].gameObject.transform.position - Player.transform.position).magnitude;
            if (newDist < dist)
            {
                closestContainer = lootables[i];
                dist = newDist;
            }
        }

        collectText.gameObject.SetActive(true);
        collectText.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Press (L) to loot " + closestContainer.Name;
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

    public void LootClosestContainer()
    {
        if (closestContainer == null) return;
        if (!collectText.gameObject.activeSelf) return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            //open
            closestContainer.OnOpenedByPlayer();
            collectText.gameObject.SetActive(false);

            InventorySlotsTransform.parent.gameObject.SetActive(true);
            //RecreateInventorySlots();
        }
    }

    public InventorySlot FindSlotByItemDescriptor(ItemDescriptor itemDescriptor)
    {
        foreach (InventorySlot inventorySlot in slots)
        {
            if (inventorySlot.itemDescriptor.itemName == itemDescriptor.itemName)
            {
                return inventorySlot;
            }
        }
        //search for WeaponSlotsParent
        for (int i = 0; i < 4; i++) 
        {
            if (WeaponSlotsParent.GetChild(i).childCount == 0) continue;
            if (WeaponSlotsParent.GetChild(i).GetChild(0).gameObject.GetComponent<InventorySlot>().itemDescriptor.itemName == itemDescriptor.itemName)
            {
                return WeaponSlotsParent.GetChild(i).GetChild(0).gameObject.GetComponent<InventorySlot>();
            }
        }
        //search for ArmourSlotsParent
        for (int i = 0; i < 5; i++)
        {
            if (ArmourSlotsParent.GetChild(i).childCount == 0) continue;
            if (ArmourSlotsParent.GetChild(i).GetChild(0).gameObject.GetComponent<InventorySlot>().itemDescriptor.itemName == itemDescriptor.itemName)
            {
                return ArmourSlotsParent.GetChild(i).GetChild(0).gameObject.GetComponent<InventorySlot>();
            }
        }
        return null;
    }

    public void OpenCloseInventory()
    {
        if (Input.GetKeyDown(KeyCode.I) && InventorySlotsTransform.parent.gameObject.activeInHierarchy)
        {
            InventorySlotsTransform.parent.gameObject.SetActive(false);
            //ClearInventoryPanel();
        }else if (Input.GetKeyDown(KeyCode.I) && !InventorySlotsTransform.parent.gameObject.activeInHierarchy)
        {
            InventorySlotsTransform.parent.gameObject.SetActive(true);
            //RecreateInventorySlots();
        }
    }

    public void OnContainerPanelClosedByPlayer()//call from container panel close button
    {
        ContainerSlotsTransform.parent.gameObject.SetActive(false);
        DestroyAllContainerSlots();

        InventorySlotsTransform.parent.gameObject.SetActive(false);
        //ClearInventoryPanel();
    }

    public void DestroyAllContainerSlots()//same ui panel will be used for all containers in the game so it has to be reset each time the panel is deactivated in the canvas
    {
        foreach (ContainerSlot slot in containerSlots.ToList())
        {
            Destroy(slot.gameObject);
        }
        containerSlots.Clear();
    }

    public void ClearInventoryPanel()
    {
        foreach (InventorySlot slot in slots.ToList())
        {
            Destroy(slot.gameObject);
        }
        slots.Clear();
    }

    public void CreateInventorySlotsForAllItems()
    {
        Dictionary<ItemDescriptor, int> items = Player.GetComponent<InventoryController>().items;
        foreach (ItemDescriptor item in items.Keys)
        {
            AddSlot(items[item], item);
        }
    }

    public void SaveGameCall()
    {
        onGameSaved.Invoke();
    }
}
