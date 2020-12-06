using BiangStudio.ObjectPool;
using UnityEngine.UI;

public class BuildingButton : PoolObject
{
    public Button Button;
    public Text Text;

    public BuildingInfo BuildingInfo;

    public override void OnRecycled()
    {
        base.OnRecycled();
        BuildingInfo = null;
    }

    public void Initialize(BuildingInfo buildingInfo)
    {
        BuildingInfo = buildingInfo;
        Button.image.color = buildingInfo.color;
        Button.onClick.AddListener(() => { LevelManager.Instance.OnClickSelectBuildingButton(buildingInfo.buildingKey); });
        Text.text = buildingInfo.englishName;
    }
}