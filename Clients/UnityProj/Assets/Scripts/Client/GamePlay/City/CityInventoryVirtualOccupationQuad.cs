using BiangStudio.AdvancedInventory;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

public class CityInventoryVirtualOccupationQuad : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer SpriteRenderer;

    [SerializeField]
    private Color AvailableColor;

    [SerializeField]
    private Color ForbiddenColor;

    private Inventory Inventory;

    public void Init(int gridSize, GridPos gp_matrix, Inventory inventory)
    {
        Inventory = inventory;
        GridPos gp_world = Inventory.CoordinateTransformationHandler_FromMatrixIndexToPos(gp_matrix);
        transform.localScale = gridSize * Vector3.one;
        transform.localPosition = new Vector3(gp_world.x * gridSize, 0, gp_world.z * gridSize);
        SpriteRenderer.color = inventory.InventoryGridMatrix[gp_matrix.x, gp_matrix.z].Available ? AvailableColor : ForbiddenColor;
    }
}