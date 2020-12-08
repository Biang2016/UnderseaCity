using UnityEngine;
using System.Collections.Generic;
using BiangStudio.AdvancedInventory;
using BiangStudio.GamePlay.UI;
using UnityEngine.UI;

public class BuildingSelectPanel : BaseUIPanel
{
    [SerializeField]
    private GameObject BuildingDetailedInfoPanel;

    [SerializeField]
    private Image BuildingImage;

    [SerializeField]
    private Text BuildingNameText;

    [SerializeField]
    private Text BuildingBasicDescText;

    [SerializeField]
    private Text BuildingDetailedDescText_Requires;

    [SerializeField]
    private Text BuildingDetailedDescText_Provides;

    void Awake()
    {
        UIType.InitUIType(
            false,
            false,
            false,
            UIFormTypes.Fixed,
            UIFormShowModes.Normal,
            UIFormLucencyTypes.Penetrable);
        BuildingDetailedInfoPanel.SetActive(false);
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

    public void Init(List<BuildingInfo> buildingInfoList)
    {
        foreach (BuildingInfo buildingInfo in buildingInfoList)
        {
            BuildingButton bb = GameObjectPoolManager.Instance.PoolDict[GameObjectPoolManager.PrefabNames.BuildingButton].AllocateGameObject<BuildingButton>(ButtonContainer);
            bb.Initialize(buildingInfo);
            BuildingButtonDict.Add(buildingInfo.buildingKey, bb);
            BuildingKey buildingKey = buildingInfo.buildingKey;
            bb.OnMouseEnterAndLeave.PointerEnter.AddListener(() => OnButtonMouseHover(buildingKey));
            bb.OnMouseEnterAndLeave.PointerExit.AddListener(() => OnButtonMouseLeave(buildingKey));
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

    public void OnButtonMouseHover(BuildingKey buildingKey)
    {
        BuildingDetailedInfoPanel.SetActive(true);
        BuildingInfo buildingInfo = LevelManager.Instance.BuildingInfoDict[buildingKey];
        BuildingImage.sprite = buildingInfo.ItemSprite;
        BuildingNameText.text = buildingInfo.englishName;
        BuildingBasicDescText.text = buildingInfo.ItemBasicInfo;
        BuildingDetailedDescText_Requires.text = buildingInfo.ItemDetailedInfo_Requires;
        BuildingDetailedDescText_Provides.text = buildingInfo.ItemDetailedInfo_Provides;
    }

    public void OnButtonMouseLeave(BuildingKey buildingKey)
    {
        BuildingDetailedInfoPanel.SetActive(false);
    }
}