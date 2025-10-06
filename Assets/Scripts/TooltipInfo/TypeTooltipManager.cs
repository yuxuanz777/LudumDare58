// TypeTooltipManager.cs
using UnityEngine;
using TMPro;

public class TypeTooltipManager : MonoBehaviour
{
    public static TypeTooltipManager Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] Canvas rootCanvas;            // 拖 Canvas
    [SerializeField] RectTransform tooltipPanel;   // 拖 TooltipPanel
    [SerializeField] TMP_Text tooltipText;         // 拖 TooltipText

    [Header("Style")]
    [SerializeField] float xMargin = 16f;          // 与图标右侧的间距
    [SerializeField] float yOffset = 0f;

    void Awake()
    {
        Instance = this;
        tooltipPanel.gameObject.SetActive(false);
    }

    public void ShowTooltip(string typeName, RectTransform iconRT)
    {
        tooltipText.text = GetDescription(typeName);

        var cam = rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera;

        // 获取 icon 世界右侧中点
        Vector3[] corners = new Vector3[4];
        iconRT.GetWorldCorners(corners);
        Vector3 rightCenterWorld = (corners[2] + corners[3]) * 0.5f; // TR + BR
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(cam, rightCenterWorld);
        screenPos.x += 20f; // 向右偏移

        // 转换为 Canvas 局部坐标
        RectTransform canvasRT = rootCanvas.GetComponent<RectTransform>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRT, screenPos, cam, out var localPoint);
        tooltipPanel.anchoredPosition = localPoint;

        tooltipPanel.gameObject.SetActive(true);
    }


    public void HideTooltip() => tooltipPanel.gameObject.SetActive(false);

    string GetDescription(string type)
    {
        switch (type)
        {
            case "RPG": return "<b>RPG</b>: Each owned RPG increases the value of all RPG games by 10%.";
            case "Strategy": return "<b>Strategy</b>: Gains +10 value for each turn passed since purchase.";
            case "Puzzle": return "<b>Puzzle</b>: Value is hidden on purchase, then revealed (10–1000) after the turn ends.";
            case "Simulation": return "<b>Simulation</b>: Shows next turn’s fluctuation in advance.";
            case "Horror": return "<b>Horror</b>: Small chance to skip the goal check — 'You scared the library police away!'.";
            default: return "";
        }
    }
}
