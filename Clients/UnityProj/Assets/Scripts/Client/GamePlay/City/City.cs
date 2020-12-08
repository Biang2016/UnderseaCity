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

    public SortedDictionary<uint, Building> BuildingDict = new SortedDictionary<uint, Building>();

    public UnityAction<Building> OnHoverBuilding;
    public UnityAction<Building> OnHoverEndBuilding;

    public void Init(CityInventory cityInventory, CityConfig cityConfig, UnityAction<Building> onHoverBuilding = null, UnityAction<Building> onHoverEndBuilding = null)
    {
        CityInventory = cityInventory;
        CityConfig = cityConfig;

        OnHoverBuilding = onHoverBuilding;
        OnHoverEndBuilding = onHoverEndBuilding;
        CityInventory.RefreshInventoryGrids();
        CityInventory.RefreshConflictAndIsolation();

        CityEditAreaIndicator.Init(CityInventory);
        cityInventoryGirdMatrix = new CityInventoryGrid[cityInventory.Columns, cityInventory.Rows];

        while (rockPosSet.Count < 2) rockPosSet.Add(new GridPos(Random.Range(0, CityInventory.Columns / 2), Random.Range(0, CityInventory.Rows / 2)));
        while (rockPosSet.Count < 4) rockPosSet.Add(new GridPos(Random.Range(CityInventory.Columns / 2, CityInventory.Columns), Random.Range(0, CityInventory.Rows / 2)));
        while (rockPosSet.Count < 6) rockPosSet.Add(new GridPos(Random.Range(0, CityInventory.Columns / 2), Random.Range(CityInventory.Rows / 2, CityInventory.Rows)));
        while (rockPosSet.Count < 8) rockPosSet.Add(new GridPos(Random.Range(CityInventory.Columns / 2, CityInventory.Columns), Random.Range(CityInventory.Rows / 2, CityInventory.Rows)));
        while (rockPosSet.Count < 10) rockPosSet.Add(new GridPos(Random.Range(CityInventory.Columns / 3 * 1, CityInventory.Columns / 3 * 2), Random.Range(CityInventory.Rows / 3 * 1, CityInventory.Rows / 3 * 2)));

        PerlinRandomOffset = new Vector2(Random.Range(-99872.3213f, 344.1239123f), Random.Range(-12312.1231f, 39291.1231f));
        for (int col = 0; col < cityInventory.Columns; col++)
        {
            for (int row = 0; row < cityInventory.Rows; row++)
            {
                CityInventoryGrid grid = cityInventory.CreateCityInventoryGrid(CityInventoryGridContainer);
                grid.transform.localPosition = new Vector3((col - cityInventory.Columns / 2) * cityInventory.GridSize, 0, (row - cityInventory.Rows / 2) * cityInventory.GridSize);
                TerrainType tt = GenerateTerrainByPerlinNoise(col, row);
                grid.Init(new GridPos(col, row), CityInventory, tt);
                cityInventoryGirdMatrix[col, row] = grid;
            }
        }

        CityInventory.OnAddItemSucAction = OnAddBuildingSuc;
        CityInventory.OnRemoveItemSucAction = OnRemoveBuildingSuc;
        GridShown = false;

        RefreshBuildingCoverMaskMatrix(false);
    }

    public Vector2 PerlinTerrainFactor = Vector2.zero;
    private Vector2 PerlinRandomOffset = Vector2.zero;

    public HashSet<GridPos> rockPosSet = new HashSet<GridPos>();

    private TerrainType GenerateTerrainByPerlinNoise(int col, int row)
    {
        if (rockPosSet.Contains(new GridPos(col, row)))
        {
            return TerrainType.Rock;
        }

        TerrainType tt = TerrainType.None;
        float noiseValue = Perlin.Noise((float) col / CityInventory.Columns * PerlinTerrainFactor.x + PerlinRandomOffset.x, (float) row / CityInventory.Rows * PerlinTerrainFactor.y + PerlinRandomOffset.y);
        if (noiseValue < 0)
        {
            tt = TerrainType.None;
        }
        else if (noiseValue < 0.1f)
        {
            tt = TerrainType.None;
        }
        else if (noiseValue < 0.4f)
        {
            tt = TerrainType.Gold_1;
        }
        else if (noiseValue < 0.7f)
        {
            tt = TerrainType.Gold_2;
        }
        else if (noiseValue < 1f)
        {
            tt = TerrainType.Gold_3;
        }

        return tt;
    }

    protected void Update()
    {
        if (!LevelManager.Instance.GameStart) return;
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
        Building building = Instantiate(LevelManager.Instance.BuildingPrefabDict[((BuildingInfo) ii.ItemContentInfo).buildingKey], BuildingContainer);
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
                CityInventoryGrid terrainGrid = cityInventoryGirdMatrix[col, row];
                BuildingCoverMaskGroup terrainMask = CityInventoryGrid.TerrainTypeToMask(terrainGrid.TerrainType);
                matrix[col, row] |= terrainMask;

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

                if (!includingPreview) cityInventoryGirdMatrix[col, row].Mask = matrix[col, row];
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
                    BuildingInfo bi = LevelManager.Instance.BuildingInfoDict[buildingKey];
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
                    BuildingInfo bi = LevelManager.Instance.BuildingInfoDict[buildingKey];
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
                    BuildingInfo bi = LevelManager.Instance.BuildingInfoDict[buildingKey];
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
                    BuildingInfo bi = LevelManager.Instance.BuildingInfoDict[buildingKey];
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
                        BuildingInfo bi = LevelManager.Instance.BuildingInfoDict[buildingKey];
                        bool needAllOccupationValid = bi.NeedAllOccupationValid;
                        bool valid = false;
                        if (needAllOccupationValid)
                        {
                            valid = true;
                            foreach (GridPos gp in item.OccupiedGridPositions_Matrix)
                            {
                                if (CityInventory.ContainsGP(gp))
                                {
                                    if (!CityInventory.InventoryValidMatrix[gp.x, gp.z])
                                    {
                                        valid = false;
                                    }
                                }
                                else
                                {
                                    valid = false;
                                }
                            }
                        }
                        else
                        {
                            if (CityInventory.ContainsGP(item.GridPos_Matrix))
                            {
                                if (CityInventory.InventoryValidMatrix[item.GridPos_Matrix.x, item.GridPos_Matrix.z])
                                {
                                    valid = true;
                                }
                            }
                            else
                            {
                                valid = false;
                            }
                        }

                        if (!valid)
                        {
                            cityInventoryGirdMatrix[col, row].SetPreviewBuildingRange(CityInventoryGrid.PreviewBuildingRangeGridType.Forbid);
                        }
                    }
                }
            }
        }
    }

    private float tick = 0;

    void FixedUpdate()
    {
        if (!LevelManager.Instance.GameStart) return;
        tick += Time.fixedDeltaTime;
        if (tick > CityConfig.SecondPerMinutes)
        {
            Tick();
            tick -= CityConfig.SecondPerMinutes;
        }

        LevelManager.Instance.HUDPanel.DayProgressBarSlider.value = tick / CityConfig.SecondPerMinutes;
    }

    void Tick()
    {
        RefreshResourcesInfo();
        LevelManager.Instance.Budget += LevelManager.Instance.BudgetNetIncrease;
        LevelManager.Instance.Oxygen += LevelManager.Instance.OxygenNetIncrease;
        LevelManager.Instance.Population += LevelManager.Instance.PopulationNetIncrease;

        // calculate gold value decrease
        foreach (KeyValuePair<uint, Building> kv in BuildingDict)
        {
            if (CityInventory.InventoryInfo.HasPreviewItem(kv.Value.InventoryItem)) continue;
            float budgetProduction = kv.Value.BuildingInfo.BudgetProduction * LevelManager.Instance.WorkingEfficiency;
            if (budgetProduction > 0)
            {
                float radius = kv.Value.BuildingInfo.provideBuildingCoverOuterRange_Radius; // treasure hunter radius
                for (int col_delta = Mathf.CeilToInt(-radius); col_delta <= Mathf.FloorToInt(radius); col_delta++)
                {
                    for (int row_delta = Mathf.CeilToInt(-radius); row_delta <= Mathf.FloorToInt(radius); row_delta++)
                    {
                        int final_col = kv.Value.InventoryItem.GridPos_Matrix.x + col_delta;
                        int final_row = kv.Value.InventoryItem.GridPos_Matrix.z + row_delta;
                        if (CityInventory.ContainsIndex(final_col, final_row))
                        {
                            if (col_delta * col_delta + row_delta * row_delta <= radius * radius)
                            {
                                BuildingCoverMaskGroup mask = CityInventory.BuildingCoverMaskMatrix[final_col, final_row];
                                if ((mask.Mask_Part2 & BuildingCoverMask_Part2.Gold) != BuildingCoverMask_Part2.None)
                                {
                                    float remainGold = cityInventoryGirdMatrix[final_col, final_row].GoldValue;
                                    budgetProduction = Mathf.Min(budgetProduction, remainGold);
                                    cityInventoryGirdMatrix[final_col, final_row].GoldValue -= budgetProduction;
                                    if (cityInventoryGirdMatrix[final_col, final_row].GoldValue <= 0)
                                    {
                                        CityInventory.BuildingCoverMaskMatrix[final_col, final_row].Mask_Part2 &= ~BuildingCoverMask_Part2.Gold;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    public void RefreshResourcesInfo()
    {
        float budgetProductionTotal = 0;
        int peopleDemandForBudgetProductionTotal = 0;
        int budgetConsumptionTotal = 0;
        int oxygenProductionTotal = 0;
        int oxygenConsumptionCommonTotal = 0;
        int oxygenConsumptionForTreasureTotal = 0;
        int oxygenCapacityTotal = 1000;
        int populationCapacityTotal = 0;
        foreach (KeyValuePair<uint, Building> kv in BuildingDict)
        {
            if (CityInventory.InventoryInfo.HasPreviewItem(kv.Value.InventoryItem)) continue;
            budgetConsumptionTotal += kv.Value.BuildingInfo.BudgetConsumption;
            oxygenProductionTotal += kv.Value.BuildingInfo.OxygenProduction;
            oxygenCapacityTotal += kv.Value.BuildingInfo.OxygenCapacity;
            populationCapacityTotal += kv.Value.BuildingInfo.PopulationCapacity;

            // calculate budget increase
            float budgetProduction = kv.Value.BuildingInfo.BudgetProduction;
            if (budgetProduction > 0)
            {
                oxygenConsumptionForTreasureTotal += kv.Value.BuildingInfo.OxygenConsumption;
                float radius = kv.Value.BuildingInfo.provideBuildingCoverOuterRange_Radius; // treasure hunter radius
                for (int col_delta = Mathf.CeilToInt(-radius); col_delta <= Mathf.FloorToInt(radius); col_delta++)
                {
                    for (int row_delta = Mathf.CeilToInt(-radius); row_delta <= Mathf.FloorToInt(radius); row_delta++)
                    {
                        int final_col = kv.Value.InventoryItem.GridPos_Matrix.x + col_delta;
                        int final_row = kv.Value.InventoryItem.GridPos_Matrix.z + row_delta;
                        if (CityInventory.ContainsIndex(final_col, final_row))
                        {
                            if (col_delta * col_delta + row_delta * row_delta <= radius * radius)
                            {
                                BuildingCoverMaskGroup mask = CityInventory.BuildingCoverMaskMatrix[final_row, final_row];
                                if ((mask.Mask_Part2 & BuildingCoverMask_Part2.Gold) != BuildingCoverMask_Part2.None)
                                {
                                    float remainGold = cityInventoryGirdMatrix[final_row, final_row].GoldValue;
                                    budgetProduction = Mathf.Min(budgetProduction, remainGold);
                                    budgetProductionTotal += budgetProduction;
                                    peopleDemandForBudgetProductionTotal += kv.Value.BuildingInfo.PeopleDemandForBudgetProduction;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                oxygenConsumptionCommonTotal += kv.Value.BuildingInfo.OxygenConsumption;
            }
        }

        oxygenConsumptionCommonTotal += LevelManager.Instance.Population;

        LevelManager.Instance.PeopleDemandForBudgetProduction = peopleDemandForBudgetProductionTotal;
        LevelManager.Instance.BudgetProduction = Mathf.RoundToInt(budgetProductionTotal * LevelManager.Instance.WorkingEfficiency);
        LevelManager.Instance.BudgetConsumption = budgetConsumptionTotal;
        LevelManager.Instance.OxygenProduction = oxygenProductionTotal;
        LevelManager.Instance.OxygenConsumptionCommon = oxygenConsumptionCommonTotal;
        LevelManager.Instance.OxygenConsumptionForTreasure = Mathf.RoundToInt(oxygenConsumptionForTreasureTotal * LevelManager.Instance.WorkingEfficiency);
        LevelManager.Instance.OxygenCapacity = oxygenCapacityTotal;
        LevelManager.Instance.PopulationCapacity = populationCapacityTotal;

        if (LevelManager.Instance.Population > LevelManager.Instance.Oxygen && (LevelManager.Instance.OxygenNetIncrease) < 0)
        {
            LevelManager.Instance.PopulationNetIncrease = LevelManager.Instance.Oxygen - LevelManager.Instance.Population;
        }
        else if (LevelManager.Instance.Population < LevelManager.Instance.PopulationCapacity && LevelManager.Instance.PopulationCapacity > 0)
        {
            float increaseFactor_Capacity = (float) (LevelManager.Instance.PopulationCapacity - LevelManager.Instance.Population) / LevelManager.Instance.PopulationCapacity;
            float increaseFactor_OxygenNetIncrease = ((float) LevelManager.Instance.OxygenNetIncrease / (LevelManager.Instance.Population + 100));
            LevelManager.Instance.PopulationNetIncrease = Mathf.RoundToInt(LevelManager.Instance.PopulationCapacity * increaseFactor_Capacity * increaseFactor_OxygenNetIncrease * 0.1f);
        }
        else
        {
            LevelManager.Instance.PopulationNetIncrease = 0;
        }
    }
}