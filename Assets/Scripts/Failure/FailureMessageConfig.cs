using UnityEngine;

[CreateAssetMenu(fileName = "FailureMessageConfig", menuName = "Configs/Failure Message")]
public class FailureMessageConfig : ScriptableObject
{
    [Header("Animation")]
    public float fadeDuration = 0.4f;
    public AnimationCurve fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    public bool pulse = true;
    [Range(1f, 2f)] public float pulseScale = 1.08f;
    public float pulseDuration = 0.25f;
    public float pulseRecoverDuration = 0.18f;

    [Header("Colors")]
    public Color targetColor = new(1f, 0.35f, 0f, 1f);
    public bool keepTargetColor = true;
}