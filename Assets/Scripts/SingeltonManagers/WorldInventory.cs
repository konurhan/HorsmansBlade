using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class WorldInventory : MonoBehaviour
{
    public static WorldInventory Instance;

    public List<ItemDescriptor> worldItemDescriptions;
    public List<ItemContainer> worldItemContainers;//each item container register itself to this list in their awake

    private void Awake()
    {
        Instance = this;
        worldItemDescriptions = new List<ItemDescriptor>();
        LoadWorldInventory();
    }
    
    void Start()
    {
        InventoryUI.Instance.onGameSaved += SaveWorldInventory;
        InventoryUI.Instance.onGameSaved += SaveContainers;

        LoadContainers();
    }

    public void AddNewItemDescriptor(ItemDescriptor itemDescriptor)
    {
        worldItemDescriptions.Add(itemDescriptor);
    }

    #region save/load methods
    public void LoadWorldInventory()
    {
        WorldInventoryData worldInventoryData = new WorldInventoryData();
        worldInventoryData = SaveSystem.LoadData<WorldInventoryData>("/WorldInventoryData.json");
        if (worldInventoryData == null) return;

        worldItemDescriptions = worldInventoryData.worldItemDescriptions;
    }

    public void SaveWorldInventory()
    {
        WorldInventoryData worldInventoryData = new WorldInventoryData();
        worldInventoryData.worldItemDescriptions = worldItemDescriptions;

        SaveSystem.SaveData("/WorldInventoryData.json", worldInventoryData);
    }

    public void LoadContainers()
    {
        ContainerDataCollection containerDataCollection;
        containerDataCollection = SaveSystem.LoadData<ContainerDataCollection>("/WorldContainerDataCollection.json");

        if (containerDataCollection == null) return;//if save file doesn't exist

        foreach (ContainerData data in containerDataCollection.worldContainers) 
        {
            for (int i = 0; i < worldItemContainers.Count; i++)
            {
                if (data.containerId == worldItemContainers[i].containerID)
                {
                    worldItemContainers[i].containedItems = data.containedItems.ToDictionary(x => x.Key, x => x.Value);
#if UNITY_EDITOR
                    worldItemContainers[i].SerializeContainedItems();
#endif
                    break;
                }
            }
        }
    }

    public void SaveContainers()
    {
        ContainerDataCollection containerDataCollection = new ContainerDataCollection();

        foreach (ItemContainer container in worldItemContainers)
        {
            ContainerData data = new ContainerData(container);
            containerDataCollection.worldContainers.Add(data);
        }

        SaveSystem.SaveData("/WorldContainerDataCollection.json", containerDataCollection);
    }
    #endregion
}

[System.Serializable]
public class WorldInventoryData
{
    public List<ItemDescriptor> worldItemDescriptions = new List<ItemDescriptor>();
}

[System.Serializable]
public class ContainerDataCollection
{
    public List<ContainerData> worldContainers;

    public ContainerDataCollection()
    {
        worldContainers = new List<ContainerData>();
    }
}

