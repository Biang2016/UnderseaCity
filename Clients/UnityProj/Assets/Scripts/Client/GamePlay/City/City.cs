using UnityEngine;
using System.Collections.Generic;
using BiangStudio.AdvancedInventory;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine.Events;

public class City : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer MeshRenderer_Range;

    [SerializeField]
    private MeshRenderer MeshRenderer_Grid;

    [HideInInspector]
    public CityInventory CityInventory;

    public Transform BuildingContainer;

    public CityInventoryVirtualOccupationQuadRoot CityInventoryVirtualOccupationQuadRoot;

    public CityEditAreaIndicator CityEditAreaIndicator;

    public Transform CityInventoryGridContainer;

    private CityInventoryGrid[,] cityInventoryGirdMatrix; // column, row

    public Dictionary<string, Building> BuildingPrefabDict = new Dictionary<string, Building>();
    public Dictionary<string, BuildingInfo> BuildingInfoDict = new Dictionary<string, BuildingInfo>();

    public SortedDictionary<uint, Building> BuildingDict = new SortedDictionary<uint, Building>();

    public UnityAction<Building> OnHoverBuilding;
    public UnityAction<Building> OnHoverEndBuilding;

    public void Init(CityInventory cityInventory, List<Building> buildingPrefabs, UnityAction<Building> onHoverBuilding = null, UnityAction<Building> onHoverEndBuilding = null)
    {
        CityInventory = cityInventory;

        foreach (Building buildingPrefab in buildingPrefabs)
        {
            BuildingPrefabDict.Add(buildingPrefab.BuildingInfo.buildingKey, buildingPrefab);
            BuildingInfoDict.Add(buildingPrefab.BuildingInfo.buildingKey, buildingPrefab.BuildingInfo.Clone());
        }

        OnHoverBuilding = onHoverBuilding;
        OnHoverEndBuilding = onHoverEndBuilding;
        CityInventory.RefreshInventoryGrids();
        CityInventory.RefreshConflictAndIsolation();

        CityEditAreaIndicator.Init(CityInventory);
        cityInventoryGirdMatrix = new CityInventoryGrid[cityInventory.Columns, cityInventory.Rows];
        for (int col = 0; col < cityInventory.Columns; col++)
        {
            for (int row = 0; row < cityInventory.Rows; row++)
            {
                CityInventoryGrid grid = cityInventory.CreateCityInventoryGrid(CityInventoryGridContainer);
                grid.transform.localPosition = new Vector3((col - cityInventory.Columns / 2) * cityInventory.GridSize, 0, (row - cityInventory.Rows / 2) * cityInventory.GridSize);
                grid.transform.localRotation = Quaternion.Euler(90, 0, 0);
                grid.Init(new GridPos(col, row));
                cityInventoryGirdMatrix[col, row] = grid;
            }
        }

        CityInventory.OnAddItemSucAction = OnAddBuildingSuc;
        CityInventory.OnRemoveItemSucAction = OnRemoveBuildingSuc;
        GridShown = false;
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GridShown = !GridShown;
        }
    }

    public Building GetBuilding(uint guid)
    {
        BuildingDict.TryGetValue(guid, out Building building);
        return building;
    }

    private void OnAddBuildingSuc(InventoryItem ii)
    {
        Building building = Instantiate(BuildingPrefabDict[((BuildingInfo) ii.ItemContentInfo).buildingKey], BuildingContainer);
        building.Initialize(this, ii);
        BuildingDict.Add(ii.GUID, building);
    }

    private void OnRemoveBuildingSuc(InventoryItem ii)
    {
        Building building = BuildingDict[ii.GUID];
        Destroy(building.gameObject);
        BuildingDict.Remove(ii.GUID);
    }

    private bool _gridShown = true;

    public bool GridShown
    {
        get { return _gridShown; }
        set
        {
            if (_gridShown != value)
            {
                foreach (KeyValuePair<uint, Building> kv in BuildingDict)
                {
                    kv.Value.BuildingGridRoot.SetGridShown(value);
                }
            }

            _gridShown = value;
        }
    }

    public void SetCityMapShown(bool shown)
    {
        MeshRenderer_Range.enabled = shown;
        MeshRenderer_Grid.enabled = shown;
    }
}