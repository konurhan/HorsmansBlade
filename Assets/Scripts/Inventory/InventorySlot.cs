using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Transform BgImage;
    Button RemoveButton;
    Transform Amount;
    Transform ItemImage;
    Button ActionButton;

    InventoryController inventoryController;
    public ItemDescriptor itemDescriptor;
    public GameObject itemInstance;

    public GameObject itemInfo;

    //bool isEquipped;

    private void Awake()
    {
        BgImage = transform.GetChild(0);
        Amount = transform.GetChild(1);
        ItemImage = transform.GetChild(2);
        ActionButton = transform.GetChild(3).GetComponent<Button>();
        RemoveButton = transform.GetChild(4).GetComponent<Button>();
        itemInstance = null;
        
        itemInfo = Instantiate(Resources.Load("Prefabs/UI/ItemInfoPopup"), InventoryUI.Instance.ItemInfoParent) as GameObject;
        itemInfo.SetActive(false);
    }

    public void Initialize(int amount, InventoryController controller, ItemDescriptor itemDesc)
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text = amount.ToString();
        ItemImage.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ItemSprites/"+ itemDesc.itemName);
        inventoryController = controller;
        itemDescriptor = itemDesc;
    }

    public void SetupItemInfo()
    {
        itemInfo.transform.position = gameObject.transform.position + new Vector3(-155, 50, 0);

        GameObject instance;
        if (itemInstance == null)
        {
            Debug.Log(itemDescriptor.itemPrefabPath + itemDescriptor.itemName);
            instance = Instantiate(Resources.Load(itemDescriptor.itemPrefabPath + itemDescriptor.itemName)) as GameObject;
        }
        else
        {
            instance = itemInstance;
        }
        InventoryItem inventoryItem = instance.GetComponent<InventoryItem>();
        itemInfo.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "Item Name: " + inventoryItem.Name;
        itemInfo.transform.GetChild(1).gameObject.GetComponent<TextMeshProUGUI>().text = "Price: " + inventoryItem.Price;
        itemInfo.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>().text = "Weight: " + inventoryItem.Weight;

        if (instance.GetComponent<Armour>())
        {
            itemInfo.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = "Protection: " + instance.GetComponent<Armour>().protection;
        }
        else if (instance.GetComponent<RangedWeapon>())
        {
            itemInfo.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = "Max Force: " + instance.GetComponent<RangedWeapon>().maximumForce;
        }
        else if (instance.GetComponent<MeleeWeapon>())
        {
            itemInfo.transform.GetChild(3).gameObject.GetComponent<TextMeshProUGUI>().text = "Max Force: " + instance.GetComponent<MeleeWeapon>().MaxDamage;
        }
        else
        {
            itemInfo.transform.GetChild(3).gameObject.SetActive(false);
        }
        
        if (instance != itemInstance)
        {
            Destroy(instance);
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("called");
        if (itemInfo.activeInHierarchy) return;
        itemInfo.SetActive(true);
        SetupItemInfo();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("called 2");
        if (!itemInfo.activeInHierarchy) return;
        itemInfo.SetActive(false);
    }

    public void UpdateAmount(int newVal) 
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text = newVal.ToString();
    }

    public void ActionCallback()//if it is a weapon call equip, if it is a consumable call consume, if it is an armour call equip/wear
    {
        //slots reappear on inventory tab after being moved to weapon or armour slots, when inventory is reopened
        Debug.Log("action button is pressed");
        if (itemInstance != null)
        {
            if (itemDescriptor.itemType == ItemType.Weapon)
            {
                inventoryController.UnequipWeapon(itemInstance);
            }
            else if (itemDescriptor.itemType == ItemType.Shield)
            {
                inventoryController.UnequipShield(itemInstance);
            }
            else if (itemDescriptor.itemType == ItemType.Armour)
            {
                inventoryController.UnequipArmour(itemInstance.GetComponent<Armour>());
            }
            InventoryUI.Instance.AddSlot(gameObject);

            RemoveButton.gameObject.SetActive(true);//reactivate remove button
            Amount.gameObject.SetActive(true);//reactivate amount

            return;
        }
        else
        {
            //if an item of the same type is already equipped bring back the occupying slot first

            InventoryUI.Instance.slots.Remove(this);//move the slot to corresponding equipped item panel
            RemoveButton.gameObject.SetActive(false);//deactivate remove button
            Amount.gameObject.SetActive(false);//deactivate amount

            if (itemDescriptor.itemType == ItemType.Weapon)
            {
                GameObject weapon = Instantiate(Resources.Load(itemDescriptor.itemPrefabPath + itemDescriptor.itemName)) as GameObject;
                weapon.GetComponent<InventoryItem>().SetOwnerReference(inventoryController.gameObject);
                weapon.GetComponentInChildren<Rigidbody>().isKinematic = true;
                weapon.GetComponentInChildren<BoxCollider>().isTrigger = true;
                inventoryController.EquipWeapon(weapon.GetComponent<Weapon>());
                itemInstance = weapon;

                if (weapon.GetComponent<IOneHanded>() != null)
                {
                    if (InventoryUI.Instance.WeaponSlotsParent.GetChild(0).childCount != 0)
                    {
                        InventoryUI.Instance.AddSlot(InventoryUI.Instance.WeaponSlotsParent.GetChild(0).GetChild(0).gameObject);
                    }
                    gameObject.transform.SetParent(InventoryUI.Instance.WeaponSlotsParent.GetChild(0), false);
                }
                else if (weapon.GetComponent<ITwoHanded>() != null)
                {
                    if (InventoryUI.Instance.WeaponSlotsParent.GetChild(1).childCount != 0)
                    {
                        InventoryUI.Instance.AddSlot(InventoryUI.Instance.WeaponSlotsParent.GetChild(1).GetChild(0).gameObject);
                    }
                    gameObject.transform.SetParent(InventoryUI.Instance.WeaponSlotsParent.GetChild(1), false);
                }
                else if (weapon.GetComponent<RangedWeapon>() != null)
                {
                    if (InventoryUI.Instance.WeaponSlotsParent.GetChild(2).childCount != 0)
                    {
                        InventoryUI.Instance.AddSlot(InventoryUI.Instance.WeaponSlotsParent.GetChild(2).GetChild(0).gameObject);
                    }
                    gameObject.transform.SetParent(InventoryUI.Instance.WeaponSlotsParent.GetChild(2), false);
                }
            }
            else if (itemDescriptor.itemType == ItemType.Shield)
            {
                GameObject shield = Instantiate(Resources.Load(itemDescriptor.itemPrefabPath + itemDescriptor.itemName)) as GameObject;
                shield.GetComponent<InventoryItem>().SetOwnerReference(inventoryController.gameObject);
                shield.GetComponentInChildren<Rigidbody>().isKinematic = true;
                shield.GetComponentInChildren<BoxCollider>().isTrigger = true;
                inventoryController.EquipShield(shield.GetComponent<Shield>());
                itemInstance = shield;
                
                if (InventoryUI.Instance.WeaponSlotsParent.GetChild(3).childCount != 0)
                {
                    InventoryUI.Instance.AddSlot(InventoryUI.Instance.WeaponSlotsParent.GetChild(3).GetChild(0).gameObject);
                }
                gameObject.transform.SetParent(InventoryUI.Instance.WeaponSlotsParent.GetChild(3), false);
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

                ArmourType type = armour.GetComponent<Armour>().GetArmourType();
                switch (type)//move to armour slot
                {
                    case ArmourType.Head:
                        if (InventoryUI.Instance.ArmourSlotsParent.GetChild(0).childCount != 0)
                        {
                            InventoryUI.Instance.AddSlot(InventoryUI.Instance.ArmourSlotsParent.GetChild(0).GetChild(0).gameObject);
                        }
                        gameObject.transform.SetParent(InventoryUI.Instance.ArmourSlotsParent.GetChild(0), false);
                        break;
                    case ArmourType.Torso:
                        if (InventoryUI.Instance.ArmourSlotsParent.GetChild(1).childCount != 0)
                        {
                            InventoryUI.Instance.AddSlot(InventoryUI.Instance.ArmourSlotsParent.GetChild(1).GetChild(0).gameObject);
                        }
                        gameObject.transform.SetParent(InventoryUI.Instance.ArmourSlotsParent.GetChild(1), false);
                        break;
                    case ArmourType.Hands:
                        if (InventoryUI.Instance.ArmourSlotsParent.GetChild(2).childCount != 0)
                        {
                            InventoryUI.Instance.AddSlot(InventoryUI.Instance.ArmourSlotsParent.GetChild(2).GetChild(0).gameObject);
                        }
                        gameObject.transform.SetParent(InventoryUI.Instance.ArmourSlotsParent.GetChild(2), false);
                        break;
                    case ArmourType.Legs:
                        if (InventoryUI.Instance.ArmourSlotsParent.GetChild(3).childCount != 0)
                        {
                            InventoryUI.Instance.AddSlot(InventoryUI.Instance.ArmourSlotsParent.GetChild(3).GetChild(0).gameObject);
                        }
                        gameObject.transform.SetParent(InventoryUI.Instance.ArmourSlotsParent.GetChild(3), false);
                        break;
                    case ArmourType.Feet:
                        if (InventoryUI.Instance.ArmourSlotsParent.GetChild(4).childCount != 0)
                        {
                            InventoryUI.Instance.AddSlot(InventoryUI.Instance.ArmourSlotsParent.GetChild(4).GetChild(0).gameObject);
                        }
                        gameObject.transform.SetParent(InventoryUI.Instance.ArmourSlotsParent.GetChild(4), false);
                        break; 
                }
            }
            else if (itemDescriptor.itemType == ItemType.Consumable)
            {
                //fill
            }
            gameObject.transform.localPosition = Vector3.zero;
            InventoryUI.Instance.ArrangeSlotPositions();
        }
    }

    public void RemoveCallback()
    {
        //what will happen to equipped items

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
            Destroy(gameObject, 0.1f);
        }
        else
        {
            inventoryController.DropUninstantiatedSlot(itemDescriptor);
            inventoryController.RemoveSlot(itemDescriptor);//this won't work for equipped items because their slots are already removed from inventoryui.slots
        }
        
        
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
