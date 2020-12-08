using BiangStudio;
using BiangStudio.GamePlay.UI;
using UnityEngine;
using UnityEngine.UI;

public class HUDPanel : BaseUIPanel
{
    void Awake()
    {
        UIType.InitUIType(
            false,
            false,
            false,
            UIFormTypes.Normal,
            UIFormShowModes.Normal,
            UIFormLucencyTypes.Penetrable);
    }

    [SerializeField]
    private Text BudgetValueText;

    [SerializeField]
    private Text BudgetCalculateText;

    [SerializeField]
    private Text OxygenValueText;

    [SerializeField]
    private Text OxygenCalculateText;

    [SerializeField]
    private Text PopulationValueText;

    [SerializeField]
    private Slider TimeScaleSlider;

    [SerializeField]
    private Text TimeScaleText;

    public Slider DayProgressBarSlider;
    public Slider PopulationProgressBarSlider;
    public Text PopulationGoalText;

    public void Init()
    {
        TimeScaleSlider.value = 5f / LevelManager.Instance.City.CityConfig.SecondPerMinutes;
        TimeScaleText.text = "1x";
        TimeScaleSlider.onValueChanged.AddListener((value) =>
        {
            if (value.Equals(0))
            {
                LevelManager.Instance.City.CityConfig.SecondPerMinutes = float.MaxValue;
            }
            else
            {
                LevelManager.Instance.City.CityConfig.SecondPerMinutes = 5f / value;
            }

            TimeScaleText.text = $"{TimeScaleSlider.value}x";
        });

        PopulationGoalText.text = LevelManager.Instance.City.CityConfig.PopulationGoal.ToString();
    }

    public void SetBudget(int budget, int production, int consumption)
    {
        string budgetText = (budget < 0 ? CommonUtils.AddHighLightColorToText($"{budget}", "#FF0000") : $"{budget} ");
        if (consumption > production)
        {
            BudgetValueText.text = budgetText + CommonUtils.AddHighLightColorToText($"({production - consumption})", "#FF0000");
        }
        else
        {
            BudgetValueText.text = budgetText + $"(+{production - consumption})";
        }

        BudgetCalculateText.text = $"+{production} ({(LevelManager.Instance.WorkingEfficiency * 100f).ToString("F1")}%)\t   -{consumption}";
    }

    public void SetOxygen(int oxygen, int production, int consumptionCommon, int consumptionForTreasure, int capacity)
    {
        if (consumptionCommon + consumptionForTreasure > production)
        {
            OxygenValueText.text = $"{oxygen}/{capacity} " + CommonUtils.AddHighLightColorToText($"({production - (consumptionCommon + consumptionForTreasure)})", "#FF0000");
        }
        else
        {
            OxygenValueText.text = $"{oxygen}/{capacity} (+{production - (consumptionCommon + consumptionForTreasure)})";
        }

        OxygenCalculateText.text = $"+{production}\t   -{consumptionForTreasure} ({(LevelManager.Instance.WorkingEfficiency * 100f).ToString("F1")}%)\t   -{consumptionCommon}";
    }

    public void SetPopulation(int population, int netIncrease, int capacity)
    {
        PopulationProgressBarSlider.value = (float) population / LevelManager.Instance.City.CityConfig.PopulationGoal;
        if (netIncrease < 0)
        {
            PopulationValueText.text = $"{population}/{capacity} " + CommonUtils.AddHighLightColorToText($"({netIncrease})", "#FF0000");
        }
        else
        {
            PopulationValueText.text = $"{population}/{capacity} (+{netIncrease})";
        }
    }
}