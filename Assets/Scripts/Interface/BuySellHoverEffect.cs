using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; 

public class ButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Main References")]
    public Image buttonImage;
    public TextMeshProUGUI textTMP;  

    [Header("Base Colors")]
    public Color normalColor;
    public Color hoverColor;
    private Color textNormalColor;
    private Color textHoverColor;

    [Header("Highlight Settings")]
    [SerializeField, Range(0f, 0.6f)] float valueBoost = 0.25f;          // Increases brightness (V in HSV)
    [SerializeField, Range(0.5f, 1.5f)] float saturationMultiplier = 0.95f; // Slightly reduce or boost saturation
    [SerializeField, Range(0f, 1f)] float whiteLerp = 0.15f;             // Blend toward white for a clean highlight
    [SerializeField] bool recalcEveryEnable = true;

    void Start()
    {
        if (buttonImage == null)
            buttonImage = GetComponent<Image>();

        // 自动查找子物体里的 TextMeshPro
        if (textTMP == null)
            textTMP = GetComponentInChildren<TextMeshProUGUI>();

        normalColor = buttonImage.color;
        if (textTMP != null)
            textNormalColor = textTMP.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // === 背景色变亮 ===
        Color.RGBToHSV(normalColor, out float h, out float s, out float v);
        v = Mathf.Min(1f, v + valueBoost);
        s = Mathf.Clamp01(s * saturationMultiplier);
        var boosted = Color.HSVToRGB(h, s, v);
        hoverColor = Color.Lerp(boosted, Color.white, whiteLerp);
        buttonImage.color = hoverColor;

        // === 文字高亮 ===
        if (textTMP != null)
        {
            // 提升亮度 + 稍微偏白
            Color.RGBToHSV(textNormalColor, out float ht, out float st, out float vt);
            vt = Mathf.Min(1f, vt + 0.3f);
            textHoverColor = Color.Lerp(Color.HSVToRGB(ht, st * 0.9f, vt), Color.white, 0.2f);
            textTMP.color = textHoverColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 还原颜色
        buttonImage.color = normalColor;
        if (textTMP != null)
            textTMP.color = textNormalColor;
    }
}
