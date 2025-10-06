using TMPro;
using UnityEngine;

public class VictoryGradientPulse : MonoBehaviour
{
    public TMP_Text targetText;
    public Color topColorA = new Color32(179, 136, 255, 255); // ��ɫ #B388FF
    public Color bottomColorA = new Color32(255, 215, 0, 255); // ��ɫ #FFD700
    public Color topColorB = Color.white;   // ����Ŀ��
    public Color bottomColorB = new Color32(255, 230, 128, 255); // ��������
    public float speed = 2f;

    void Update()
    {
        if (!targetText) return;
        // ��̬����0~1֮���sin��
        float t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;

        // ���������ɫ
        var cg = new VertexGradient(
            Color.Lerp(topColorA, topColorB, t),
            Color.Lerp(topColorA, topColorB, t),
            Color.Lerp(bottomColorA, bottomColorB, t),
            Color.Lerp(bottomColorA, bottomColorB, t)
        );

        targetText.colorGradient = cg;
    }
}
