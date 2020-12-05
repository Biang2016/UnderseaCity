using BiangStudio.Singleton;
using UnityEngine;

public class LayerManager : TSingletonBaseManager<LayerManager>
{
    public int LayerMask_UI;
    public int LayerMask_DragAreas;
    public int LayerMask_BuildingHitBox;

    public int Layer_UI;
    public int Layer_DragAreas;
    public int Layer_BuildingHitBox;

    public override void Awake()
    {
        LayerMask_UI = LayerMask.GetMask("UI");
        LayerMask_DragAreas = LayerMask.GetMask("DragAreas");
        LayerMask_BuildingHitBox = LayerMask.GetMask("BuildingHitBox");

        Layer_UI = LayerMask.NameToLayer("UI");
        Layer_DragAreas = LayerMask.NameToLayer("DragAreas");
        Layer_BuildingHitBox = LayerMask.NameToLayer("BuildingHitBox");
    }
}