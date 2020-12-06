using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using TMPro;
using UnityEngine;

public class CityInventoryGrid : PoolObject
{
    [SerializeField]
    private TextMeshPro GridPosText;

    public void Init(GridPos gridPos)
    {
        GridPosText.text = gridPos.ToString();
    }
}