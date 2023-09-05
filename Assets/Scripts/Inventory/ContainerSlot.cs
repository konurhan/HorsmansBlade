using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContainerSlot : MonoBehaviour
{
    Transform BgImage;
    Button RemoveButton;
    Transform Amount;
    Transform ItemImage;
    Button ActionButton;

    ItemContainer itemContainer;
    public ItemDescriptor itemDescriptor;
    public GameObject itemInstance;

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

    public void Initialize(int amount, ItemContainer container, ItemDescriptor itemDesc)
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text = amount.ToString();
        ItemImage.gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/ItemSprites/" + itemDesc.itemName);
        Debug.Log("Trying to load sprite named: " + itemDesc.itemName);
        itemContainer = container;
        itemDescriptor = itemDesc;
    }

    public void UpdateAmount(int newVal)
    {
        Amount.gameObject.GetComponent<TextMeshProUGUI>().text = newVal.ToString();
    }

    #region Button clilck events

    public void OnMainButtonClicked()
    {
        itemContainer.TakeItemFromContainer(itemDescriptor);
        //InventoryUI.Instance.Player.GetComponent<InventoryController>().AddItemByDescriptor(itemDescriptor);
        foreach (var itemDesc in WorldInventory.Instance.worldItemDescriptions)
        {
            Debug.Log(itemDesc.itemName);
        }
    }

    #endregion
}
