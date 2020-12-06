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
    public CityConfiguration CityConfiguration;
    public City City;
    internal Transform CityContainerRoot;
    internal BuildingSelectPanel BuildingSelectPanel;

    internal string CurrentSelectedBuildingKey;

    public void StartGame()
    {
        BuildingSelectPanel = UIManager.Instance.ShowUIForms<BuildingSelectPanel>();
        BuildingSelectPanel.Init(CityConfiguration.BuildingPrefabList);
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

        City.CityEditAreaIndicator.GetMousePosOnThisArea(out Vector3 pos_world, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix);

        if (!string.IsNullOrWhiteSpace(CurrentSelectedBuildingKey) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (City.CityInventory.InventoryInfo.PreviewInventoryItem == null)
            {
                BuildingInfo bi = City.BuildingInfoDict[CurrentSelectedBuildingKey].Clone();
                InventoryItem ii = new InventoryItem(bi, City.CityInventory, gp_matrix);
                City.CityInventory.AddPreviewItem(ii);
            }

            if (City.CityInventory.InventoryInfo.PreviewInventoryItem != null)
            {
                GridPosR newGPR = gp_matrix;
                newGPR.orientation = City.CityInventory.InventoryInfo.PreviewInventoryItem.GridPos_Matrix.orientation;
                City.CityInventory.InventoryInfo.PreviewInventoryItem?.SetGridPosition(newGPR);
            }
        }

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            InventoryItem previewItem = City.CityInventory.InventoryInfo.PreviewInventoryItem;
            if (previewItem != null)
            {
                if (City.CityInventory.CheckSpaceAvailable(previewItem.OccupiedGridPositions_Matrix))
                {
                    City.CityInventory.PutDownItem(previewItem);
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
                }
            }
        }
    }

    public void OnClickSelectBuildingButton(string buildingKey)
    {
        OnClearSelectBuildingButton();
        CurrentSelectedBuildingKey = buildingKey;
        BuildingSelectPanel.SelectButton(buildingKey);
    }

    public void OnClearSelectBuildingButton()
    {
        if (City.CityInventory.InventoryInfo.PreviewInventoryItem != null)
        {
            City.CityInventory.RemoveItem(City.CityInventory.InventoryInfo.PreviewInventoryItem, true);
        }

        CurrentSelectedBuildingKey = "";
        BuildingSelectPanel.ClearSelectButton();
    }
}