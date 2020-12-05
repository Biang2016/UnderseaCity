using UnityEngine;

[CreateAssetMenu(menuName = "Model/ModelTwinkleConfig")]
public class BuildingModelTwinkleConfigSSO : ScriptableObject
{
    public Gradient DamageGradient;
    public AnimationCurve DamageIntensityCurve;

    public float DamageTwinkleDuration;

    //public Gradient PowerGradient;
    public AnimationCurve PowerIntensityCurve;
    public float PowerTwinkleDuration;
}