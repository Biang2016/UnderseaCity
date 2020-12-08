using System.Collections.Generic;
using BiangStudio.DragHover;
using BiangStudio.GamePlay.UI;
using UnityEngine;

public class CitySetup : MonoBehaviour
{
    public GameObject CityInventoryVirtualOccupationQuadPrefab;
    public GameObject CityInventoryGridPrefab;

    private CityInventory CityInventory;

    public City City;

    [SerializeField]
    private CityConfig CityConfig;

    public List<Building> BuildingPrefabList = new List<Building>();

    [SerializeField]
    private LayerMask BuildingLayerMask;

    [SerializeField]
    private string InventoryName;

    [SerializeField]
    private int Rows;

    [SerializeField]
    private int Columns;

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        foreach (Building buildingPrefab in BuildingPrefabList)
        {
            LevelManager.Instance.BuildingPrefabDict.Add(buildingPrefab.BuildingInfo.buildingKey, buildingPrefab);
            LevelManager.Instance.BuildingInfoDict.Add(buildingPrefab.BuildingInfo.buildingKey, buildingPrefab.BuildingInfo);
        }

        DragProcessor<Building> DragProcessor_Building = new DragProcessor<Building>();
        DragProcessor_Building.Init(
            UIManager.Instance.UICamera,
            BuildingLayerMask.value,
            (out Vector2 mouseScreenPos) =>
            {
                mouseScreenPos = Input.mousePosition;
                return true;
            },
            City.CityEditAreaIndicator.GetMousePosOnThisArea,
            delegate(Building building, Collider collider, DragProcessor dragProcessor) { },
            delegate(Building building, Collider collider, DragProcessor dragProcessor) { }
        );
        CityInventory = new CityInventory(
            InventoryName,
            City.CityEditAreaIndicator,
            DragProcessor_Building,
            gridSize: ConfigManager.GRID_SIZE, // UI units
            rows: Rows,
            columns: Columns,
            x_Mirror: false,
            z_Mirror: false,
            unlockedPartialGrids: false,
            unlockedGridCount: Rows * Columns,
            dragOutDrop: false,
            rotateItemKeyDownHandler: () => Input.GetKeyDown(KeyCode.R), // Rotate building
            instantiateCityInventoryVirtualOccupationQuadHandler: (parent) => Instantiate(CityInventoryVirtualOccupationQuadPrefab, parent).GetComponent<CityInventoryVirtualOccupationQuad>(),
            instantiateCityInventoryGridHandler: (parent) => Instantiate(CityInventoryGridPrefab, parent).GetComponent<CityInventoryGrid>()
        );

        CityInventory.ToggleDebugKeyDownHandler = () => Input.GetKeyDown(KeyCode.BackQuote); // Toggle debug log
        City.Init(CityInventory, CityConfig);
        CityInventory.City = City;
    }
}