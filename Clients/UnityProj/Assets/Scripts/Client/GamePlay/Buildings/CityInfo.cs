using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ShapedInventory;
using UnityEngine.Events;

[Serializable]
public class CityInfo : IClone<CityInfo>
{
    public CityConfig CityConfig;
    public SortedDictionary<uint, BuildingInfo> BuildingInfoDict = new SortedDictionary<uint, BuildingInfo>();
    public UnityAction<BuildingInfo, GridPosR> OnAddBuildingInfoSuc;
    public UnityAction<CityInfo> OnRemoveCityInfoSuc;
    public CityEditorInventory CityEditorInventory;

    public CityInfo(CityConfig cityConfig)
    {
        CityConfig = cityConfig;
    }

    public CityInfo Clone()
    {
        CityInfo cityInfo = new CityInfo(CityConfig.Clone());
        foreach (KeyValuePair<uint, BuildingInfo> kv in BuildingInfoDict)
        {
            AddBuildingInfo(kv.Value.Clone(), kv.Value.InventoryItem.GridPos_Matrix);
        }

        return cityInfo;
    }

    public bool AddBuildingInfo(BuildingInfo bi, GridPosR gp_matrix)
    {
        bi.CityInfo = this;
        bi.OnRemoveBuildingInfoSuc += RemoveBuildingInfo;
        BuildingInfoDict.Add(bi.GUID, bi);

        InventoryItem item = new InventoryItem(bi, CityEditorInventory, gp_matrix);
        item.AmIRootItemInIsolationCalculationHandler = () => true;
        bi.SetInventoryItem(item);

        bool instantiateBuilding()
        {
            item.Inventory = CityEditorInventory;
            OnAddBuildingInfoSuc.Invoke(bi, gp_matrix);
            if (CityEditorInventory.TryAddItem(item))
            {
                CityEditorInventory.RefreshConflictAndIsolation();
                return true;
            }
            else
            {
                bi.RemoveBuildingInfo();
                return false;
            }
        }

        if (CityEditorInventory != null)
        {
            return instantiateBuilding();
        }
        else
        {
            OnInstantiated += () => instantiateBuilding();
            return true;
        }
    }

    public UnityAction OnInstantiated;

    private void RemoveBuildingInfo(BuildingInfo bi)
    {
        CityEditorInventory.RemoveItem(bi.InventoryItem, false);
        CityEditorInventory.RefreshConflictAndIsolation(out List<InventoryItem> _, out List<InventoryItem> isolatedItems);
        BuildingInfoDict.Remove(bi.GUID);
        bi.OnRemoveBuildingInfoSuc = null;
        bi.CityInfo = null;
    }

    public void RemoveCityInfo()
    {
        OnRemoveCityInfoSuc?.Invoke(this);
    }
}