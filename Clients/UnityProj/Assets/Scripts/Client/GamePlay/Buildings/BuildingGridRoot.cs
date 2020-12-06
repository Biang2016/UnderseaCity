using System.Collections.Generic;
using System.Linq;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

public class BuildingGridRoot : ForbidLocalMoveRoot
{
    internal Building Building;
    public List<BuildingGrid> buildingGrids = new List<BuildingGrid>();
    public List<BuildingHitBox> HitBoxes = new List<BuildingHitBox>();

    void Awake()
    {
        GetOccupiedPositions();
    }

    public void SetInUsed(bool inUsed)
    {
        foreach (BuildingHitBox hitBox in HitBoxes)
        {
            hitBox.SetInUsed(inUsed);
        }
    }

    internal BuildingHitBox FindHitBox(Collider collider)
    {
        foreach (BuildingHitBox hitBox in HitBoxes)
        {
            if (hitBox.BoxCollider == collider)
            {
                return hitBox;
            }
        }

        return null;
    }

    public List<GridPos> GetOccupiedPositions()
    {
        buildingGrids = GetComponentsInChildren<BuildingGrid>().ToList();
        Building = GetComponentInParent<Building>();
        HitBoxes = GetComponentsInChildren<BuildingHitBox>().ToList();
        foreach (BuildingHitBox hitBox in HitBoxes)
        {
            hitBox.LocalGridPos = GridPos.GetGridPosByLocalTransXZ(hitBox.transform, ConfigManager.GRID_SIZE);
        }

        List<GridPos> res = new List<GridPos>();
        foreach (BuildingGrid bg in buildingGrids)
        {
            res.Add(bg.GetGridPos());
        }

        return res;
    }

    public void SetGridShown(bool shown)
    {
        foreach (BuildingGrid bg in buildingGrids)
        {
            bg.SetGridShown(shown);
        }
    }

    public void SetConflictIndicatorShown(bool shown)
    {
        foreach (BuildingGrid bg in buildingGrids)
        {
            bg.SetForbidIndicatorShown(shown);
        }
    }

    public void SetIsolatedIndicatorShown(bool shown)
    {
        foreach (BuildingGrid bg in buildingGrids)
        {
            bg.SetIsolatedIndicatorShown(shown);
        }
    }

    public void SetGridConflicted(GridPos gridPos)
    {
        foreach (BuildingGrid bg in buildingGrids)
        {
            GridPos gp = bg.GetGridPos();
            if (gp.x == gridPos.x && gp.z == gridPos.z)
            {
                bg.IsConflicted = true;
                bg.SetForbidIndicatorShown(true);
            }
        }
    }

    public void ResetAllGridConflict()
    {
        foreach (BuildingGrid bg in buildingGrids)
        {
            bg.IsConflicted = false;
            bg.SetForbidIndicatorShown(false);
        }
    }
}