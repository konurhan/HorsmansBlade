using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    Transform BgImage;
    Button RemoveButton;
    Transform Amount;
    Transform ItemImage;
    Button ActionButton;

    InventoryController inventoryController;
    public ItemDescriptor itemDescriptor;
    public GameObject itemInstance;
    //public bool isInstantiated;

    private void Awake()
    {
        BgImage = transform.GetChild(0);
        Amount = transform.GetChild(1);
        ItemImage = transform.GetChild(2);
        ActionButton = transform.GetChild(3).GetComponent<Button>();
        RemoveButton = transform.GetChild(4).GetComponent<Button>();
        itemInstance = null;
        //isInstantiated = false;
    }

    public void Initialize(int amount, InventoryController controller, ItemDescriptor itemDesc)
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text = amount.ToString();
        ItemImage.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ItemSprites/"+ itemDesc.itemName);
        inventoryController = controller;
        itemDescriptor = itemDesc;
    }

    public void UpdateAmount(int newVal) 
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text = newVal.ToString();
    }

    public void ActionCallback()//if it is a weapon call equip, if it is a consumable call consume, if it is an armour call equip/wear
    {
        Debug.Log("action button is pressed");
        if (itemInstance != null) return;//do not instantiate the item if it already has
        if (itemDescriptor.itemType == ItemType.Weapon) 
        {
            GameObject weapon = Instantiate(Resources.Load(itemDescriptor.itemPrefabPath+itemDescriptor.itemName)) as GameObject;
            weapon.GetComponent<InventoryItem>().SetOwnerReference(inventoryController.gameObject);
            weapon.GetComponentInChildren<Rigidbody>().isKinematic = true;
            weapon.GetComponentInChildren<BoxCollider>().isTrigger = true;
            inventoryController.EquipWeapon(weapon.GetComponent<Weapon>());
            itemInstance = weapon;
        }
        else if (itemDescriptor.itemType == ItemType.Shield)
        {
            GameObject shield = Instantiate(Resources.Load(itemDescriptor.itemPrefabPath + itemDescriptor.itemName)) as GameObject;
            shield.GetComponent<InventoryItem>().SetOwnerReference(inventoryController.gameObject);
            shield.GetComponentInChildren<Rigidbody>().isKinematic = true;
            shield.GetComponentInChildren<BoxCollider>().isTrigger = true;
            inventoryController.EquipShield(shield.GetComponent<Shield>());
            itemInstance = shield;
        }
        else if (itemDescriptor.itemType == ItemType.Armour)
        {
            GameObject armour = Instantiate(Resources.Load(itemDescriptor.itemPrefabPath + itemDescriptor.itemName)) as GameObject;
            armour.GetComponent<InventoryItem>().SetOwnerReference(inventoryController.gameObject);
            
            armour.GetComponent<Rigidbody>().isKinematic = true;
            BoxCollider[] colls = armour.GetComponents<BoxCollider>();
            foreach (BoxCollider col in colls)
            {
                col.enabled = false;
            }
            armour.GetComponent<MeshRenderer>().enabled = false;
            
            inventoryController.EquipArmour(armour, itemDescriptor);
            itemInstance = armour;
        }
        else if (itemDescriptor.itemType == ItemType.Consumable)
        {
            //fill
        }
    }

    public void RemoveCallback()
    {
        if (itemDescriptor.itemType == ItemType.Armour && itemInstance != null)
        {
            itemInstance.GetComponent<Rigidbody>().isKinematic = false;
            BoxCollider[] colls = itemInstance.GetComponents<BoxCollider>();
            foreach (BoxCollider col in colls)
            {
                col.enabled = true;
            }
            itemInstance.GetComponent<MeshRenderer>().enabled = true;
            inventoryController.UnequipArmour(itemInstance.GetComponent<Armour>());// destroys the itemInstance, so call drop uninstantiated method

            inventoryController.DropUninstantiatedSlot(itemDescriptor);
            inventoryController.RemoveSlot(itemDescriptor);
            return;
        }

        if (itemInstance != null)
        {
            inventoryController.DropInstantiatedSlot(itemDescriptor,itemInstance);
        }
        else
        {
            inventoryController.DropUninstantiatedSlot(itemDescriptor);
        }
        
        inventoryController.RemoveSlot(itemDescriptor);
    }

    public void DropSingleItemCallback()
    {
        int curAmount = inventoryController.items[itemDescriptor] - 1;
        inventoryController.DropSingleItem(itemDescriptor);
        if (curAmount > 0)
        {
            Amount.gameObject.GetComponent<TextMeshProUGUI>().text = curAmount.ToString();
        }
        else
        {
            inventoryController.RemoveSlot(itemDescriptor);
        }
    }
}
