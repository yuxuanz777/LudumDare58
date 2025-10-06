using TMPro;
using UnityEngine;

public class VictoryGradientPulse : MonoBehaviour
{
    public TMP_Text targetText;
    public Color topColorA = new Color32(179, 136, 255, 255); // 紫色 #B388FF
    public Color bottomColorA = new Color32(255, 215, 0, 255); // 金色 #FFD700
    public Color topColorB = Color.white;   // 变亮目标
    public Color bottomColorB = new Color32(255, 230, 128, 255); // 淡金亮光
    public float speed = 2f;

    void Update()
    {
        if (!targetText) return;
        // 动态计算0~1之间的sin波
        float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;

        // 混合上下颜色
        var cg = new VertexGradient(
            Color.Lerp(topColorA, topColorB, t),
            Color.Lerp(topColorA, topColorB, t),
            Color.Lerp(bottomColorA, bottomColorB, t),
            Color.Lerp(bottomColorA, bottomColorB, t)
        );

        targetText.colorGradient = cg;
    }
}
