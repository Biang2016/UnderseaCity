using BiangStudio.CloneVariant;
using Sirenix.OdinInspector;
using UnityEngine;

public class BuildingConfig : IClone<BuildingConfig>
{
    [ReadOnly]
    [LabelText("建筑名称")]
    public string BuildingKey;

    [VerticalGroup("按钮主题颜色")]
    [TableColumnWidth(100, true)]
    [HideLabel]
    [Required]
    public Color Color;

    [VerticalGroup("建筑名称(英)")]
    [TableColumnWidth(100, true)]
    [HideLabel]
    [Required]
    public string EnglishName;

    [VerticalGroup("建筑名称(中)")]
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

    [ReadOnly]
    [LabelText("建筑类型")]
    public BuildingType BuildingType;

    [ReadOnly]
    [LabelText("图片Key")]
    public string ItemSpriteKey;

    public BuildingConfig Clone()
    {
        BuildingConfig newConfig = new BuildingConfig();
        newConfig.BuildingKey = BuildingKey;
        newConfig.Color = Color;
        newConfig.EnglishName = EnglishName;
        newConfig.ChineseName = ChineseName;
        newConfig.ItemBasicInfo = ItemBasicInfo;
        newConfig.ItemDetailedInfo = ItemDetailedInfo;
        newConfig.BuildingType = BuildingType;
        newConfig.ItemSpriteKey = ItemSpriteKey;
        return newConfig;
    }
}