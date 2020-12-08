using System.Collections.Generic;
using System.Linq;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.AdvancedInventory;
using BiangStudio.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelManager : MonoSingleton<LevelManager>
{
    public CitySetup CitySetup;
    public City City => CitySetup.City;
    internal Transform CityContainerRoot;
    internal BuildingSelectPanel BuildingSelectPanel;
    internal HUDPanel HUDPanel;

    internal BuildingKey CurrentSelectedBuildingKey
    {
        get { return currentSelectedBuildingKey; }
        set
        {
            if (currentSelectedBuildingKey != value)
            {
                currentSelectedBuildingKey = value;
                BuildingSelectPanel.SelectButton(value);
                City.RefreshValidGridView(value);
                City.RefreshBuildingCoverMaskMatrix(true);
                City.RefreshBuildingRangeGridView(value);

                if (City.CityInventory.InventoryInfo.PreviewInventoryItem != null)
                {
                    City.CityInventory.RemoveItem(City.CityInventory.InventoryInfo.PreviewInventoryItem, true);
                }

                City.CityInventoryVirtualOccupationQuadRoot.Clear();
            }
        }
    }

    private BuildingKey currentSelectedBuildingKey;
    public Dictionary<BuildingKey, BuildingInfo> BuildingInfoDict = new Dictionary<BuildingKey, BuildingInfo>();
    public Dictionary<BuildingKey, Building> BuildingPrefabDict = new Dictionary<BuildingKey, Building>();

    internal bool GameStart = false;

    public void StartGame()
    {
        AudioManager.Instance.BGMFadeIn("bgm/Success", 0.1f, 1f, true);
        BuildingSelectPanel = UIManager.Instance.ShowUIForms<BuildingSelectPanel>();
        BuildingSelectPanel.Init(CitySetup.BuildingPrefabList.Select(buildingPrefab => buildingPrefab.BuildingInfo).ToList());
        HUDPanel = UIManager.Instance.ShowUIForms<HUDPanel>();
        HUDPanel.Init();
        CurrentSelectedBuildingKey = BuildingKey.None;
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.Building);

        Budget = 10000;
        OxygenCapacity = 1000;
        Oxygen = 1000;
        PopulationCapacity = 0;
        PopulationNetIncrease = 0;
        Population = 0;
        City.RefreshResourcesInfo();

        GameStart = true;
    }

    public void Awake()
    {
        CityContainerRoot = transform;
    }

    public void Start()
    {
    }

    private int budget = 0;

    public int Budget
    {
        get { return budget; }
        set
        {
            if (budget != value)
            {
                budget = value;
                RecheckResourcesMeetCurrentSelectBuilding();
                HUDPanel.SetBudget(budget, budgetProduction, budgetConsumption);
                BuildingSelectPanel.OnResourcesChanged(budget, oxygen);
            }
        }
    }

    private int budgetProduction = 0;

    public int BudgetProduction
    {
        get { return budgetProduction; }
        set
        {
            if (budgetProduction != value)
            {
                budgetProduction = value;
                HUDPanel.SetBudget(budget, budgetProduction, budgetConsumption);
            }
        }
    }

    private int peopleDemandForBudgetProduction = 0; // 目前总共需要多少个人力来开采资源

    public int PeopleDemandForBudgetProduction
    {
        get { return peopleDemandForBudgetProduction; }
        set
        {
            if (peopleDemandForBudgetProduction != value)
            {
                peopleDemandForBudgetProduction = value;
            }
        }
    }

    public float WorkingEfficiency => PeopleDemandForBudgetProduction > 0 ? Mathf.Min(1f, (float) Population / PeopleDemandForBudgetProduction) : 0;

    private int budgetConsumption = 0;

    public int BudgetConsumption
    {
        get { return budgetConsumption; }
        set
        {
            if (budgetConsumption != value)
            {
                budgetConsumption = value;
                HUDPanel.SetBudget(budget, budgetProduction, budgetConsumption);
            }
        }
    }

    public int BudgetNetIncrease => BudgetProduction - BudgetConsumption;

    private int oxygen = -1;

    public int Oxygen
    {
        get { return oxygen; }
        set
        {
            value = Mathf.Clamp(value, 0, OxygenCapacity);
            if (oxygen != value)
            {
                oxygen = value;
                RecheckResourcesMeetCurrentSelectBuilding();
                HUDPanel.SetOxygen(oxygen, oxygenProduction, oxygenConsumptionCommon, oxygenConsumptionForTreasure, oxygenCapacity);
                BuildingSelectPanel.OnResourcesChanged(budget, oxygen);
            }
        }
    }

    private int oxygenProduction = 0;

    public int OxygenProduction
    {
        get { return oxygenProduction; }
        set
        {
            if (oxygenProduction != value)
            {
                oxygenProduction = value;
                HUDPanel.SetOxygen(oxygen, oxygenProduction, oxygenConsumptionCommon, oxygenConsumptionForTreasure, oxygenCapacity);
            }
        }
    }

    private int oxygenConsumptionCommon = 0;

    public int OxygenConsumptionCommon
    {
        get { return oxygenConsumptionCommon; }
        set
        {
            if (oxygenConsumptionCommon != value)
            {
                oxygenConsumptionCommon = value;
                HUDPanel.SetOxygen(oxygen, oxygenProduction, oxygenConsumptionCommon, oxygenConsumptionForTreasure, oxygenCapacity);
            }
        }
    }

    private int oxygenConsumptionForTreasure = 0;

    public int OxygenConsumptionForTreasure
    {
        get { return oxygenConsumptionForTreasure; }
        set
        {
            if (oxygenConsumptionForTreasure != value)
            {
                oxygenConsumptionForTreasure = value;
                HUDPanel.SetOxygen(oxygen, oxygenProduction, oxygenConsumptionCommon, oxygenConsumptionForTreasure, oxygenCapacity);
            }
        }
    }

    public int OxygenTotalConsumption => OxygenConsumptionCommon + OxygenConsumptionForTreasure;

    public int OxygenNetIncrease => OxygenProduction - OxygenTotalConsumption;

    private int oxygenCapacity = -1;

    public int OxygenCapacity
    {
        get { return oxygenCapacity; }
        set
        {
            if (oxygenCapacity != value)
            {
                oxygenCapacity = value;
                HUDPanel.SetOxygen(oxygen, oxygenProduction, oxygenConsumptionCommon, oxygenConsumptionForTreasure, oxygenCapacity);
            }
        }
    }

    private int population = -1;

    public int Population
    {
        get { return population; }
        set
        {
            value = Mathf.Clamp(value, 0, PopulationCapacity);
            if (population != value)
            {
                population = value;
                HUDPanel.SetPopulation(population, populationNetIncrease, populationCapacity);
            }
        }
    }

    private int populationNetIncrease = -1;

    public int PopulationNetIncrease
    {
        get { return populationNetIncrease; }
        set
        {
            if (populationNetIncrease != value)
            {
                populationNetIncrease = value;
                HUDPanel.SetPopulation(population, populationNetIncrease, populationCapacity);
            }
        }
    }

    private int populationCapacity = -1;

    public int PopulationCapacity
    {
        get { return populationCapacity; }
        set
        {
            if (populationCapacity != value)
            {
                populationCapacity = value;
                HUDPanel.SetPopulation(population, populationNetIncrease, populationCapacity);
            }
        }
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CurrentSelectedBuildingKey = BuildingKey.None;
        }

        City.CityEditAreaIndicator.GetMousePosOnThisArea(out Vector3 pos_world, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix);

        if (CurrentSelectedBuildingKey != BuildingKey.None && !EventSystem.current.IsPointerOverGameObject())
        {
            // add preview
            if (City.CityInventory.InventoryInfo.PreviewInventoryItem == null)
            {
                BuildingInfo bi = BuildingInfoDict[CurrentSelectedBuildingKey].Clone();
                InventoryItem ii = new InventoryItem(bi, City.CityInventory, gp_matrix);
                City.CityInventory.AddPreviewItem(ii);
            }

            // move with cursor
            if (City.CityInventory.InventoryInfo.PreviewInventoryItem != null)
            {
                GridPosR newGPR = gp_matrix;
                newGPR.orientation = City.CityInventory.InventoryInfo.PreviewInventoryItem.GridPos_Matrix.orientation;
                City.CityInventory.InventoryInfo.PreviewInventoryItem?.SetGridPosition(newGPR);

                City.RefreshBuildingCoverMaskMatrix(true);
                City.RefreshBuildingRangeGridView(CurrentSelectedBuildingKey);
            }
        }

        // put down building
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            InventoryItem previewItem = City.CityInventory.InventoryInfo.PreviewInventoryItem;
            if (previewItem != null)
            {
                bool sucPutDown = false;
                if (City.CityInventory.CheckSpaceAvailable(previewItem.OccupiedGridPositions_Matrix))
                {
                    bool needAllOccupationValid = BuildingInfoDict[CurrentSelectedBuildingKey].NeedAllOccupationValid;
                    if (needAllOccupationValid)
                    {
                        sucPutDown = true;
                        foreach (GridPos gp in previewItem.OccupiedGridPositions_Matrix)
                        {
                            if (!City.CityInventory.InventoryValidMatrix[gp.x, gp.z])
                            {
                                sucPutDown = false;
                            }
                        }
                    }

                    else
                    {
                        if (City.CityInventory.InventoryValidMatrix[previewItem.GridPos_Matrix.x, previewItem.GridPos_Matrix.z])
                        {
                            sucPutDown = true;
                        }
                    }

                    if (sucPutDown)
                    {
                        City.CityInventory.PutDownItem(previewItem);
                        BuildingInfo buildingInfo = BuildingInfoDict[City.GetBuilding(previewItem.GUID).BuildingInfo.buildingKey];
                        Budget -= buildingInfo.BudgetCost;
                        Oxygen -= buildingInfo.OxygenCost;
                        City.RefreshResourcesInfo();
                        RecheckResourcesMeetCurrentSelectBuilding();
                        City.RefreshBuildingCoverMaskMatrix(false);
                        City.RefreshValidGridView(CurrentSelectedBuildingKey);
                        City.RefreshBuildingCoverMaskMatrix(true);
                        City.RefreshBuildingRangeGridView(CurrentSelectedBuildingKey);
                        City.CityInventoryVirtualOccupationQuadRoot.Clear();
                    }
                }

                if (!sucPutDown)
                {
                    City.CityInventory.RemoveItem(previewItem, true);
                }
            }
        }

        if (City.CityInventory.RotateItemKeyDownHandler != null && City.CityInventory.RotateItemKeyDownHandler.Invoke())
        {
            InventoryItem previewItem = City.CityInventory.InventoryInfo.PreviewInventoryItem;
            if (previewItem != null)
            {
                Building building = City.GetBuilding(previewItem.GUID);
                if (building != null)
                {
                    building.Rotate();
                    City.RefreshBuildingCoverMaskMatrix(true);
                    City.RefreshBuildingRangeGridView(CurrentSelectedBuildingKey);
                }
            }
        }
    }

    private void RecheckResourcesMeetCurrentSelectBuilding()
    {
        if (CurrentSelectedBuildingKey != BuildingKey.None)
        {
            BuildingInfo bi = BuildingInfoDict[CurrentSelectedBuildingKey];
            if (budget < bi.BudgetCost || Oxygen < bi.OxygenCost)
            {
                CurrentSelectedBuildingKey = BuildingKey.None;
            }
        }
    }
}