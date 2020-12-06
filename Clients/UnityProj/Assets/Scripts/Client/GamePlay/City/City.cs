using UnityEngine;
using System.Collections.Generic;
using BiangStudio.AdvancedInventory;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine.Events;

public class City : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer MeshRenderer_Range;

    [SerializeField]
    private MeshRenderer MeshRenderer_Grid;

    [HideInInspector]
    public CityConfig CityConfig;

    [HideInInspector]
    public CityInventory CityInventory;

    public Transform BuildingContainer;

    public CityInventoryVirtualOccupationQuadRoot CityInventoryVirtualOccupationQuadRoot;

    public CityEditAreaIndicator CityEditAreaIndicator;

    public Transform CityInventoryGridContainer;

    private CityInventoryGrid[,] cityInventoryGirdMatrix; // column, row

    public Dictionary<BuildingKey, Building> BuildingPrefabDict = new Dictionary<BuildingKey, Building>();
    public Dictionary<BuildingKey, BuildingInfo> BuildingInfoDict = new Dictionary<BuildingKey, BuildingInfo>();

    public SortedDictionary<uint, Building> BuildingDict = new SortedDictionary<uint, Building>();

    public UnityAction<Building> OnHoverBuilding;
    public UnityAction<Building> OnHoverEndBuilding;

    public void Init(CityInventory cityInventory, CityConfig cityConfig, List<Building> buildingPrefabs, UnityAction<Building> onHoverBuilding = null, UnityAction<Building> onHoverEndBuilding = null)
    {
        CityInventory = cityInventory;
        CityConfig = cityConfig;

        foreach (Building buildingPrefab in buildingPrefabs)
        {
            BuildingPrefabDict.Add(buildingPrefab.BuildingInfo.buildingKey, buildingPrefab);
            BuildingInfoDict.Add(buildingPrefab.BuildingInfo.buildingKey, buildingPrefab.BuildingInfo.Clone());
        }

        OnHoverBuilding = onHoverBuilding;
        OnHoverEndBuilding = onHoverEndBuilding;
        CityInventory.RefreshInventoryGrids();
        CityInventory.RefreshConflictAndIsolation();

        CityEditAreaIndicator.Init(CityInventory);
        cityInventoryGirdMatrix = new CityInventoryGrid[cityInventory.Columns, cityInventory.Rows];
        for (int col = 0; col < cityInventory.Columns; col++)
        {
            for (int row = 0; row < cityInventory.Rows; row++)
            {
                CityInventoryGrid grid = cityInventory.CreateCityInventoryGrid(CityInventoryGridContainer);
                grid.transform.localPosition = new Vector3((col - cityInventory.Columns / 2) * cityInventory.GridSize, 0, (row - cityInventory.Rows / 2) * cityInventory.GridSize);
                grid.Init(new GridPos(col, row), CityInventory);
                cityInventoryGirdMatrix[col, row] = grid;
            }
        }

        CityInventory.OnAddItemSucAction = OnAddBuildingSuc;
        CityInventory.OnRemoveItemSucAction = OnRemoveBuildingSuc;
        GridShown = false;
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GridShown = !GridShown;
        }
    }

    public Building GetBuilding(uint guid)
    {
        BuildingDict.TryGetValue(guid, out Building building);
        return building;
    }

    private void OnAddBuildingSuc(InventoryItem ii)
    {
        Building building = Instantiate(BuildingPrefabDict[((BuildingInfo) ii.ItemContentInfo).buildingKey], BuildingContainer);
        building.Initialize(this, ii);
        BuildingDict.Add(ii.GUID, building);
    }

    private void OnRemoveBuildingSuc(InventoryItem ii)
    {
        Building building = BuildingDict[ii.GUID];
        Destroy(building.gameObject);
        BuildingDict.Remove(ii.GUID);
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

    public void SetCityMapShown(bool shown)
    {
        MeshRenderer_Range.enabled = shown;
        MeshRenderer_Grid.enabled = shown;
    }

    public void RefreshBuildingCoverMaskMatrix(bool includingPreview)
    {
        BuildingCoverMaskGroup[,] matrix = includingPreview ? CityInventory.BuildingCoverMaskMatrix_IncludingPreview : CityInventory.BuildingCoverMaskMatrix;

        void BatchSetRadiusGridCoverMask(BuildingCoverMaskGroup mask, int col, int row, float radius)
        {
            for (int col_delta = Mathf.CeilToInt(-radius); col_delta <= Mathf.FloorToInt(radius); col_delta++)
            {
                for (int row_delta = Mathf.CeilToInt(-radius); row_delta <= Mathf.FloorToInt(radius); row_delta++)
                {
                    int final_col = col + col_delta;
                    int final_row = row + row_delta;
                    if (CityInventory.ContainsIndex(final_col, final_row))
                    {
                        if (col_delta * col_delta + row_delta * row_delta <= radius * radius)
                        {
                            matrix[final_col, final_row] |= mask;
                        }
                    }
                }
            }
        }

        void ResetAllCoverMask()
        {
            for (int col = 0; col < CityInventory.Columns; col++)
            {
                for (int row = 0; row < CityInventory.Rows; row++)
                {
                    matrix[col, row] = BuildingCoverMaskGroup.None;
                }
            }
        }

        ResetAllCoverMask();
        for (int col = 0; col < CityInventory.Columns; col++)
        {
            for (int row = 0; row < CityInventory.Rows; row++)
            {
                InventoryItem item = CityInventory.InventoryItemMatrix[col, row];
                if (!includingPreview && CityInventory.InventoryInfo.HasPreviewItem(item))
                {
                    continue;
                }

                BuildingInfo buildingInfo = (BuildingInfo) item?.ItemContentInfo;
                if (buildingInfo != null)
                {
                    BatchSetRadiusGridCoverMask(buildingInfo.provideBuildingCoverInnerRange, item.GridPos_Matrix.x, item.GridPos_Matrix.z, buildingInfo.provideBuildingCoverInnerRange_Radius);
                    BatchSetRadiusGridCoverMask(buildingInfo.provideBuildingCoverOuterRange, item.GridPos_Matrix.x, item.GridPos_Matrix.z, buildingInfo.provideBuildingCoverOuterRange_Radius);
                    matrix[col, row] |= buildingInfo.provideBuildingCoverByOccupation;
                }
            }
        }
    }

    public void RefreshValidGridView(BuildingKey buildingKey)
    {
        void SetAllGridValid(bool valid)
        {
            for (int col = 0; col < CityInventory.Columns; col++)
            {
                for (int row = 0; row < CityInventory.Rows; row++)
                {
                    CityInventory.InventoryValidMatrix[col, row] = valid;
                }
            }
        }

        SetAllGridValid(false);
        for (int col = 0; col < CityInventory.Columns; col++)
        {
            for (int row = 0; row < CityInventory.Rows; row++)
            {
                bool valid = false;
                if (buildingKey != BuildingKey.None)
                {
                    BuildingCoverMaskGroup mask = CityInventory.BuildingCoverMaskMatrix[col, row];
                    BuildingInfo bi = BuildingInfoDict[buildingKey];
                    if ((mask & bi.forbidBuildingCover) == BuildingCoverMaskGroup.None)
                    {
                        foreach (BuildingCoverMaskGroup requireMask in bi.requireBuildingCoverList)
                        {
                            if (mask.HasFlag(requireMask))
                            {
                                valid = true;
                                CityInventory.InventoryValidMatrix[col, row] = true;
                                break;
                            }
                        }
                    }
                }

                cityInventoryGirdMatrix[col, row].SetValidOccupationPreview(valid);
            }
        }
    }

    public void RefreshBuildingRangeGridView(BuildingKey buildingKey)
    {
        void SetAllRangeGrids(CityInventoryGrid.PreviewBuildingRangeGridType showType)
        {
            for (int col = 0; col < CityInventory.Columns; col++)
            {
                for (int row = 0; row < CityInventory.Rows; row++)
                {
                    cityInventoryGirdMatrix[col, row].SetPreviewBuildingRange(showType);
                }
            }
        }

        SetAllRangeGrids(CityInventoryGrid.PreviewBuildingRangeGridType.None);
        // outer Range
        for (int col = 0; col < CityInventory.Columns; col++)
        {
            for (int row = 0; row < CityInventory.Rows; row++)
            {
                if (buildingKey != BuildingKey.None)
                {
                    BuildingCoverMaskGroup mask = CityInventory.BuildingCoverMaskMatrix_IncludingPreview[col, row];
                    BuildingInfo bi = BuildingInfoDict[buildingKey];
                    if (bi.provideBuildingCoverOuterRange != BuildingCoverMaskGroup.None && (mask.HasFlag(bi.provideBuildingCoverOuterRange)))
                    {
                        cityInventoryGirdMatrix[col, row].SetPreviewBuildingRange(CityInventoryGrid.PreviewBuildingRangeGridType.Outer);
                    }
                }
            }
        }

        // inner Range
        for (int col = 0; col < CityInventory.Columns; col++)
        {
            for (int row = 0; row < CityInventory.Rows; row++)
            {
                if (buildingKey != BuildingKey.None)
                {
                    BuildingCoverMaskGroup mask = CityInventory.BuildingCoverMaskMatrix_IncludingPreview[col, row];
                    BuildingInfo bi = BuildingInfoDict[buildingKey];
                    if (bi.provideBuildingCoverInnerRange != BuildingCoverMaskGroup.None && (mask.HasFlag(bi.provideBuildingCoverInnerRange)))
                    {
                        cityInventoryGirdMatrix[col, row].SetPreviewBuildingRange(CityInventoryGrid.PreviewBuildingRangeGridType.Inner);
                    }
                }
            }
        }

        // occupation
        for (int col = 0; col < CityInventory.Columns; col++)
        {
            for (int row = 0; row < CityInventory.Rows; row++)
            {
                if (buildingKey != BuildingKey.None)
                {
                    BuildingCoverMaskGroup mask = CityInventory.BuildingCoverMaskMatrix_IncludingPreview[col, row];
                    BuildingInfo bi = BuildingInfoDict[buildingKey];
                    if (bi.provideBuildingCoverByOccupation != BuildingCoverMaskGroup.None && (mask.HasFlag(bi.provideBuildingCoverByOccupation)))
                    {
                        cityInventoryGirdMatrix[col, row].SetPreviewBuildingRange(CityInventoryGrid.PreviewBuildingRangeGridType.Occupation);
                    }
                }
            }
        }

        // previewing invalid occupation
        for (int col = 0; col < CityInventory.Columns; col++)
        {
            for (int row = 0; row < CityInventory.Rows; row++)
            {
                if (buildingKey != BuildingKey.None)
                {
                    InventoryItem item = CityInventory.InventoryItemMatrix[col, row];
                    if (CityInventory.InventoryInfo.HasPreviewItem(item))
                    {
                        if (!CityInventory.InventoryValidMatrix[item.GridPos_Matrix.x, item.GridPos_Matrix.z])
                        {
                            cityInventoryGirdMatrix[item.GridPos_Matrix.x, item.GridPos_Matrix.z].SetPreviewBuildingRange(CityInventoryGrid.PreviewBuildingRangeGridType.Forbid);
                        }
                    }
                }
            }
        }
    }
}