using System.Collections.Generic;
using BiangStudio.GameDataFormat.Grid;
using UnityEngine;

public class CityEditAreaGridRoot : MonoBehaviour
{
    private List<CityEditorAreaGrid> CityEditorAreaGrids = new List<CityEditorAreaGrid>();

    public void Init()
    {
        if (ConfigManager.ShowMechaEditorAreaGridPosText)
        {
            for (int col = -ConfigManager.EDIT_AREA_HALF_SIZE; col <= ConfigManager.EDIT_AREA_HALF_SIZE; col++)
            {
                for (int row = -ConfigManager.EDIT_AREA_HALF_SIZE; row <= ConfigManager.EDIT_AREA_HALF_SIZE; row++)
                {
                    CityEditorAreaGrid grid = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.CityEditorAreaGrid].AllocateGameObject<CityEditorAreaGrid>(transform);
                    grid.transform.localPosition = new Vector3(col * ConfigManager.GRID_SIZE, 0, row * ConfigManager.GRID_SIZE);
                    grid.transform.localRotation = Quaternion.Euler(90, 0, 0);
                    grid.Init(new GridPos(col + ConfigManager.EDIT_AREA_HALF_SIZE, row + ConfigManager.EDIT_AREA_HALF_SIZE));
                    CityEditorAreaGrids.Add(grid);
                }
            }
        }
    }

    public void Clear()
    {
        foreach (CityEditorAreaGrid grid in CityEditorAreaGrids)
        {
            grid.PoolRecycle();
        }

        CityEditorAreaGrids.Clear();
    }
}