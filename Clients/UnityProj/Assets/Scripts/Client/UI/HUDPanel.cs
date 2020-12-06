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
    private Text OxygenValueText;

    public void SetBudget(int budget)
    {
        BudgetValueText.text = budget.ToString();
    }

    public void SetOxygen(int oxygen)
    {
        OxygenValueText.text = oxygen.ToString();
    }
}