using BiangStudio.GameDataFormat.Grid;
using BiangStudio.ObjectPool;
using TMPro;
using UnityEngine;

public class CityEditorAreaGrid : PoolObject
{
    [SerializeField]
    private TextMeshPro GridPosText;

    public void Init(GridPos gridPos)
    {
        GridPosText.text = gridPos.ToString();
    }
}