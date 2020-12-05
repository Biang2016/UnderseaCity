using BiangStudio.ObjectPool;
using UnityEngine.UI;

public class BuildingButton : PoolObject
{
    public Button Button;
    public Text Text;

    public override void OnRecycled()
    {
        base.OnRecycled();
        BuildingConfig = null;
    }

    private BuildingConfig BuildingConfig;

    public void Initialize(BuildingConfig buildingConfig)
    {
        BuildingConfig = buildingConfig;
        Button.image.color = buildingConfig.Color;
        Button.onClick.AddListener(() => { LevelManager.Instance.OnClickSelectBuildingButton(buildingConfig.BuildingKey); });
        Text.text = buildingConfig.EnglishName;
    }
}