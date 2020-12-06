using BiangStudio.AdvancedInventory;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using TMPro;
using UnityEngine;

public class CityInventoryGrid : PoolObject
{
    [SerializeField]
    private TextMeshPro GridPosText;

    [SerializeField]
    private SpriteRenderer ValidOccupationSpriteRenderer;

    [SerializeField]
    private SpriteRenderer PreviewBuildingRangeIndicatorSpriteRenderer;

    [SerializeField]
    private Color ValidColor;

    [SerializeField]
    private Color ForbiddenColor;

    [SerializeField]
    private Color RangePreviewColor_None;

    [SerializeField]
    private Color RangePreviewColor_Occupation;

    [SerializeField]
    private Color RangePreviewColor_Inner;

    [SerializeField]
    private Color RangePreviewColor_Outer;

    [SerializeField]
    private Color RangePreviewColor_Forbid;

    private Inventory Inventory;

    void Awake()
    {
        SetValidOccupationPreview(false);
    }

    public void Init(GridPos gp_matrix, Inventory inventory)
    {
        GridPosText.text = gp_matrix.ToString();

        Inventory = inventory;
        GridPos gp_world = Inventory.CoordinateTransformationHandler_FromMatrixIndexToPos(gp_matrix);
        transform.localScale = inventory.GridSize * Vector3.one;
        transform.localPosition = new Vector3(gp_world.x * inventory.GridSize, 0, gp_world.z * inventory.GridSize);

        SetPreviewBuildingRange(PreviewBuildingRangeGridType.None);
    }

    public void SetValidOccupationPreview(bool valid)
    {
        if (valid)
        {
            ValidOccupationSpriteRenderer.color = ValidColor;
        }
        else
        {
            ValidOccupationSpriteRenderer.color = ForbiddenColor;
        }
    }

    public void SetPreviewBuildingRange(PreviewBuildingRangeGridType typeIndex)
    {
        switch (typeIndex)
        {
            case PreviewBuildingRangeGridType.None:
            {
                PreviewBuildingRangeIndicatorSpriteRenderer.color = RangePreviewColor_None;
                break;
            }
            case PreviewBuildingRangeGridType.Occupation:
            {
                PreviewBuildingRangeIndicatorSpriteRenderer.color = RangePreviewColor_Occupation;
                break;
            }
            case PreviewBuildingRangeGridType.Inner:
            {
                PreviewBuildingRangeIndicatorSpriteRenderer.color = RangePreviewColor_Inner;
                break;
            }
            case PreviewBuildingRangeGridType.Outer:
            {
                PreviewBuildingRangeIndicatorSpriteRenderer.color = RangePreviewColor_Outer;
                break;
            }
            case PreviewBuildingRangeGridType.Forbid:
            {
                PreviewBuildingRangeIndicatorSpriteRenderer.color = RangePreviewColor_Forbid;
                break;
            }
        }
    }

    public enum PreviewBuildingRangeGridType
    {
        None = 0,
        Occupation = 1,
        Inner = 2,
        Outer = 3,
        Forbid = 4,
    }
}