using BiangStudio.GameDataFormat.Grid;
using BiangStudio.AdvancedInventory;
using BiangStudio.DragHover;

public class CityInventory : Inventory
{
    public CityInventory(
        string inventoryName,
        DragAreaIndicator dragAreaIndicator,
        int gridSize,
        int rows,
        int columns,
        bool x_Mirror,
        bool z_Mirror,
        bool unlockedPartialGrids,
        int unlockedGridCount,
        bool dragOutDrop,
        KeyDownDelegate rotateItemKeyDownHandler
    ) : base(inventoryName, dragAreaIndicator, gridSize, rows, columns, x_Mirror, z_Mirror, unlockedPartialGrids, unlockedGridCount, dragOutDrop,
        rotateItemKeyDownHandler,
        (gridPos) => new GridPosR(gridPos.x + ConfigManager.EDIT_AREA_HALF_SIZE, gridPos.z + ConfigManager.EDIT_AREA_HALF_SIZE, gridPos.orientation),
        (gp_matrix) => new GridPosR(gp_matrix.x - ConfigManager.EDIT_AREA_HALF_SIZE, gp_matrix.z - ConfigManager.EDIT_AREA_HALF_SIZE, gp_matrix.orientation),
        (gridPos) => new GridPos(gridPos.x, gridPos.z),
        (gp_matrix) => new GridPos(gp_matrix.x, gp_matrix.z)
    )
    {
    }
}