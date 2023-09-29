using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

public class InventoryController : MonoBehaviour
{
    public float collectRadius;

    public Dictionary<ItemDescriptor,int> items = new Dictionary<ItemDescriptor,int>();

    public PlayerController player;
    public List<InventoryItem> collectibleItems;
    public List<ItemContainer> lootableContainers;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    void Start()
    {
        //LoadInventoryTest();
        LoadInventory();

        InventoryUI.Instance.onGameSaved += SaveInventory;
    }

    public void EquipWeapon(Weapon weapon)//called from slot action button click callback
    {
        if (weapon.gameObject.GetComponent<IOneHanded>() != null)
        {
            if (GetComponent<PlayerAttack>().meleeWeaponOneHanded != null)//if already equipped a same type of weapon
            {
                UnequipWeapon(GetComponent<PlayerAttack>().meleeWeaponOneHanded);//first destroy that GameObject and mark on inventory as uninstantiated
            }
            GetComponent<PlayerAttack>().meleeWeaponOneHanded = weapon.gameObject;
            weapon.gameObject.transform.SetParent(gameObject.GetComponent<PlayerAttack>().MeleeSheatTransform,false);
            weapon.gameObject.transform.localPosition = Vector3.zero;
            weapon.gameObject.transform.localEulerAngles = Vector3.zero;
            weapon.gameObject.SetActive(true);
        }
        else if (weapon.gameObject.GetComponent<ITwoHanded>() != null)
        {
            //if (GetComponent<PlayerAttack>().meleeWeaponTwoHanded.GetComponent<Weapon>() == weapon) return;//no need same item wont be instantiated twice
            if (GetComponent<PlayerAttack>().meleeWeaponTwoHanded != null)//if already equipped a same type of weapon
            {
                UnequipWeapon(GetComponent<PlayerAttack>().meleeWeaponTwoHanded);//first destroy that GameObject and mark on inventory as uninstantiated
            }
            GetComponent<PlayerAttack>().meleeWeaponTwoHanded = weapon.gameObject;
            weapon.gameObject.transform.SetParent(gameObject.GetComponent<PlayerAttack>().MeleeSheatTransform, false);
            weapon.gameObject.transform.localPosition = Vector3.zero;
            weapon.gameObject.transform.localEulerAngles = Vector3.zero;
            weapon.gameObject.SetActive(true);
        }
        else if (weapon.gameObject.GetComponent<RangedWeapon>() != null)
        {
            if (GetComponent<PlayerAttack>().rangedWeapon != null)//if already equipped a same type of weapon
            {
                UnequipWeapon(GetComponent<PlayerAttack>().rangedWeapon);//first destroy that GameObject and mark on inventory as uninstantiated
            }
            GetComponent<PlayerAttack>().RangedQuiverTransform.GetChild(0).gameObject.SetActive(true);
            GetComponent<PlayerAttack>().rangedWeapon = weapon.gameObject;
            weapon.gameObject.transform.SetParent(gameObject.GetComponent<PlayerAttack>().RangedSheatTransform, false);
            weapon.gameObject.transform.localPosition = Vector3.zero;
            weapon.gameObject.transform.localEulerAngles = Vector3.zero;
            weapon.gameObject.SetActive(true);
        }

        weapon.OnEquipped();
    }

    public void UnequipWeapon(GameObject weaponObj)
    {
        if (weaponObj.GetComponent<IOneHanded>() != null)
        {
            
        }
        else if (weaponObj.GetComponent<ITwoHanded>() != null)
        {
            
        }
        else if (weaponObj.GetComponent<RangedWeapon>() != null)
        {
            GetComponent<PlayerAttack>().RangedQuiverTransform.GetChild(0).gameObject.SetActive(false);
        }

        Destroy(weaponObj);
    }

    public void EquipShield(Shield shield)
    {
        if(GetComponent<PlayerAttack>().shield != null)
        {
            UnequipShield(GetComponent<PlayerAttack>().shield);
        }
        GetComponent<PlayerAttack>().shield = shield.gameObject;
        shield.gameObject.transform.SetParent(gameObject.GetComponent<PlayerAttack>().ShieldHandTransform, false);
        shield.gameObject.SetActive(false);
    }

    public void UnequipShield(GameObject shieldObj)
    {
        Destroy(shieldObj);
    }

    public void EquipArmour(GameObject armourObj, ItemDescriptor itemDesc)
    {
        Armour armour = armourObj.GetComponent<Armour>();
        ArmourType type = armour.GetArmourType();
        PlayerController controller = GetComponent<PlayerController>();

        switch (type)
        {
            case ArmourType.Head:
                if (controller.ArmourSlots.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipArmour(controller.head.GetComponent<BodyPart>().armour);
                }
                controller.ArmourSlots.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/"+itemDesc.itemName);
                controller.head.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
            case ArmourType.Torso:
                if (controller.ArmourSlots.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipArmour(controller.torso.GetComponent<BodyPart>().armour);
                }
                controller.NakedParts.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.ArmourSlots.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/" + itemDesc.itemName);
                controller.torso.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.leftUpperArm.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.rightUpperArm.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
            case ArmourType.Feet:
                if (controller.ArmourSlots.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipArmour(controller.legLeftLower.GetComponent<BodyPart>().armour);
                }
                controller.NakedParts.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.ArmourSlots.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/" + itemDesc.itemName);
                controller.legLeftLower.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.legRightLower.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
            case ArmourType.Hands:
                if (controller.ArmourSlots.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipArmour(controller.leftForearm.GetComponent<BodyPart>().armour);
                }
                controller.NakedParts.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.ArmourSlots.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/" + itemDesc.itemName);
                controller.leftForearm.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.rightForearm.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
            case ArmourType.Legs:
                if (controller.ArmourSlots.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled)
                {
                    UnequipArmour(controller.legLeftUpper.GetComponent<BodyPart>().armour);
                }
                controller.NakedParts.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.ArmourSlots.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = Resources.Load<Mesh>("Meshes/Armour/" + itemDesc.itemName);
                controller.legLeftUpper.GetComponent<BodyPart>().PutOnArmour(armour);
                controller.legRightUpper.GetComponent<BodyPart>().PutOnArmour(armour);
                break;
        }
    }

    public void UnequipArmour(Armour armour)//also call when an armour is dropped
    {
        ArmourType type = armour.GetArmourType();
        PlayerController controller = GetComponent<PlayerController>();

        switch (type)
        {
            case ArmourType.Head:
                controller.ArmourSlots.GetChild(0).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.head.GetComponent<BodyPart>().RemoveArmour();
                break;
            case ArmourType.Torso:
                controller.NakedParts.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(1).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.torso.GetComponent<BodyPart>().RemoveArmour();
                controller.leftUpperArm.GetComponent<BodyPart>().RemoveArmour();
                controller.rightUpperArm.GetComponent<BodyPart>().RemoveArmour();
                break;
            case ArmourType.Feet:
                controller.NakedParts.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(2).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.legLeftLower.GetComponent<BodyPart>().RemoveArmour();
                controller.legRightLower.GetComponent<BodyPart>().RemoveArmour();
                break;
            case ArmourType.Hands:
                controller.NakedParts.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(3).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.leftForearm.GetComponent<BodyPart>().RemoveArmour();
                controller.rightForearm.GetComponent<BodyPart>().RemoveArmour();
                break;
            case ArmourType.Legs:
                controller.NakedParts.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = true;
                controller.ArmourSlots.GetChild(4).gameObject.GetComponent<SkinnedMeshRenderer>().enabled = false;
                controller.legLeftUpper.GetComponent<BodyPart>().RemoveArmour();
                controller.legRightUpper.GetComponent<BodyPart>().RemoveArmour();
                break;
        }

        Destroy(armour.gameObject);//destroying the object before returning it back to the inventory ==>> BUT HOW TO SAVE CONDITION, SHOULDN'T BE BROUGHT BACK BRAND-NEW !!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }


    public void DropUninstantiatedSlot(ItemDescriptor item)//make non-weapon items drop in a sack: a container
    {
        //don't need to call ondropped since onequipped is never called
        Vector3 dropOffset = gameObject.transform.forward * 0.3f + Vector3.up * 0.3f;
        int amount = items[item];
        for (int i = 0; i < amount; i++)
        {
            GameObject toBeDropped = Instantiate(Resources.Load(item.itemPrefabPath + item.itemName)) as GameObject;
            toBeDropped.transform.position = transform.position + transform.forward + transform.up;
            toBeDropped.GetComponentInChildren<Rigidbody>().isKinematic = false;// so that dropped items can fall to the ground
            if (toBeDropped.GetComponentInChildren<BoxCollider>())
            {
                toBeDropped.GetComponentInChildren<BoxCollider>().isTrigger = false;
            }
            else if (toBeDropped.GetComponentInChildren<MeshCollider>())
            {
                toBeDropped.GetComponentInChildren<MeshCollider>().isTrigger = false;
            }
            toBeDropped.GetComponent<Rigidbody>().AddForce(dropOffset, ForceMode.Impulse);
        }
        items.Remove(item);
    }

    public void DropInstantiatedSlot(ItemDescriptor itemDesc, GameObject itemInstance)
    {
        itemInstance.GetComponent<InventoryItem>().OnDropped();

        Vector3 dropOffset = gameObject.transform.forward * 0.3f + Vector3.up * 0.3f;
        int amount = items[itemDesc];

        GameObject toBeDropped = itemInstance;
        toBeDropped.transform.position = transform.position + transform.forward + transform.up;
        toBeDropped.GetComponentInChildren<Rigidbody>().isKinematic = false;// so that dropped items can fall to the ground
        toBeDropped.GetComponentInChildren<BoxCollider>().isTrigger = false;
        toBeDropped.GetComponent<Rigidbody>().AddForce(dropOffset, ForceMode.Impulse);

        for (int i = 1; i < amount; i++)
        {
            toBeDropped = Instantiate(Resources.Load(itemDesc.itemPrefabPath + itemDesc.itemName)) as GameObject;
            toBeDropped.transform.position = transform.position + transform.up;
            toBeDropped.GetComponentInChildren<Rigidbody>().isKinematic = false;// so that dropped items can fall to the ground
            toBeDropped.GetComponentInChildren<BoxCollider>().isTrigger = false;
            toBeDropped.GetComponent<Rigidbody>().AddForce(dropOffset, ForceMode.Impulse);
        }

        items.Remove(itemDesc);
    }

    public void RemoveSlot(ItemDescriptor itemDesc)
    {
        InventoryUI.Instance.DestroySlot(itemDesc);
    }

    public void AddSlot(int amount, ItemDescriptor itemDesc)
    {
        InventoryUI.Instance.AddSlot(amount, itemDesc);
    }

    public void AddItemByDescriptor(ItemDescriptor itemDesc)
    {
        ItemDescriptor localDesc = FindDescriptionInPlayerInventory(itemDesc.itemName);
        if (localDesc != null)//search by name not the class instance itself
        {
            items[localDesc]++;
            InventoryUI.Instance.UpdateSlotAmount(items[localDesc], localDesc);
        }
        else
        {
            items.Add(itemDesc, 1);
            InventoryUI.Instance.AddSlot(1, itemDesc);
            if (FindDescriptionInWorldInventory(itemDesc.itemName) == null)
            {
                WorldInventory.Instance.worldItemDescriptions.Add(itemDesc);
            }
        }
    }

    public void DropSingleItem(ItemDescriptor item)//will be called from inventory slot button callback --> for uninstantiated items
    {
        items[item]--;
        if (items[item] == 0)
        {
            items.Remove(item);
        }
        Vector3 dropOffset = gameObject.transform.forward * 2f + Vector3.up * 1.5f;
        GameObject toBeDropped = Instantiate(Resources.Load(item.itemPrefabPath + item.itemName)) as GameObject;//only if item is not already instantiated
        toBeDropped.transform.position += dropOffset;
        toBeDropped.GetComponentInChildren<Rigidbody>().isKinematic = false;// so that dropped items can fall to the ground
        toBeDropped.GetComponentInChildren<BoxCollider>().isTrigger = false;
        toBeDropped.GetComponent<InventoryItem>().SetOwnerReference(null);
    }

    public void CollectSingleItem(GameObject gathered)
    {
        //check weight limitation

        //trigger gathering animation


        if(gathered.GetComponent<InventoryItem>() != null)
        {
            gameObject.GetComponent<Animator>().SetTrigger("Gather");//when knee is down collect the item or set active false

            InventoryItem invItem = gathered.GetComponent<InventoryItem>();
            invItem.SetOwnerReference(gameObject);

            ItemDescriptor itemDesc = FindDescriptionInPlayerInventory(invItem.Name);

            if (itemDesc != null)
            {
                items[itemDesc]++;
                InventoryUI.Instance.UpdateSlotAmount(items[itemDesc], itemDesc);
            }
            else
            {
                ItemDescriptor worldItemDesc = FindDescriptionInWorldInventory(invItem.Name);
                /*if (worldItemDesc == null)
                {
                    WorldInventory.Instance.AddNewItemDescriptor()
                }*/
                items.Add(worldItemDesc, 1);
                InventoryUI.Instance.AddSlot(1, worldItemDesc);
            }
        }

        Destroy(gathered,1f);//delaying the destruction so that closest paramater of InventoryUI script doesn't reference a destroyed object 
    }

    public void LoadInventory()
    {
        //implement after saveload system
        //load dict of itemdescriptors

        InventoryData data = SaveSystem.LoadData<InventoryData>("/PlayerInventoryData.json"); 
        if (data == null) return;

        Dictionary<ItemDescriptor,int> loadedDict = new Dictionary<ItemDescriptor,int>();
        items = data.items.ToDictionary(x => x.Key, x=> x.Value);
    }

    public void SaveInventory()
    {
        InventoryData data = new InventoryData(this);

        SaveSystem.SaveData("/PlayerInventoryData.json", data);
    }


    public void LoadInventoryTest()//assign longsword to inventory
    {
        ItemDescriptor longSword = new ItemDescriptor("SimpleLongSword","Prefabs/Weapons/Melee/", ItemType.Weapon);
        items.Add(longSword, 1);

        ItemDescriptor shield = new ItemDescriptor("SimpleShield","Prefabs/Shields/", ItemType.Shield);
        items.Add(shield, 1);

        ItemDescriptor archerBodyArmour = new ItemDescriptor("ArcherBodyArmour", "Prefabs/Armours/", ItemType.Armour);
        items.Add(archerBodyArmour, 1);

        ItemDescriptor knightBodyArmour = new ItemDescriptor("KnightBodyArmour", "Prefabs/Armours/", ItemType.Armour);
        items.Add(knightBodyArmour, 1);

        ItemDescriptor simpleLongBow = new ItemDescriptor("SimpleLongBow", "Prefabs/Weapons/Ranged/", ItemType.Weapon);
        items.Add(simpleLongBow, 1);

        ItemDescriptor longBowArrow = new ItemDescriptor("ArrowLongBow", "Prefabs/Weapons/Ranged/", ItemType.Ammo);
        items.Add(longBowArrow, 15);

        InventoryUI.Instance.AddSlot(1, longSword);
        InventoryUI.Instance.AddSlot(1, shield);
        InventoryUI.Instance.AddSlot(1, archerBodyArmour);
        InventoryUI.Instance.AddSlot(1, knightBodyArmour);
        InventoryUI.Instance.AddSlot(1, simpleLongBow);
        InventoryUI.Instance.AddSlot(15, longBowArrow);

        WorldInventory.Instance.worldItemDescriptions.Add(longSword);
        WorldInventory.Instance.worldItemDescriptions.Add(shield);
        WorldInventory.Instance.worldItemDescriptions.Add(archerBodyArmour);
        WorldInventory.Instance.worldItemDescriptions.Add(knightBodyArmour);
        WorldInventory.Instance.worldItemDescriptions.Add(simpleLongBow);
        WorldInventory.Instance.worldItemDescriptions.Add(longBowArrow);

    }

    public ItemDescriptor FindDescriptionInPlayerInventory(string itemName)
    {
        foreach (ItemDescriptor itemDesc in items.Keys)
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
}

[System.Serializable]
public class InventoryData
{
    public List<KeyValuePair<ItemDescriptor, int>> items;

    public InventoryData()
    {
        items = new List<KeyValuePair<ItemDescriptor, int>>();
    }

    public InventoryData(InventoryController controller)
    {
        items = controller.items.ToList();
    }
}


