using BiangStudio;
using BiangStudio.DragHover;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CityEditAreaIndicator : DragAreaIndicator
{
    public CityInventory CityInventory;
    private DragProcessor DragProcessor;

    public void Init(CityInventory cityInventory)
    {
        CityInventory = cityInventory;
        DragProcessor = DragManager.Instance.GetDragProcessor<Building>();
    }

    private bool onMouseDrag_Right = false;
    private Vector3 mouseDownPos_Right = Vector3.zero;
    private Vector3 rotateCenter = Vector3.zero;
    private bool onMouseDrag_Left = false;
    private Vector3 mouseDownPos_Left = Vector3.zero;

    public void Update()
    {
        // Mouse Right button drag for rotate view
        if (Input.GetMouseButtonDown(1))
        {
            onMouseDrag_Right = true;
            {
                if (GetMousePosOnThisArea(out Vector3 pos_world, out Vector3 _, out Vector3 _, out GridPos _))
                {
                    mouseDownPos_Right = pos_world;
                }
            }
            {
                if (GetScreenCenterPosOnThisArea(out Vector3 pos_world, out Vector3 _, out Vector3 _, out GridPos _))
                {
                    rotateCenter = pos_world;
                }
            }
        }

        if (onMouseDrag_Right && Input.GetMouseButton(1))
        {
            if (GetMousePosOnThisArea(out Vector3 pos_world, out Vector3 _, out Vector3 _, out GridPos _))
            {
                Vector3 startVec = mouseDownPos_Right - rotateCenter;
                Vector3 endVec = pos_world - rotateCenter;

                float rotateAngle = Vector3.SignedAngle(startVec, endVec, Vector3.up);
                if (Mathf.Abs(rotateAngle) > 3)
                {
                    CameraManager.Instance.FieldCamera.TargetConfigData.HorAngle -= rotateAngle;
                    mouseDownPos_Right = pos_world;
                }
            }
            else
            {
                onMouseDrag_Right = false;
                mouseDownPos_Right = Vector3.zero;
            }
        }

        if (Input.GetMouseButtonUp(1))
        {
            onMouseDrag_Right = false;
            mouseDownPos_Right = Vector3.zero;
        }
    }

    public bool GetMousePosOnThisArea(out Vector3 pos_world, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix)
    {
        return GetPosOnThisArea(DragProcessor.CurrentMousePosition_Screen, out pos_world, out pos_local, out pos_matrix, out gp_matrix);
    }

    public bool GetScreenCenterPosOnThisArea(out Vector3 pos_world, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix)
    {
        return GetPosOnThisArea(new Vector2(Screen.width / 2f, Screen.height / 2f), out pos_world, out pos_local, out pos_matrix, out gp_matrix);
    }

    private bool GetPosOnThisArea(Vector2 screenPos, out Vector3 pos_world, out Vector3 pos_local, out Vector3 pos_matrix, out GridPos gp_matrix)
    {
        pos_world = Vector3.zero;
        pos_local = Vector3.zero;
        pos_matrix = Vector3.zero;
        gp_matrix = GridPos.Zero;
        Ray ray = DragProcessor.Camera.ScreenPointToRay(screenPos);
        Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerManager.Instance.LayerMask_DragAreas);
        if (hit.collider)
        {
            if (hit.collider == BoxCollider)
            {
                pos_world = hit.point;
                Vector3 pos_local_absolute = CityInventory.City.BuildingContainer.transform.InverseTransformPoint(pos_world);
                Vector3 containerSize = new Vector3(CityInventory.Columns * CityInventory.GridSize, 0, CityInventory.Rows * CityInventory.GridSize);
                pos_local = new Vector3(pos_local_absolute.x + containerSize.x / 2f - CityInventory.GridSize / 2f, 0, pos_local_absolute.z + containerSize.z / 2f - CityInventory.GridSize / 2f);
                pos_matrix = new Vector3(pos_local_absolute.x + containerSize.x / 2f, 0, pos_local_absolute.z + containerSize.z / 2f);
                Vector3 pos_matrix_round = new Vector3(pos_matrix.x - CityInventory.GridSize / 2f, 0, pos_matrix.z - CityInventory.GridSize / 2f);
                gp_matrix = GridPos.GetGridPosByPointXZ(pos_matrix_round, CityInventory.GridSize);
                return true;
            }
            else
            {
                return false;
            }
        }

        pos_world = CommonUtils.GetIntersectWithLineAndPlane(ray.origin, ray.direction, Vector3.up, transform.position);
        return false;
    }
}