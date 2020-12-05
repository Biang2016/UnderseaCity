using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using static System.String;

[CreateAssetMenu(menuName = "BuildingConfig/AllBuildingConfigSSO")]
public class AllBuildingConfigSSO : SerializedScriptableObject
{
    [LabelText("建筑配置列表（工具）")]
    [ListDrawerSettings(ListElementLabelName = "EditorDisplayName", AddCopiesLastElement = true)]
    [TableList(ShowPaging = false)]
    public List<BuildingConfigRaw> BuildingConfigRawList = new List<BuildingConfigRaw>();

    [LabelText("建筑配置列表")]
    [TableList(ShowPaging = false)]
    public List<BuildingConfig> BuildingConfigList = new List<BuildingConfig>();

    [Serializable]
    public class BuildingConfigRaw
    {
        [VerticalGroup("按钮主题颜色")]
        [TableColumnWidth(100, true)]
        [HideLabel]
        [Required]
        public Color Color;

        [VerticalGroup("名称(英)")]
        [TableColumnWidth(100, true)]
        [HideLabel]
        [Required]
        public string EnglishName;

        [VerticalGroup("名称(中)")]
        [TableColumnWidth(100, true)]
        [HideLabel]
        [Required]
        public string ChineseName;

        [VerticalGroup("基本信息")]
        [TableColumnWidth(100, true)]
        [HideLabel]
        [Required]
        public string ItemBasicInfo;

        [VerticalGroup("详细信息")]
        [TableColumnWidth(100, true)]
        [HideLabel]
        [Required]
        public string ItemDetailedInfo;

        [VerticalGroup("建筑")]
        [TableColumnWidth(300, true)]
        [HideLabel]
        [Required]
        [AssetSelector(Filter = "Building_", Paths = "Assets/Resources/Prefabs/Buildings", FlattenTreeView = false)]
        [AssetsOnly]
        public Building BuildingPrefab;

        [VerticalGroup("建筑类型")]
        [HideLabel]
        [TableColumnWidth(80, true)]
        public BuildingType BuildingType;

        [VerticalGroup("图片")]
        [HideLabel]
        [Required]
        [AssetsOnly]
        [TableColumnWidth(110, true)]
        public Sprite ItemSprite;

        private string EditorDisplayName
        {
            get
            {
                string shortName = BuildingPrefab != null ? BuildingPrefab.name.Replace("MC_", "") : "";
                return $"{shortName}";
            }
        }

        public bool Valid => BuildingPrefab && ItemSprite;
    }

    [Button("刷新、排序")]
    private void OnBuildingConfigRawListChanged()
    {
        BuildingConfigList.Clear();
        foreach (BuildingConfigRaw raw in BuildingConfigRawList)
        {
            if (raw.BuildingPrefab != null &&
                raw.ItemSprite != null
            )
            {
                BuildingConfigList.Add(new BuildingConfig
                {
                    BuildingKey = raw.BuildingPrefab.name,
                    Color = raw.Color,
                    BuildingType = raw.BuildingType,
                    EnglishName = raw.EnglishName,
                    ChineseName = raw.ChineseName,
                    ItemBasicInfo = raw.ItemBasicInfo,
                    ItemDetailedInfo = raw.ItemDetailedInfo,
                    ItemSpriteKey = raw.ItemSprite.name,
                });
            }
        }
    }

    public void RefreshConfigList()
    {
        OnBuildingConfigRawListChanged();
    }
}