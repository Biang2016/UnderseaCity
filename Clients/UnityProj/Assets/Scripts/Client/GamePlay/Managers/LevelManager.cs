using System.Collections.Generic;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay.UI;
using BiangStudio.AdvancedInventory;
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
        GridPosR matrixGP = City.CityInfo.CityInventory.CoordinateTransformationHandler_FromPosToMatrixIndex.Invoke(worldGP);
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            InventoryItem previewItem = City.CityInfo.CityInventory.InventoryInfo.PreviewInventoryItem;
            if (previewItem != null)
            {
                if (City.CityInfo.CityInventory.CheckSpaceAvailable(previewItem.OccupiedGridPositions_Matrix))
                {
                    City.CityInfo.CityInventory.PutDownItem(previewItem);
                }
                else
                {
                    City.CityInfo.CityInventory.RemoveItem(previewItem, true);
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(CurrentSelectedBuildingKey) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (City.CityInfo.CityInventory.InventoryInfo.PreviewInventoryItem == null)
            {
                BuildingInfo bi = new BuildingInfo(ConfigManager.BuildingConfigDict[CurrentSelectedBuildingKey].Clone());
                City.CityInfo.AddBuildingInfo(bi, matrixGP, true);
            }

            City.CityInfo.CityInventory.InventoryInfo.PreviewInventoryItem?.SetGridPosition(matrixGP);
        }
    }

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
        if (City.CityInfo.CityInventory.InventoryInfo.PreviewInventoryItem != null)
        {
            ((BuildingInfo) (City.CityInfo.CityInventory.InventoryInfo.PreviewInventoryItem.ItemContentInfo)).RemoveBuildingInfo();
        }

        CurrentSelectedBuildingKey = "";
        BuildingSelectPanel.ClearSelectButton();
    }
}