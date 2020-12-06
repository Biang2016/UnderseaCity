using BiangStudio;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.AdvancedInventory;
using Sirenix.OdinInspector;
using UnityEngine;

public class Building : MonoBehaviour, IMouseHoverComponent, IDraggable
{
    public City City = null;
    internal CityInventory CityInventory;
    public InventoryItem InventoryItem;
    public BuildingInfo BuildingInfo;

    public BuildingGridRoot BuildingGridRoot;
    public BuildingModelRoot BuildingModelRoot;

    public void Initialize(City parentCity, InventoryItem inventoryItem)
    {
        City = parentCity;
        CityInventory = City.CityInventory;
        SetInventoryItem(inventoryItem);
        SetGridPos(InventoryItem.GridPos_World);
        BuildingGridRoot.SetInUsed(true);
    }

    [Button("序列化位置信息")]
    private void SerializeGPOccupation()
    {
        BuildingInfo.BuildingOccupiedGridPositionList = BuildingGridRoot.GetOccupiedPositions();
    }

    public void SetInventoryItem(InventoryItem inventoryItem)
    {
        InventoryItem = inventoryItem;
        InventoryItem.OnSetGridPosHandler = SetGridPos;
        InventoryItem.OnIsolatedHandler = OnInventoryItemOnIsolated;
        InventoryItem.OnConflictedHandler = BuildingGridRoot.SetGridConflicted;
        InventoryItem.OnResetConflictHandler = BuildingGridRoot.ResetAllGridConflict;
    }

    public void Rotate()
    {
        GridPosR.Orientation newOri = GridPosR.RotateOrientationClockwise90(InventoryItem.GridPos_Matrix.orientation);
        GridPosR newGPR = new GridPosR(InventoryItem.GridPos_Matrix.x, InventoryItem.GridPos_Matrix.z, newOri);
        InventoryItem.SetGridPosition(newGPR);
    }

    private void SetGridPos(GridPosR gridPos_World)
    {
        GridPosR.ApplyGridPosToLocalTrans(gridPos_World, transform, CityInventory.GridSize);
        CityInventory.RefreshConflictAndIsolation();
        SetVirtualGridPos(gridPos_World);
    }

    private void SetVirtualGridPos(GridPosR gridPos_World)
    {
        City.CityInventoryVirtualOccupationQuadRoot.Clear();
        foreach (GridPos gp_matrix in InventoryItem.OccupiedGridPositions_Matrix)
        {
            if (!CityInventory.ContainsGP(gp_matrix))
            {
                continue;
            }

            CityInventoryVirtualOccupationQuad quad = CityInventory.CreateCityInventoryVirtualOccupationQuad(City.CityInventoryVirtualOccupationQuadRoot.transform);
            quad.Init(InventoryItem.Inventory.GridSize, gp_matrix, InventoryItem.Inventory);
            City.CityInventoryVirtualOccupationQuadRoot.cityInventoryVirtualOccupationQuads.Add(quad);
        }
    }

    private void OnInventoryItemOnIsolated(bool shown)
    {
        if (shown)
        {
            BuildingModelRoot.SetBuildingBasicEmissionColor(CommonUtils.HTMLColorToColor("#E42835"));
        }
        else
        {
            BuildingModelRoot.ResetBuildingBasicEmissionColor();
        }

        BuildingGridRoot.SetIsolatedIndicatorShown(shown);
    }

    private void HighLightColorChange(Color highLightColor, float intensity)
    {
        BuildingModelRoot.SetDefaultHighLightEmissionColor(highLightColor * intensity);
    }

    public void SetShown(bool shown)
    {
        BuildingModelRoot.SetShown(shown);
    }

    #region IMouseHoverComponent

    public void MouseHoverComponent_OnHoverBegin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnHoverEnd()
    {
    }

    public void MouseHoverComponent_OnFocusBegin(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnFocusEnd()
    {
    }

    public void MouseHoverComponent_OnMousePressEnterImmediately(Vector3 mousePosition)
    {
    }

    public void MouseHoverComponent_OnMousePressLeaveImmediately()
    {
    }

    #endregion

    #region IDraggable

    public void Draggable_OnMouseDown(DragAreaIndicator dragAreaIndicator, Collider collider)
    {
    }

    public void Draggable_OnMousePressed(DragAreaIndicator dragAreaIndicator, Vector3 diffFromStart, Vector3 deltaFromLastFrame)
    {
    }

    public void Draggable_OnMouseUp(DragAreaIndicator dragAreaIndicator, Vector3 diffFromStart, Vector3 deltaFromLastFrame)
    {
    }

    public void Draggable_OnPaused()
    {
    }

    public void Draggable_OnResume()
    {
    }

    public void Draggable_OnSucceedWhenPaused()
    {
    }

    public void Draggable_SetStates(ref bool canDrag, ref DragAreaIndicator dragFromDragAreaIndicator)
    {
    }

    #endregion
}