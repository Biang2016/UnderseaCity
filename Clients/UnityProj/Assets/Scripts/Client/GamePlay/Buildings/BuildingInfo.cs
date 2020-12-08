using System;
using System.Collections.Generic;
using BiangStudio;
using BiangStudio.AdvancedInventory;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;
using UnityEngine.Serialization;

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

    public int OxygenCapacity = 0;
    public int PopulationCapacity = 0;

    public int BudgetProduction = 0;
    public int PeopleDemandForBudgetProduction = 10;
    public int OxygenProduction = 0;

    public int BudgetConsumption = 0;
    public int OxygenConsumption = 0;

    public bool NeedAllOccupationValid = false;

    public List<GridPos> BuildingOccupiedGridPositionList = new List<GridPos>();
    public BuildingType buildingType;
    public string englishName;
    public string chineseName;
    public string itemBasicInfo;

    [FormerlySerializedAs("itemDetailedInfo")]
    public string itemDetailedInfo_Requires;

    public string itemDetailedInfo_Provides;
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
        newBuildingInfo.OxygenCapacity = OxygenCapacity;
        newBuildingInfo.PopulationCapacity = PopulationCapacity;
        newBuildingInfo.PeopleDemandForBudgetProduction = PeopleDemandForBudgetProduction;
        newBuildingInfo.BudgetProduction = BudgetProduction;
        newBuildingInfo.OxygenProduction = OxygenProduction;
        newBuildingInfo.BudgetConsumption = BudgetConsumption;
        newBuildingInfo.OxygenConsumption = OxygenConsumption;

        newBuildingInfo.NeedAllOccupationValid = NeedAllOccupationValid;

        newBuildingInfo.BuildingOccupiedGridPositionList = BuildingOccupiedGridPositionList.Clone();
        newBuildingInfo.buildingType = buildingType;
        newBuildingInfo.englishName = englishName;
        newBuildingInfo.chineseName = chineseName;
        newBuildingInfo.itemBasicInfo = itemBasicInfo;
        newBuildingInfo.itemDetailedInfo_Requires = itemDetailedInfo_Requires;
        newBuildingInfo.itemDetailedInfo_Provides = itemDetailedInfo_Provides;
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
    public string ItemDetailedInfo => itemDetailedInfo_Requires;
    public Sprite ItemSprite => itemSprite;
    public Color ItemColor => color;

    #endregion

    public string ItemDetailedInfo_Requires =>
        "Requires: "
        + CommonUtils.AddHighLightColorToText(itemDetailedInfo_Requires, "#FFDD00") + "\n"
        + (BudgetCost > 0 ? "\t\tBudget: " + CommonUtils.AddHighLightColorToText(BudgetCost.ToString(), "#FFAB00") : "")
        + (OxygenCost > 0 ? "\t\tOxygen: " + CommonUtils.AddHighLightColorToText(OxygenCost.ToString(), "#00F4FF") : "") + "\n"
        + ((BudgetConsumption > 0 || OxygenConsumption > 0) ? "Consumes: " : "")
        + (BudgetConsumption > 0 ? "\t\t" + CommonUtils.AddHighLightColorToText("-" + BudgetConsumption + " per day", "#FFAB00") : "")
        + (OxygenConsumption > 0 ? "\t\t" + CommonUtils.AddHighLightColorToText("-" + OxygenConsumption + " per day", "#00F4FF") : "");

    public string ItemDetailedInfo_Provides =>
        "Provides: "
        + CommonUtils.AddHighLightColorToText(itemDetailedInfo_Provides, "#FFDD00") + "\n"
        + (BudgetProduction > 0 ? "\t\t" + CommonUtils.AddHighLightColorToText("+" + BudgetProduction + " per golden grid", "#FFAB00") : "")
        + (OxygenProduction > 0 ? "\t\t" + CommonUtils.AddHighLightColorToText("+" + OxygenProduction + " per day", "#00F4FF") : "")
        + (OxygenCapacity > 0 ? "\t\tOxygen Capacity: " + CommonUtils.AddHighLightColorToText("+" + OxygenCapacity, "#0076FF") : "")
        + (PopulationCapacity > 0 ? "\t\tPopulation Capacity: " + CommonUtils.AddHighLightColorToText("+" + PopulationCapacity, "#10D100") : "");
}