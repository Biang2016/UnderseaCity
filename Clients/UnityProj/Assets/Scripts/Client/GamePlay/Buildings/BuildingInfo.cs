using System;
using System.Collections.Generic;
using BiangStudio.AdvancedInventory;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

[Serializable]
public class BuildingInfo : IClone<BuildingInfo>, IInventoryItemContentInfo
{
    public BuildingKey buildingKey;
    public List<BuildingCoverMaskGroup> requireBuildingCoverList = new List<BuildingCoverMaskGroup>();
    public BuildingCoverMaskGroup forbidBuildingCover;
    public BuildingCoverMaskGroup provideBuildingCoverByOccupation;
    public BuildingCoverMaskGroup provideBuildingCoverInnerRange;
    public float provideBuildingCoverInnerRange_Radius;
    public BuildingCoverMaskGroup provideBuildingCoverOuterRange;
    public float provideBuildingCoverOuterRange_Radius;

    public int BudgetCost = 100;
    public int OxygenCost = 100;

    public List<GridPos> BuildingOccupiedGridPositionList = new List<GridPos>();
    public BuildingType buildingType;
    public string englishName;
    public string chineseName;
    public string itemBasicInfo;
    public string itemDetailedInfo;
    public Sprite itemSprite;
    public Color color;

    public BuildingInfo Clone()
    {
        BuildingInfo newBuildingInfo = new BuildingInfo();
        newBuildingInfo.buildingKey = buildingKey;
        newBuildingInfo.requireBuildingCoverList = requireBuildingCoverList.Clone();
        newBuildingInfo.forbidBuildingCover = forbidBuildingCover;
        newBuildingInfo.provideBuildingCoverByOccupation = provideBuildingCoverByOccupation;
        newBuildingInfo.provideBuildingCoverInnerRange = provideBuildingCoverInnerRange;
        newBuildingInfo.provideBuildingCoverInnerRange_Radius = provideBuildingCoverInnerRange_Radius;
        newBuildingInfo.provideBuildingCoverOuterRange = provideBuildingCoverOuterRange;
        newBuildingInfo.provideBuildingCoverOuterRange_Radius = provideBuildingCoverOuterRange_Radius;

        newBuildingInfo.BudgetCost = BudgetCost;
        newBuildingInfo.OxygenCost = OxygenCost;

        newBuildingInfo.BuildingOccupiedGridPositionList = BuildingOccupiedGridPositionList.Clone();
        newBuildingInfo.buildingType = buildingType;
        newBuildingInfo.englishName = englishName;
        newBuildingInfo.chineseName = chineseName;
        newBuildingInfo.itemBasicInfo = itemBasicInfo;
        newBuildingInfo.itemDetailedInfo = itemDetailedInfo;
        newBuildingInfo.itemSprite = itemSprite;
        newBuildingInfo.color = color;
        return newBuildingInfo;
    }

    #region IInventoryItemContentInfo

    public List<GridPos> IInventoryItemContentInfo_OriginalOccupiedGridPositions => BuildingOccupiedGridPositionList;
    public string ItemCategoryName => buildingType.ToString();
    public string ItemName => englishName;
    public string ItemQuality => "None";
    public string ItemBasicInfo => itemBasicInfo;
    public string ItemDetailedInfo => itemDetailedInfo;
    public Sprite ItemSprite => itemSprite;
    public Color ItemColor => color;

    #endregion
}