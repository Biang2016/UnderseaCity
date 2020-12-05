using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using BiangStudio.Singleton;
using UnityEngine;

public class DragExecuteManager : TSingletonBaseManager<DragExecuteManager>
{
    public void Init()
    {
        DragManager.Instance.Init(
            () => Input.GetMouseButtonDown(0),
            () => Input.GetMouseButtonUp(0),
            Debug.LogError,
            LayerManager.Instance.LayerMask_DragAreas);
        DragProcessor<Building> dragProcessor_Building = new DragProcessor<Building>(); // building里面不包含draggable组件，因此不响应拖拽事件
        dragProcessor_Building.Init(
            CameraManager.Instance.MainCamera,
            LayerManager.Instance.LayerMask_BuildingHitBox,
            (out Vector2 mouseScreenPos) =>
            {
                mouseScreenPos = Input.mousePosition;
                return true;
            }
            ,
            ScreenMousePositionToWorld_CityInventory,
            delegate (Building building, Collider collider, DragProcessor dragProcessor) { },
            delegate (Building building, Collider collider, DragProcessor dragProcessor) { }
        );
    }

    private bool ScreenMousePositionToWorld_CityInventory(out Vector3 pos_world, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix)
    {
        if (LevelManager.Instance.City.CityEditAreaIndicator.GetMousePosOnThisArea(out pos_world, out pos_local, out pos_matrix, out gp_matrix))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}