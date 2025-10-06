using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class EventDisplayAutoLayout : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] HorizontalLayoutGroup layout;
    [SerializeField] Image leftIcon;
    [SerializeField] Image rightIcon;
    [SerializeField] TMP_Text eventText;

    [Header("Default Style")]
    [SerializeField] Vector2 defaultIconSize = new(80, 80);
    [SerializeField] int defaultSpacing = 25;
    [SerializeField] float defaultFontSize = 60;

    [Header("Custom Styles")]
    [SerializeField] Vector2 autumnIconSize = new(100, 100);
    [SerializeField] int autumnSpacing = 30;
    [SerializeField] float autumnFontSize = 65;

    [SerializeField] Vector2 workIconSize = new(70, 70);
    [SerializeField] int workSpacing = 20;
    [SerializeField] float workFontSize = 55;

    RectTransform _leftRect;
    RectTransform _rightRect;

    void Awake()
    {
        if (layout == null) layout = GetComponent<HorizontalLayoutGroup>();
        if (leftIcon != null) _leftRect = leftIcon.GetComponent<RectTransform>();
        if (rightIcon != null) _rightRect = rightIcon.GetComponent<RectTransform>();
    }

    public void ApplyStyle(string eventKey)
    {
        // 默认值
        Vector2 iconSize = defaultIconSize;
        int spacing = defaultSpacing;
        float fontSize = defaultFontSize;

        // 不同事件的个性化样式
        switch (eventKey)
        {
            case "sale":
                iconSize = autumnIconSize;
                spacing = autumnSpacing;
                fontSize = autumnFontSize;
                break;

            case "work":
                iconSize = workIconSize;
                spacing = workSpacing;
                fontSize = workFontSize;
                break;
        }

        // 应用到布局和元素
        if (layout != null)
            layout.spacing = spacing;

        if (_leftRect != null)
        {
            _leftRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize.x);
            _leftRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize.y);
        }

        if (_rightRect != null)
        {
            _rightRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, iconSize.x);
            _rightRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, iconSize.y);
        }

        if (eventText != null)
            eventText.fontSize = fontSize;
    }
}
