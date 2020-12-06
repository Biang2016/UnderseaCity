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
        foreach (KeyValuePair<BuildingKey, BuildingButton> kv in BuildingButtonDict)
        {
            kv.Value.PoolRecycle();
        }

        BuildingButtonDict.Clear();
    }

    public Transform ButtonContainer;

    private Dictionary<BuildingKey, BuildingButton> BuildingButtonDict = new Dictionary<BuildingKey, BuildingButton>();

    public void Init(List<Building> buildingList)
    {
        foreach (Building buildingPrefab in buildingList)
        {
            BuildingButton bb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BuildingButton].AllocateGameObject<BuildingButton>(ButtonContainer);
            bb.Initialize(buildingPrefab.BuildingInfo);
            BuildingButtonDict.Add(buildingPrefab.BuildingInfo.buildingKey, bb);
        }
    }

    public void OnResourcesChanged(int budget, int oxygen)
    {
        foreach (KeyValuePair<BuildingKey, BuildingButton> kv in BuildingButtonDict)
        {
            kv.Value.Button.interactable = kv.Value.BuildingInfo.BudgetCost <= budget && kv.Value.BuildingInfo.OxygenCost <= oxygen;
        }
    }

    public void SelectButton(BuildingKey buildingKey)
    {
        foreach (KeyValuePair<BuildingKey, BuildingButton> kv in BuildingButtonDict)
        {
            kv.Value.transform.localScale = kv.Key == buildingKey ? 1.2f * Vector3.one : Vector3.one;
        }
    }
}