using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LibraryRowView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text tagText;   
    [SerializeField] TMP_Text valueText;
    [SerializeField] Button sellButton;

    GameItems boundItem;
    Action<GameItems> onSell;

    public void Bind(GameItems item, Action<GameItems> onSellCallback)
    {
        boundItem = item;
        onSell = onSellCallback;

        nameText.text = item.name;
        tagText.text = item.genre; 
        SetValue(item.value);

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => onSell?.Invoke(boundItem));
    }

    public void SetValue(int v)
    {
        valueText.text = $"${v}";
        valueText.color = v >= 0 ? new Color(0.4f, 0.9f, 0.5f) : new Color(1f, 0.4f, 0.4f);
    }
}
