using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

public class BuildingHitBox : MonoBehaviour
{
    internal BuildingGridRoot ParentGridRootRoot;

    internal City City => ParentGridRootRoot.Building.City;

    internal BoxCollider BoxCollider;

    private void Awake()
    {
        BoxCollider = GetComponentInParent<BoxCollider>();
        ParentGridRootRoot = GetComponentInParent<BuildingGridRoot>();
    }

    private bool InUsed;

    public void SetInUsed(bool inUsed)
    {
        InUsed = inUsed;
        BoxCollider.enabled = inUsed;
    }

    public GridPos LocalGridPos;

    public void Initialize(GridPos localGP)
    {
        LocalGridPos = localGP;
    }
}