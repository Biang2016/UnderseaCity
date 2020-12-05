using System;
using System.Collections.Generic;
using System.IO;
using BiangStudio;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.GamePlay;
using BiangStudio.ObjectPool;
using BiangStudio.AdvancedInventory;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Building : PoolObject, IMouseHoverComponent, IDraggable
{
    [PropertyOrder(-10)]
    [Title("Data")]
    public BuildingInfo BuildingInfo;

    [ReadOnly]
    [PropertyOrder(-10)]
    [HideInEditorMode]
    [LabelText("父City")]
    public City City = null;

    [TitleGroup("GameObjectContainers")]
    [PropertyOrder(-9)]
    [LabelText("Grid根节点")]
    public BuildingGridRoot BuildingGridRoot;

    [TitleGroup("GameObjectContainers")]
    [PropertyOrder(-9)]
    [LabelText("模型根节点")]
    public BuildingModelRoot BuildingModelRoot;

    internal Draggable Draggable;

    internal CityInfo CityInfo => City.CityInfo;
    internal Inventory Inventory => BuildingInfo.InventoryItem.Inventory;
    internal InventoryItem InventoryItem => BuildingInfo.InventoryItem;
    internal UnityAction<Building> OnRemoveBuildingSuc;

    void Awake()
    {
        Draggable = GetComponent<Draggable>();
    }

    public override void OnRecycled()
    {
        BuildingGridRoot.SetIsolatedIndicatorShown(true);
        BuildingGridRoot.SetInUsed(false);
        BuildingModelRoot.ResetColor();
        BuildingInfo?.Reset();
        BuildingInfo = null;
        City = null;
        OnRemoveBuildingSuc = null;
        base.OnRecycled();
    }

    protected virtual void Update()
    {
        //if (!Application.isPlaying) // 编辑器中不允许调整位置
        //{
        //    if (!GetComponentInParent<MechaDesignerHelper>())
        //    {
        //        transform.localPosition = Vector3.zero;
        //        transform.localRotation = Quaternion.identity;
        //    }
        //}
    }

    public static Building BaseInitialize(BuildingInfo buildingInfo, City parentCity)
    {
        Building building = GameObjectPoolManager.Instance.BuildingBasePoolDict[buildingInfo.BuildingConfig.BuildingKey]
            .AllocateGameObject<Building>(LevelManager.Instance.transform);
        building.Initialize(buildingInfo, parentCity);
        return building;
    }

    private void Initialize(BuildingInfo buildingInfo, City parentCity)
    {
        buildingInfo.OnRemoveBuildingInfoSuc += (mci) =>
        {
            OnRemoveBuildingSuc?.Invoke(this);
            PoolRecycle();
        };

        {
            buildingInfo.InventoryItem.OnSetGridPosHandler = (gridPos_World) =>
            {
                GridPosR.ApplyGridPosToLocalTrans(gridPos_World, transform, ConfigManager.GRID_SIZE);
                CityInfo?.CityInventory.RefreshConflictAndIsolation();
            };
            buildingInfo.InventoryItem.OnIsolatedHandler = (shown) =>
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
            };
            buildingInfo.InventoryItem.OnConflictedHandler = BuildingGridRoot.SetGridConflicted;
            buildingInfo.InventoryItem.OnResetConflictHandler = BuildingGridRoot.ResetAllGridConflict;
        }

        BuildingInfo = buildingInfo;
        GridPos.ApplyGridPosToLocalTransXZ(BuildingInfo.InventoryItem.GridPos_World, transform, ConfigManager.GRID_SIZE);
        City = parentCity;
        BuildingGridRoot.SetInUsed(true);
    }

    private void HighLightColorChange(Color highLightColor, float intensity)
    {
        BuildingModelRoot.SetDefaultHighLightEmissionColor(highLightColor * intensity);
    }

#if UNITY_EDITOR

    [HideInPlayMode]
    [TitleGroup("Editor Params")]
    [LabelText("拍照角度")]
    public float ScreenShotAngle;

    [MenuItem("开发工具/配置/序列化建筑占位")]
    public static void SerializeBuildingOccupiedPositions()
    {
        PrefabManager.Instance.LoadPrefabs();
        Dictionary<string, BuildingOriginalOccupiedGridInfo> dict = new Dictionary<string, BuildingOriginalOccupiedGridInfo>();
        List<Building> buildings = new List<Building>();
        ConfigManager.LoadAllConfigs();
        try
        {
            foreach (KeyValuePair<string, BuildingConfig> kv in ConfigManager.BuildingConfigDict)
            {
                GameObject prefab = PrefabManager.Instance.GetPrefab(kv.Key);
                if (prefab != null)
                {
                    Debug.Log($"建筑占位序列化成功: <color=\"#00ADFF\">{kv.Key}</color>");
                    Building building = Instantiate(prefab).GetComponent<Building>();
                    buildings.Add(building);
                    BuildingOriginalOccupiedGridInfo info = new BuildingOriginalOccupiedGridInfo();
                    info.BuildingOccupiedGridPositionList = building.BuildingGridRoot.GetOccupiedPositions();
                    dict.Add(kv.Key, info);
                }
            }

            if (!Directory.Exists(ConfigManager.BuildingOriginalOccupiedGridInfoJsonFileFolder))
            {
                Directory.CreateDirectory(ConfigManager.BuildingOriginalOccupiedGridInfoJsonFileFolder);
            }

            string json = JsonConvert.SerializeObject(dict, Formatting.Indented);
            StreamWriter sw = new StreamWriter(ConfigManager.BuildingOriginalOccupiedGridInfoJsonFilePath);
            sw.Write(json);
            sw.Close();
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
        finally
        {
            foreach (Building building in buildings)
            {
                DestroyImmediate(building.gameObject);
            }
        }
    }

#endif

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