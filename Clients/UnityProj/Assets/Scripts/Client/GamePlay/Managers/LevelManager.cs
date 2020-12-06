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

                if (value == BuildingKey.None)
                {
                    if (City.CityInventory.InventoryInfo.PreviewInventoryItem != null)
                    {
                        City.CityInventory.RemoveItem(City.CityInventory.InventoryInfo.PreviewInventoryItem, true);
                    }

                    City.CityInventoryVirtualOccupationQuadRoot.Clear();
                }
            }
        }
    }

    private BuildingKey currentSelectedBuildingKey;

    public void StartGame()
    {
        BuildingSelectPanel = UIManager.Instance.ShowUIForms<BuildingSelectPanel>();
        BuildingSelectPanel.Init(CitySetup.BuildingPrefabList);
        HUDPanel = UIManager.Instance.ShowUIForms<HUDPanel>();
        CurrentSelectedBuildingKey = BuildingKey.None;
        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.Building);

        Budget = 10000;
        Oxygen = 1000;
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
                HUDPanel.SetBudget(budget);
                BuildingSelectPanel.OnResourcesChanged(budget, oxygen);
            }
        }
    }

    private int oxygen = 0;

    public int Oxygen
    {
        get { return oxygen; }
        set
        {
            if (oxygen != value)
            {
                oxygen = value;
                HUDPanel.SetOxygen(oxygen);
                BuildingSelectPanel.OnResourcesChanged(budget, oxygen);
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
                BuildingInfo bi = City.BuildingInfoDict[CurrentSelectedBuildingKey].Clone();
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
                    if (City.CityInventory.InventoryValidMatrix[previewItem.GridPos_Matrix.x, previewItem.GridPos_Matrix.z])
                    {
                        City.CityInventory.PutDownItem(previewItem);
                        BuildingInfo buildingInfo = City.BuildingInfoDict[City.GetBuilding(previewItem.GUID).BuildingInfo.buildingKey];
                        Budget -= buildingInfo.BudgetCost;
                        Oxygen -= buildingInfo.OxygenCost;
                        if (Budget < buildingInfo.BudgetCost || Oxygen < buildingInfo.OxygenCost)
                        {
                            CurrentSelectedBuildingKey = BuildingKey.None;
                        }

                        sucPutDown = true;
                    }
                }

                if (sucPutDown)
                {
                    City.RefreshBuildingCoverMaskMatrix(false);
                    City.RefreshValidGridView(CurrentSelectedBuildingKey);
                    City.RefreshBuildingCoverMaskMatrix(true);
                    City.RefreshBuildingRangeGridView(CurrentSelectedBuildingKey);
                    City.CityInventoryVirtualOccupationQuadRoot.Clear();
                }
                else
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
}