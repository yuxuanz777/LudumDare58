using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game_Strategy : GameItems
{
    public int purchaseTurn; // 记录购买时的回合数

    public Game_Strategy(string id, string name, int price, int baseValue, int purchaseTurn)
        : base(id, name, price, baseValue, "Strategy")
    {
        this.value = baseValue;
        this.purchaseTurn = purchaseTurn;
    }

    public override int GetCurrentValue(List<GameItems> library, int n = 0)
    {
        // 从 GameController 单例拿当前回合
        int currentTurn = GameController.Instance != null ? GameController.Instance._progress.CurrentTurn : purchaseTurn;
        int turnDiff = Mathf.Max(0, currentTurn - purchaseTurn);
        return value + 10 * turnDiff;
    }
}
