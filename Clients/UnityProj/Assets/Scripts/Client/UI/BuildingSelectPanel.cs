using UnityEngine;
using System.Collections.Generic;
using BiangStudio.GamePlay.UI;

public class BuildingSelectPanel : BaseUIPanel
{
    void Awake()
    {
        UIType.InitUIType(
            false,
            false,
            false,
            UIFormTypes.Fixed,
            UIFormShowModes.Normal,
            UIFormLucencyTypes.Penetrable);
    }

    public void Clear()
    {
        foreach (KeyValuePair<string, BuildingButton> kv in BuildingButtonDict)
        {
            kv.Value.PoolRecycle();
        }

        BuildingButtonDict.Clear();
    }

    public Transform ButtonContainer;

    private Dictionary<string, BuildingButton> BuildingButtonDict = new Dictionary<string, BuildingButton>();

    public void Init(List<Building> buildingList)
    {
        foreach (Building buildingPrefab in buildingList)
        {
            BuildingButton bb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BuildingButton].AllocateGameObject<BuildingButton>(ButtonContainer);
            bb.Initialize(buildingPrefab.BuildingInfo);
            BuildingButtonDict.Add(buildingPrefab.BuildingInfo.buildingKey, bb);
        }
    }

    public void ClearSelectButton()
    {
        foreach (KeyValuePair<string, BuildingButton> kv in BuildingButtonDict)
        {
            kv.Value.transform.localScale = Vector3.one;
        }
    }

    public void SelectButton(string buildingKey)
    {
        foreach (KeyValuePair<string, BuildingButton> kv in BuildingButtonDict)
        {
            kv.Value.transform.localScale = kv.Key.Equals(buildingKey) ? 1.2f * Vector3.one : Vector3.one;
        }
    }
}