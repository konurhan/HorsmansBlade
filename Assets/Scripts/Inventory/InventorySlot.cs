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
    InventoryItem inventoryItem;

    private void Awake()
    {
        BgImage = transform.GetChild(0);
        Amount = transform.GetChild(1);
        ItemImage = transform.GetChild(2);
        ActionButton = transform.GetChild(3).GetComponent<Button>();
        RemoveButton = transform.GetChild(4).GetComponent<Button>();
    }

    public void Initialize(string spriteName, int amount, InventoryController controller, InventoryItem item)
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text = amount.ToString();
        ItemImage.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ItemSprites/"+spriteName);
        inventoryController = controller;
        inventoryItem = item;
    }

    public void UpdateAmount(int newVal) 
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text += newVal.ToString();
    }

    public void ActionCallback()//if it is a weapon call equip, if it is a consumable call consume, if it is an armour call equip/wear
    {
        Debug.Log("action button is pressed");
        if (inventoryItem.GetComponent<Weapon>()) 
        { 
            inventoryController.EquipWeapon(inventoryItem.GetComponent<Weapon>());
        }
        else if (inventoryItem.GetComponent<Shield>())
        {
            inventoryController.EquipShield(inventoryItem.GetComponent<Shield>());
        }
        else if (inventoryItem.GetComponent<Armour>())
        {
            inventoryController.EquipArmour(inventoryItem.GetComponent<Armour>());
        }
    }

    public void RemoveCallback()
    {
        string itemName = inventoryItem.Name;
        inventoryController.DropAllSlotItems(itemName);
        inventoryController.RemoveSlot(itemName);
    }
}
