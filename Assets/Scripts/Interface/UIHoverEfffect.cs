using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class UIButtonHoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    public Image background;   // 背景图层
    public Image icon;         // 图标层（可选）

    [Header("Hover Colors")]
    public Color hoverBackgroundColor = new Color(1f, 0f, 0f, 0.4f); // 默认红色半透明（Exit）
    public Color hoverIconColor = Color.white;

    [Header("Normal Colors")]
    public Color normalBackgroundColor = new Color(1f, 1f, 1f, 0f);  // 透明
    public Color normalIconColor = new Color(0.8f, 0.8f, 0.8f, 1f); // 灰色

    [Header("Transition Settings")]
    public float fadeDuration = 0.15f;

    private bool isHovered = false;
    private float t = 0f;

    private void Reset()
    {
        // 自动尝试填充
        background = GetComponent<Image>();
        if (transform.childCount > 0)
        {
            icon = transform.GetChild(0).GetComponent<Image>();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        t = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        t = 0f;
    }

    private void Update()
    {
        if (background == null && icon == null)
            return;

        t += Time.deltaTime / fadeDuration;
        float progress = Mathf.Clamp01(t);

        if (isHovered)
        {
            if (background) background.color = Color.Lerp(background.color, hoverBackgroundColor, progress);
            if (icon) icon.color = Color.Lerp(icon.color, hoverIconColor, progress);
        }
        else
        {
            if (background) background.color = Color.Lerp(background.color, normalBackgroundColor, progress);
            if (icon) icon.color = Color.Lerp(icon.color, normalIconColor, progress);
        }
    }
}
