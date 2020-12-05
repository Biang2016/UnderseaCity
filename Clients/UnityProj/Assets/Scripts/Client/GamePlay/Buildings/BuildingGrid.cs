using System;
using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using Sirenix.OdinInspector;
using UnityEngine;

[ExecuteInEditMode]
public class BuildingGrid : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer BorderIndicator;

    [SerializeField]
    private MeshRenderer ForbidIndicator;

    [SerializeField]
    private MeshRenderer IsolatedIndicator;

    [SerializeField]
    private BuildingHitBox BuildingHitBox;

    public bool IsConflicted = false;

    void Awake()
    {
    }

    public void SetGridShown(bool shown)
    {
        BorderIndicator.enabled = shown;
    }

    public void SetForbidIndicatorShown(bool shown)
    {
        ForbidIndicator.enabled = IsConflicted && shown;
    }

    public void SetIsolatedIndicatorShown(bool shown)
    {
        IsolatedIndicator.enabled = shown;
    }

    public GridPos GetGridPos()
    {
        return GridPos.GetGridPosByLocalTransXZ(transform, ConfigManager.GRID_SIZE);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0.0f, 0.1f);
        Gizmos.DrawCube(transform.position, Vector3.one);
    }
}