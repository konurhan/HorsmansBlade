using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InventoryController : MonoBehaviour
{
    public float collectRadius;

    public Dictionary<string,List<InventoryItem>> items = new Dictionary<string, List<InventoryItem>>();
    public PlayerController player;
    public List<InventoryItem> collectibleItems;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    void Start()
    {
        LoadInventoryTest();
    }

    public void EquipWeapon(Weapon weapon)//call this method from button click callback
    {
        if (weapon.gameObject.GetComponent<IOneHanded>() != null)
        {
            GetComponent<PlayerAttack>().meleeWeaponOneHanded = weapon.gameObject;
            weapon.gameObject.transform.SetParent(gameObject.GetComponent<PlayerAttack>().MeleeSheatTransform,false);
            weapon.gameObject.transform.localPosition = Vector3.zero;
            weapon.gameObject.transform.localEulerAngles = Vector3.zero;
            weapon.gameObject.SetActive(true);
        }
        else if (weapon.gameObject.GetComponent<ITwoHanded>() != null)
        {
            GetComponent<PlayerAttack>().meleeWeaponTwoHanded = weapon.gameObject;
        }
        else if (weapon.gameObject.GetComponent<RangedWeapon>() != null)
        {
            GetComponent<PlayerAttack>().rangedWeapon = weapon.gameObject;
        }
    }

    public void EquipShield(Shield shield)
    {
        GetComponent<PlayerAttack>().shield = shield.gameObject;
        shield.gameObject.transform.SetParent(gameObject.GetComponent<PlayerAttack>().ShieldHandTransform, false);
        //shield.gameObject.SetActive(true);
    }

    public void EquipArmour(Armour armour)
    {
        //implement
    }

    public void DropAllSlotItems(string itemName)
    {
        List<InventoryItem> toBeRemoved = items[itemName];
        Vector3 dropOffset = gameObject.transform.forward * 2f + Vector3.up * 1.5f;
        foreach (InventoryItem item in toBeRemoved)
        {
            item.SetOwnerReference(null);

            item.gameObject.transform.SetParent(null, true);
            item.gameObject.SetActive(true);

            item.gameObject.transform.position += dropOffset;

            item.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = false;
            item.gameObject.GetComponentInChildren<BoxCollider>().isTrigger = false;
            //item.AddComponent<Rigidbody>();
        }
        items.Remove(itemName);
    }

    public void RemoveSlot(string itemName)
    {
        InventoryUI.Instance.DestroySlot(itemName);
    }

    public void AddSlot(int amount, string itemName, InventoryItem item)
    {
        InventoryUI.Instance.AddSlot(amount, itemName, item);
    }

    public void RemoveSingleItem(string itemName)
    {
        InventoryItem removed = items[itemName][0];
        removed.SetOwnerReference(null);
        items[itemName].RemoveAt(0);

        if (items[itemName].Count == 0)
        {
            //destroy slot
            InventoryUI.Instance.DestroySlot(itemName);
        }
        else
        {
            //update ui
            InventoryUI.Instance.UpdateSlotAmount(items[itemName].Count, itemName);
        }
        removed.transform.SetParent(null, false);
    }

    public void CollectSingleItem(GameObject item)
    {
        //check weight limitation
        //trigger gathering animation
        if(item.GetComponent<InventoryItem>() != null)
        {
            InventoryItem invItem = item.GetComponent<InventoryItem>();
            invItem.SetOwnerReference(gameObject);

            if (items.ContainsKey(invItem.Name))
            {
                items[invItem.Name].Add(invItem);
                InventoryUI.Instance.UpdateSlotAmount(items[invItem.Name].Count, invItem.Name);
            }
            else
            {
                //create a slot
                items.Add(invItem.Name, new List<InventoryItem> { invItem});
                InventoryUI.Instance.AddSlot(1, invItem.Name, invItem);
            }
        }
        if (item.GetComponent<Weapon>() != null)
        {
            item.GetComponent<Weapon>().SetOwnerReference(gameObject);
            item.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
            item.gameObject.GetComponentInChildren<BoxCollider>().isTrigger = true;
        }
        else if (item.GetComponent<Shield>() != null)
        {
            item.GetComponent<Shield>().SetOwnerReference(gameObject);
            item.gameObject.GetComponentInChildren<Rigidbody>().isKinematic = true;
            item.gameObject.GetComponentInChildren<BoxCollider>().isTrigger = true;
        }
        item.transform.SetParent(transform, false);
        item.SetActive(false);
    }

    public void LoadInventory(Dictionary<string, List<InventoryItem>> items)
    {
        //implement after saveload system
        this.items = items;

    }

    public void LoadInventoryTest()//assign longsword to inventory
    {
        //Transform MeleeHandTransform = gameObject.GetComponent<PlayerAttack>().MeleeHandTransform;
        GameObject longSword;
        longSword = Instantiate(Resources.Load("Prefabs/LongSword")) as GameObject;//not assigned to and parented by a sheat transform
        longSword.SetActive(false);
        longSword.GetComponent<InventoryItem>().SetOwnerReference(gameObject);
        longSword.GetComponent<MeleeWeaponDamageController>().SetOwnerReference(gameObject);//take this from weapon object attached
        InventoryItem invItem1 = longSword.GetComponent<InventoryItem>();
        InventoryUI.Instance.AddSlot(1, invItem1.Name, invItem1);

        GameObject shield;
        shield = Instantiate(Resources.Load("Prefabs/Shield")) as GameObject;//not assigned to and parented by a sheat transform
        shield.SetActive(false);
        shield.GetComponent<InventoryItem>().SetOwnerReference(gameObject);
        InventoryItem invItem2 = shield.GetComponent<InventoryItem>();
        InventoryUI.Instance.AddSlot(1, invItem2.Name, invItem2);

        longSword.transform.SetParent(gameObject.transform, false);
        shield.transform.SetParent(gameObject.transform, false);

        items.Add(invItem1.Name, new List<InventoryItem>{ invItem1});
        items.Add(invItem2.Name, new List<InventoryItem> { invItem2 });
    }

}
