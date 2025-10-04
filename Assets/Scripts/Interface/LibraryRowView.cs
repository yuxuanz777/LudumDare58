using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
public class LibraryRowView : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text tagText;   
    [SerializeField] TMP_Text valueText;
    [SerializeField] Button sellButton;

    GameItems boundItem;
    Action<GameItems> onSell;

    public void Bind(GameItems item, Action<GameItems> onSellCallback, int computedValue, int n)
    {
        boundItem = item;
        onSell = onSellCallback;

        nameText.text = item.name;
        tagText.text = item.genre; 
        SetValue(item, computedValue, item.value, n);

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => onSell?.Invoke(boundItem));

        //StartCoroutine(FlashValue());
    }

    public void SetValue(GameItems item, int v, int baseValue, int n)
    {
        if (item.genre == "RPG" && n > 0)
        {
            valueText.text = $"${baseValue} × (1+{n}×0.1) = ${v}!";
            valueText.color = new Color(0.5f, 0.8f, 1f); // 淡蓝色更显RPG特色
        }
        else if (item.genre == "Strategy")
        {
            var strategy = item as Game_Strategy;
            int turnDiff = GameController.Instance._progress.CurrentTurn - strategy.purchaseTurn;
            valueText.text = $"${baseValue} + (10×{turnDiff}) = ${v}";
            valueText.color = new Color(1f, 0.8f, 0.3f); // 橙色风格
        }
        else if (item.genre == "Puzzle")
        {
            var puzzle = item as Game_Puzzle;
            if (!puzzle.isRevealed)
            {
                // 当回合未揭晓，显示“???” 或乱码
                valueText.text = "???";
                //valueText.text = "!&*#%@*?"[UnityEngine.Random.Range(0, 8)].ToString();
                valueText.color = Color.black;
            }
            else
            {
                valueText.text = $"${puzzle.finalValue}";
                valueText.color = new Color(0.8f, 0.9f, 1f);
            }
        }
        else if (item.genre == "Simulation")
        {
            int currentValue = item.value;
            int predict = item.predictedDelta;

            string sign = predict >= 0 ? "+" : "";
            string predictText = $"{sign}{predict}";
            Color predictColor = predict >= 0 ? new Color(0.5f, 1f, 0.6f) : new Color(1f, 0.5f, 0.5f);

            valueText.text = $"${currentValue}  <color=#{ColorUtility.ToHtmlStringRGB(predictColor)}>({predictText})</color>";
        }
        else if (item.genre == "Horror")
        {
            var horror = item as Game_Horror;
            int currentValue = item.value;
            valueText.text = $"${currentValue}";

            if (horror.abilityTriggeredThisTurn)
            {
                valueText.text += "\n<color=#FF6666>OMG, You scared the library police away!</color>";
            }
        }

        else
        {
            valueText.text = $"${v}";
            valueText.color = v > 0 ? new Color(0.4f, 0.9f, 0.5f) : new Color(1f, 0.4f, 0.4f);
        }
     }
    // 在 LibraryRowView 里加入动画闪烁协程（特殊加成时显示？）
    IEnumerator FlashValue()
    {
        Color c1 = Color.yellow;
        Color c2 = valueText.color;
        for (int i = 0; i < 3; i++)
        {
            valueText.color = c1;
            yield return new WaitForSeconds(0.1f);
            valueText.color = c2;
            yield return new WaitForSeconds(0.1f);
        }
    }

}
