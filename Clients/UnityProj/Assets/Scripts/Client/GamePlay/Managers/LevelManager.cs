using System.Collections.Generic;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.GridBackpack;
using BiangStudio.ShapedInventory;
using BiangStudio.Singleton;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelManager : MonoSingleton<LevelManager>
{
    internal City City;
    internal Transform CityContainerRoot;
    internal BuildingSelectPanel BuildingSelectPanel;

    internal string CurrentSelectedBuildingKey;

    public void StartGame()
    {
        CityInfo cityInfo = new CityInfo(new CityConfig());
        cityInfo.AddBuildingInfo(new BuildingInfo(ConfigManager.BuildingConfigDict["Building_PioneerHub"]), GridPosR.Zero);
        AddCity(cityInfo);

        BuildingSelectPanel = UIManager.Instance.ShowUIForms<BuildingSelectPanel>();
        BuildingSelectPanel.Init();

        CurrentSelectedBuildingKey = "";

        MouseHoverManager.Instance.M_StateMachine.SetState(MouseHoverManager.StateMachine.States.Building);
    }

    public void Awake()
    {
        CityContainerRoot = transform;
    }

    public void Start()
    {
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnClearSelectBuildingButton();
        }

        GridPosR worldGP = DragManager.Instance.GetDragProcessor<Building>().CurrentMousePosition_World.ToGridPosR_XZ();
        GridPosR matrixGP = City.CityInfo.CityEditorInventory.CoordinateTransformationHandler_FromPosToMatrixIndex.Invoke(worldGP);
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (CurrentFakeBuilding != null)
            {
                CurrentFakeBuilding.BuildingInfo.InventoryItem.SetGridPosition(matrixGP);
                City.CityInfo.CityEditorInventory.RefreshConflictAndIsolation(out List<InventoryItem> conflictItems, out List<InventoryItem> _);
                if (conflictItems.Count > 0)
                {
                    CurrentFakeBuilding.BuildingInfo.RemoveBuildingInfo();
                }

                CurrentFakeBuilding = null;
            }
        }

        if (!string.IsNullOrWhiteSpace(CurrentSelectedBuildingKey) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (CurrentFakeBuilding == null)
            {
                BuildingInfo bi = new BuildingInfo(ConfigManager.BuildingConfigDict[CurrentSelectedBuildingKey].Clone());
                if (City.CityInfo.AddBuildingInfo(bi, matrixGP))
                {
                    CurrentFakeBuilding = City.BuildingDict[bi.GUID];
                }
            }

            if (CurrentFakeBuilding != null)
            {
                CurrentFakeBuilding.BuildingInfo.InventoryItem.SetGridPosition(matrixGP);
            }
        }
    }

    private Building CurrentFakeBuilding;

    private void AddCity(CityInfo cityInfo)
    {
        City = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.City].AllocateGameObject<City>(CityContainerRoot);
        City.Initialize(cityInfo);
        City.OnRemoveCitySuc = RemoveCity;
    }

    private void RemoveCity(City city)
    {
        city.PoolRecycle();
    }

    public void OnClickSelectBuildingButton(string buildingKey)
    {
        OnClearSelectBuildingButton();
        CurrentSelectedBuildingKey = buildingKey;
        BuildingSelectPanel.SelectButton(buildingKey);
    }

    public void OnClearSelectBuildingButton()
    {
        if (CurrentFakeBuilding != null)
        {
            CurrentFakeBuilding.BuildingInfo.RemoveBuildingInfo();
            CurrentFakeBuilding = null;
        }

        CurrentSelectedBuildingKey = "";
        BuildingSelectPanel.ClearSelectButton();
    }
}