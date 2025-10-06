// TypeIconHover.cs
using UnityEngine;
using UnityEngine.EventSystems;

public class TypeIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] string typeName; // "RPG"/"Strategy"/"Puzzle"/"Simulation"/"Horror"
    RectTransform _rt;

    void Awake() => _rt = transform as RectTransform;

    public void OnPointerEnter(PointerEventData eventData)
        => TypeTooltipManager.Instance.ShowTooltip(typeName, _rt);

    public void OnPointerExit(PointerEventData eventData)
        => TypeTooltipManager.Instance.HideTooltip();
}
