using System;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.AdvancedInventory;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class BuildingInfo : IInventoryItemContentInfo
{
    [ReadOnly]
    [HideInEditorMode]
    public uint GUID;

    private static uint guidGenerator = (uint) ConfigManager.GUID_Separator.BuildingInfo;

    private uint GetGUID()
    {
        return guidGenerator++;
    }

    [HideInEditorMode]
    public InventoryItem InventoryItem;

    private BuildingOriginalOccupiedGridInfo BuildingOriginalOccupiedGridInfo;

    #region IInventoryItemContentInfo

    public List<GridPos> IInventoryItemContentInfo_OriginalOccupiedGridPositions => BuildingOriginalOccupiedGridInfo.BuildingOccupiedGridPositionList;

    public string ItemCategoryName => BuildingConfig.BuildingType.ToString();
    public string ItemName => BuildingConfig.EnglishName;
    public string ItemQuality => "";
    public string ItemBasicInfo => BuildingConfig.ItemBasicInfo;
    public string ItemDetailedInfo => BuildingConfig.ItemDetailedInfo;
    public Sprite ItemSprite => null;
    public string ItemSpriteKey => BuildingConfig.ItemSpriteKey;
    public Color ItemColor => Color.white;

    #endregion

    public BuildingConfig BuildingConfig;

    public UnityAction<BuildingInfo> OnRemoveBuildingInfoSuc;

    [NonSerialized]
    public CityInfo CityInfo;

    public BuildingInfo(BuildingConfig buildingConfig)
    {
        GUID = GetGUID();
        BuildingConfig = buildingConfig;
        if (ConfigManager.BuildingOriginalOccupiedGridInfoDict.TryGetValue(BuildingConfig.BuildingKey, out BuildingOriginalOccupiedGridInfo info))
        {
            BuildingOriginalOccupiedGridInfo = info.Clone();
        }
    }

    public void Reset()
    {
        OnRemoveBuildingInfoSuc = null;
    }

    public BuildingInfo Clone()
    {
        BuildingInfo bi = new BuildingInfo(BuildingConfig);
        return bi;
    }

    public void RemoveBuildingInfo()
    {
        OnRemoveBuildingInfoSuc?.Invoke(this);
    }

    public void SetInventoryItem(InventoryItem inventoryItem)
    {
        InventoryItem = inventoryItem;
    }
}