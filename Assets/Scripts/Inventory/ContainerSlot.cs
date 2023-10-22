using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ContainerSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Transform BgImage;
    Button RemoveButton;
    Transform Amount;
    Transform ItemImage;
    Button ActionButton;

    ItemContainer itemContainer;
    public ItemDescriptor itemDescriptor;
    public GameObject itemInstance;

    public GameObject itemInfo;

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

    private void OnDestroy()
    {
        if (itemInfo != null)
        {
            if (!itemInfo.activeInHierarchy) return;
            itemInfo.SetActive(false);
            Destroy(itemInfo);
        }
    }

    public void SetupItemInfo()
    {
        itemInfo.transform.position = gameObject.transform.position + new Vector3(155, 50, 0);

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

    public void Initialize(int amount, ItemContainer container, ItemDescriptor itemDesc)
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text = amount.ToString();
        ItemImage.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ItemSprites/" + itemDesc.itemName);
        Debug.Log("Trying to load sprite named: " + itemDesc.itemName);
        itemContainer = container;
        itemDescriptor = itemDesc;

        AddToWorldInventory();
    }

    private void AddToWorldInventory()
    {
        if (itemContainer.FindDescriptionInWorldInventory(itemDescriptor.itemName) == null)
        {
            WorldInventory.Instance.AddNewItemDescriptor(itemDescriptor);
        }
    }

    public void UpdateAmount(int newVal)
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text = newVal.ToString();
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

    #region Button clilck events

    public void OnMainButtonClicked()
    {
        itemContainer.TakeItemFromContainer(itemDescriptor);
        //InventoryUI.Instance.Player.GetComponent<InventoryController>().AddItemByDescriptor(itemDescriptor);
        /*foreach (var itemDesc in WorldInventory.Instance.worldItemDescriptions)
        {
            Debug.Log(itemDesc.itemName);
        }*/
    }

    #endregion
}
