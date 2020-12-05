using System;
using System.Collections.Generic;
using BiangStudio.CloneVariant;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.AdvancedInventory;
using UnityEngine.Events;

[Serializable]
public class CityInfo : IClone<CityInfo>
{
    public CityConfig CityConfig;
    public SortedDictionary<uint, BuildingInfo> BuildingInfoDict = new SortedDictionary<uint, BuildingInfo>();
    public UnityAction<BuildingInfo, GridPosR> OnAddBuildingInfoSuc;
    public UnityAction<CityInfo> OnRemoveCityInfoSuc;
    public CityInventory CityInventory;

    public CityInfo(CityConfig cityConfig)
    {
        CityConfig = cityConfig;
    }

    public CityInfo Clone()
    {
        CityInfo cityInfo = new CityInfo(CityConfig.Clone());
        foreach (KeyValuePair<uint, BuildingInfo> kv in BuildingInfoDict)
        {
            AddBuildingInfo(kv.Value.Clone(), kv.Value.InventoryItem.GridPos_Matrix, false);
        }

        return cityInfo;
    }

    public bool AddBuildingInfo(BuildingInfo bi, GridPosR gp_matrix, bool isPreview)
    {
        bi.CityInfo = this;
        bi.OnRemoveBuildingInfoSuc += RemoveBuildingInfo;
        BuildingInfoDict.Add(bi.GUID, bi);

        InventoryItem item = new InventoryItem(bi, CityInventory, gp_matrix);
        item.AmIRootItemInIsolationCalculationHandler = () => true;
        bi.SetInventoryItem(item);

        bool instantiateBuilding()
        {
            item.Inventory = CityInventory;
            OnAddBuildingInfoSuc.Invoke(bi, gp_matrix);
            if (isPreview)
            {
                CityInventory.AddPreviewItem(item);
                CityInventory.RefreshConflictAndIsolation();
                return true;
            }
            else
            {
                if (CityInventory.TryAddItem(item))
                {
                    CityInventory.RefreshConflictAndIsolation();
                    return true;
                }
                else
                {
                    bi.RemoveBuildingInfo();
                    return false;
                }
            }
        }

        if (CityInventory != null)
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
        CityInventory.RemoveItem(bi.InventoryItem, false);
        CityInventory.RefreshConflictAndIsolation(out List<InventoryItem> _, out List<InventoryItem> isolatedItems);
        BuildingInfoDict.Remove(bi.GUID);
        bi.OnRemoveBuildingInfoSuc = null;
        bi.CityInfo = null;
    }

    public void RemoveCityInfo()
    {
        OnRemoveCityInfoSuc?.Invoke(this);
    }
}