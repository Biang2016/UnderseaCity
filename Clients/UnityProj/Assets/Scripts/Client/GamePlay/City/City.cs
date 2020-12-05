using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BiangStudio.GamePlay;
using BiangStudio.ObjectPool;
using UnityEngine.Events;

public class City : PoolObject
{
    public UnityAction<City> OnRemoveCitySuc;

    [SerializeField]
    private Transform BuildingContainer;

    public CityInfo CityInfo;

    public CityEditAreaIndicator CityEditAreaIndicator;
    public SortedDictionary<uint, Building> BuildingDict = new SortedDictionary<uint, Building>();

    public override void OnRecycled()
    {
        Clean();
        if (CityEditAreaIndicator != null)
        {
            DestroyImmediate(CityEditAreaIndicator);
            CityEditAreaIndicator = null;
        }

        base.OnRecycled();
    }

    public void Initialize(CityInfo cityInfo)
    {
        Clean();

        CityInfo = cityInfo;

        CityInfo.OnAddBuildingInfoSuc = (bi, gp_matrix) => AddBuilding(bi);
        CityInfo.OnRemoveCityInfoSuc += (bi) =>
        {
            OnRemoveCitySuc?.Invoke(this);
            PoolRecycle();
        };

        CityInfo.CityInventory = new CityInventory(
            "City",
            CityEditAreaIndicator,
            ConfigManager.GRID_SIZE,
            ConfigManager.EDIT_AREA_FULL_SIZE,
            ConfigManager.EDIT_AREA_FULL_SIZE,
            false,
            false,
            false,
            0,
            false,
            () => Input.GetKeyDown(KeyCode.R));
        CityInfo.CityInventory.OnRemoveItemSucAction = (item) => { ((BuildingInfo) item.ItemContentInfo).RemoveBuildingInfo(); };
        CityInfo.CityInventory.RefreshInventoryGrids();
        CityInfo.OnInstantiated?.Invoke(); // 将已经积攒的未实例化的组件实例化
        CityInfo.OnInstantiated = null;
        CityInfo.CityInventory.RefreshConflictAndIsolation();

        // CityEditorArea
        GameObject cea = PrefabManager.Instance.GetPrefab(nameof(CityEditAreaIndicator));
        GameObject ceaGO = Instantiate(cea);
        ceaGO.transform.parent = transform;
        ceaGO.transform.localPosition = new Vector3(0, -0.5f, 0);
        CityEditAreaIndicator = ceaGO.GetComponent<CityEditAreaIndicator>();
        CityEditAreaIndicator.Init(CityInfo);

        foreach (KeyValuePair<uint, BuildingInfo> kv in CityInfo.BuildingInfoDict) // 将其它的建筑实例化
        {
            if (BuildingDict.ContainsKey(kv.Key)) continue;
            AddBuilding(kv.Value);
        }

        GridShown = false;
    }

    public void Clean()
    {
        CityEditAreaIndicator?.Clear();
        foreach (KeyValuePair<uint, Building> kv in BuildingDict)
        {
            kv.Value.PoolRecycle();
        }

        BuildingDict.Clear();
        OnRemoveCitySuc = null;
        CityInfo = null;
    }

    protected void Update()
    {
        if (!IsRecycled)
        {
            Update_Building();
        }
    }

    void FixedUpdate()
    {
        if (!IsRecycled)
        {
            FixedUpdate_Building();
        }
    }

    void LateUpdate()
    {
        if (!IsRecycled)
        {
        }
    }

    public void SetShown(bool shown)
    {
        foreach (KeyValuePair<uint, Building> kv in BuildingDict)
        {
            kv.Value.SetShown(shown);
        }
    }

    private Building AddBuilding(BuildingInfo bi)
    {
        Building building = Building.BaseInitialize(bi, this);
        building.OnRemoveBuildingSuc = RemoveBuilding;
        BuildingDict.Add(bi.GUID, building);

        building.transform.SetParent(BuildingContainer);
        building.BuildingGridRoot.SetGridShown(GridShown);
        building.BuildingGridRoot.ResetAllGridConflict();
        building.BuildingGridRoot.SetIsolatedIndicatorShown(false);
        return building;
    }

    private void RemoveBuilding(Building building)
    {
        building.OnRemoveBuildingSuc = null;
        BuildingDict.Remove(building.BuildingInfo.GUID);
    }

    void Update_Building()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GridShown = !GridShown;
        }
    }

    void FixedUpdate_Building()
    {
    }

    private bool _gridShown = true;

    public bool GridShown
    {
        get { return _gridShown; }
        set
        {
            if (_gridShown != value)
            {
                foreach (KeyValuePair<uint, Building> kv in BuildingDict)
                {
                    kv.Value.BuildingGridRoot.SetGridShown(value);
                }
            }

            _gridShown = value;
        }
    }
}