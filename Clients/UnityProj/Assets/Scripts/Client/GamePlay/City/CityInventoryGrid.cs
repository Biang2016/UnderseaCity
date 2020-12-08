using System.Collections.Generic;
using BiangStudio.AdvancedInventory;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using TMPro;
using UnityEngine;

public class CityInventoryGrid : PoolObject
{
    public BuildingCoverMaskGroup Mask = new BuildingCoverMaskGroup();

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
        mpb = new MaterialPropertyBlock();
    }

    public void Init(GridPos gp_matrix, Inventory inventory, TerrainType terrainType)
    {
        GridPosText.text = gp_matrix.ToString();

        Inventory = inventory;
        GridPos gp_world = Inventory.CoordinateTransformationHandler_FromMatrixIndexToPos(gp_matrix);
        transform.localScale = inventory.GridSize * Vector3.one;
        transform.localPosition = new Vector3(gp_world.x * inventory.GridSize, 0, gp_world.z * inventory.GridSize);

        SetPreviewBuildingRange(PreviewBuildingRangeGridType.None);

        InitTerrain(terrainType);
    }

    private MaterialPropertyBlock mpb;

    private void SetRendererColor(Renderer renderer, Color color)
    {
        if (mpb == null)
        {
            mpb = new MaterialPropertyBlock();
        }

        renderer.GetPropertyBlock(mpb);
        mpb.SetColor("_Color", color);
        renderer.SetPropertyBlock(mpb);
    }

    public void SetValidOccupationPreview(bool valid)
    {
        if (valid)
        {
            SetRendererColor(ValidOccupationSpriteRenderer, ValidColor);
        }
        else
        {
            SetRendererColor(ValidOccupationSpriteRenderer, ForbiddenColor);
        }
    }

    public void SetPreviewBuildingRange(PreviewBuildingRangeGridType typeIndex)
    {
        switch (typeIndex)
        {
            case PreviewBuildingRangeGridType.None:
            {
                SetRendererColor(PreviewBuildingRangeIndicatorSpriteRenderer, RangePreviewColor_None);
                break;
            }
            case PreviewBuildingRangeGridType.Occupation:
            {
                SetRendererColor(PreviewBuildingRangeIndicatorSpriteRenderer, RangePreviewColor_Occupation);
                break;
            }
            case PreviewBuildingRangeGridType.Inner:
            {
                SetRendererColor(PreviewBuildingRangeIndicatorSpriteRenderer, RangePreviewColor_Inner);
                break;
            }
            case PreviewBuildingRangeGridType.Outer:
            {
                SetRendererColor(PreviewBuildingRangeIndicatorSpriteRenderer, RangePreviewColor_Outer);
                break;
            }
            case PreviewBuildingRangeGridType.Forbid:
            {
                SetRendererColor(PreviewBuildingRangeIndicatorSpriteRenderer, RangePreviewColor_Forbid);
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

    #region TerrainType

    [SerializeField]
    private GameObject Terrain_Gold_1;

    [SerializeField]
    private GameObject Terrain_Gold_2;

    [SerializeField]
    private GameObject Terrain_Gold_3;

    [SerializeField]
    private GameObject Terrain_Rock;

    private Dictionary<TerrainType, GameObject> TerrainModelDict = new Dictionary<TerrainType, GameObject>();

    [SerializeField]
    private float goldValue = 0;

    public float GoldValue
    {
        get { return goldValue; }
        set
        {
            value = Mathf.Clamp(value, 0, float.MaxValue);
            if (!goldValue.Equals(value))
            {
                goldValue = value;
                int goldLevel = Mathf.CeilToInt(goldValue / LevelManager.Instance.City.CityConfig.GoldValuePerLevel);
                TerrainType = GoldLevelToTerrainType(goldLevel);
            }
        }
    }

    private TerrainType _terrainType = TerrainType.None;

    public TerrainType TerrainType
    {
        get { return _terrainType; }
        private set
        {
            if (_terrainType != value)
            {
                _terrainType = value;
                OnTerrainTypeChanged(_terrainType);
            }
        }
    }

    private void InitTerrain(TerrainType terrainType)
    {
        TerrainModelDict.Add(TerrainType.Gold_1, Terrain_Gold_1);
        TerrainModelDict.Add(TerrainType.Gold_2, Terrain_Gold_2);
        TerrainModelDict.Add(TerrainType.Gold_3, Terrain_Gold_3);
        TerrainModelDict.Add(TerrainType.Rock, Terrain_Rock);

        _terrainType = terrainType;
        GoldValue = TerrainTypeToGoldLevel(terrainType) * LevelManager.Instance.City.CityConfig.GoldValuePerLevel;
        OnTerrainTypeChanged(TerrainType);
    }

    public void OnTerrainTypeChanged(TerrainType terrainType)
    {
        foreach (KeyValuePair<TerrainType, GameObject> kv in TerrainModelDict)
        {
            kv.Value.SetActive(kv.Key == terrainType);
        }
    }

    public static BuildingCoverMaskGroup TerrainTypeToMask(TerrainType tt)
    {
        switch (tt)
        {
            case TerrainType.None:
            {
                return BuildingCoverMaskGroup.None;
            }
            case TerrainType.Gold_1:
            {
                BuildingCoverMaskGroup mask = new BuildingCoverMaskGroup();
                mask.Mask_Part2 = BuildingCoverMask_Part2.Gold_1;
                return mask;
            }
            case TerrainType.Gold_2:
            {
                BuildingCoverMaskGroup mask = new BuildingCoverMaskGroup();
                mask.Mask_Part2 = BuildingCoverMask_Part2.Gold_2;
                return mask;
            }
            case TerrainType.Gold_3:
            {
                BuildingCoverMaskGroup mask = new BuildingCoverMaskGroup();
                mask.Mask_Part2 = BuildingCoverMask_Part2.Gold_3;
                return mask;
            }
            case TerrainType.Rock:
            {
                BuildingCoverMaskGroup mask = new BuildingCoverMaskGroup();
                mask.Mask_Part2 = BuildingCoverMask_Part2.Rock;
                return mask;
            }
        }

        return BuildingCoverMaskGroup.None;
    }

    private static int TerrainTypeToGoldLevel(TerrainType tt)
    {
        switch (tt)
        {
            case TerrainType.None:
            {
                return 0;
            }
            case TerrainType.Gold_1:
            {
                return 1;
            }
            case TerrainType.Gold_2:
            {
                return 2;
            }
            case TerrainType.Gold_3:
            {
                return 3;
            }
            case TerrainType.Rock:
            {
                return 0;
            }
        }

        return 0;
    }

    public static TerrainType GoldLevelToTerrainType(int goldLevel)
    {
        switch (goldLevel)
        {
            case 0:
            {
                return TerrainType.None;
            }
            case 1:
            {
                return TerrainType.Gold_1;
            }
            case 2:
            {
                return TerrainType.Gold_2;
            }
            case 3:
            {
                return TerrainType.Gold_3;
            }
        }

        return 0;
    }

    #endregion
}