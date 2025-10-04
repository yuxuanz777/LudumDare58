using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketCardView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text priceText;
    [SerializeField] TMP_Text tagText;
    [SerializeField] Button buyButton;

    GameItems boundItem;
    Action<GameItems> onBuy;

    public void Bind(GameItems item, Action<GameItems> onBuyCallback, bool canAfford)
    {
        boundItem = item;
        onBuy = onBuyCallback;
        nameText.text = item.name;
        priceText.text = $"Price: ${item.price}";
        tagText.text = item.genre;
        buyButton.interactable = canAfford;
        buyButton.onClick.RemoveAllListeners();
        buyButton.onClick.AddListener(() => onBuy?.Invoke(boundItem));
    }

    public void SetInteractable(bool interactable)
    {
        buyButton.interactable = interactable;
    }
}
