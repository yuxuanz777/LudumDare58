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

    GameItem boundItem;
    Action<GameItem> onBuy;

    public void Bind(GameItem item, Action<GameItem> onBuyCallback, bool canAfford)
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
