using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Description of an InventoryItem GameObject, items which are not instantiated are of this type
public class ItemDescriptor
{
    public string itemName;//same as the InventoryItem.Name and the Prefab name
    public string itemPrefabPath;//relative path to Resources path
    public ItemType itemType;

    //public bool isInstantiated;

    public ItemDescriptor()
    {
        //isInstantiated = false;
    }

    public ItemDescriptor(string name, string path, ItemType type)
    {
        itemName = name;
        itemPrefabPath = path;
        itemType = type;
        //isInstantiated = false;
    }
}

public enum ItemType
{
    Weapon,
    Ammo,
    Shield,
    Armour,
    Consumable,
    Generic
}

