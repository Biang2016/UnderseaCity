using System;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.AdvancedInventory;
using BiangStudio.DragHover;
using UnityEngine;

public class CityInventory : Inventory
{
    public City City;
    public DragProcessor DragProcessor;

    private InstantiatePrefabDelegate InstantiateCityInventoryGridHandler;
    private InstantiatePrefabDelegate InstantiateCityInventoryVirtualOccupationQuadHandler;

    public bool[,] InventoryValidMatrix; // column, row
    public BuildingCoverMaskGroup[,] BuildingCoverMaskMatrix; // column, row
    public BuildingCoverMaskGroup[,] BuildingCoverMaskMatrix_IncludingPreview; // column, row

    public CityInventory(
        string inventoryName,
        DragAreaIndicator dragAreaIndicator,
        DragProcessor dragProcessor,
        int gridSize,
        int rows,
        int columns,
        bool x_Mirror,
        bool z_Mirror,
        bool unlockedPartialGrids,
        int unlockedGridCount,
        bool dragOutDrop,
        KeyDownDelegate rotateItemKeyDownHandler,
        InstantiatePrefabDelegate instantiateCityInventoryGridHandler,
        InstantiatePrefabDelegate instantiateCityInventoryVirtualOccupationQuadHandler
    ) : base(inventoryName, dragAreaIndicator, gridSize, rows, columns, x_Mirror, z_Mirror, unlockedPartialGrids, unlockedGridCount, dragOutDrop,
        rotateItemKeyDownHandler,
        (gridPos) => new GridPosR(gridPos.x + columns / 2, gridPos.z + rows / 2, gridPos.orientation),
        (gp_matrix) => new GridPosR(gp_matrix.x - columns / 2, gp_matrix.z - rows / 2, gp_matrix.orientation),
        (gridPos) => new GridPos(gridPos.x, gridPos.z),
        (gp_matrix) => new GridPos(gp_matrix.x, gp_matrix.z)
    )
    {
        DragProcessor = dragProcessor;
        InstantiateCityInventoryGridHandler = instantiateCityInventoryGridHandler;
        InstantiateCityInventoryVirtualOccupationQuadHandler = instantiateCityInventoryVirtualOccupationQuadHandler;
        InventoryValidMatrix = new bool[columns, rows];
        BuildingCoverMaskMatrix = new BuildingCoverMaskGroup[columns, rows];
        BuildingCoverMaskMatrix_IncludingPreview = new BuildingCoverMaskGroup[columns, rows];
    }

    public CityInventoryGrid CreateCityInventoryGrid(Transform transform)
    {
        if (InstantiateCityInventoryGridHandler != null)
        {
            MonoBehaviour mono = InstantiateCityInventoryGridHandler?.Invoke(transform);
            if (mono != null)
            {
                try
                {
                    CityInventoryGrid res = (CityInventoryGrid) mono;
                    return res;
                }
                catch (Exception e)
                {
                    LogError(e.ToString());
                }
            }
        }

        return null;
    }

    public CityInventoryVirtualOccupationQuad CreateCityInventoryVirtualOccupationQuad(Transform transform)
    {
        if (InstantiateCityInventoryVirtualOccupationQuadHandler != null)
        {
            MonoBehaviour mono = InstantiateCityInventoryVirtualOccupationQuadHandler?.Invoke(transform);
            if (mono != null)
            {
                try
                {
                    CityInventoryVirtualOccupationQuad res = (CityInventoryVirtualOccupationQuad) mono;
                    return res;
                }
                catch (Exception e)
                {
                    LogError(e.ToString());
                }
            }
        }

        return null;
    }
}