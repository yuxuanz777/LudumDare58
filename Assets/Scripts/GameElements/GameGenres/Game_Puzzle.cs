using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game_Puzzle : GameItems
{
    public bool isRevealed;       // 是否已揭晓
    public int revealedTurn;      // 揭晓的回合
    public int finalValue;        // 确定后的真实初始值

    public Game_Puzzle(string id, string name, int price, int baseValue)
        : base(id, name, price, baseValue, "Puzzle")
    {
        this.value = baseValue;
        this.isRevealed = false;
        this.finalValue = 0;
    }

    public override int GetCurrentValue(List<GameItems> library, int n = 0)
    {
        // 若还未揭晓，显示“？”或随机波动值
        if (!isRevealed)
        {
            // 显示乱码时返回伪随机值（仅用于视觉，不改变状态）
            return Random.Range(0, 999);
        }

        // 揭晓后正常返回固定值
        return finalValue;
    }

    // 在 EndTurn 时调用，执行揭晓逻辑
    public void RevealValue()
    {
        if (isRevealed) return;

        isRevealed = true;
        revealedTurn = GameController.Instance._progress.CurrentTurn;

        // 随机抽一个 10~1000 的值（高值概率更低）
        finalValue = GetWeightedRandomValue();
        value = finalValue; // 用作后续基础值
        value = finalValue;
    }

    int GetWeightedRandomValue()
    {
        // 简单权重概率模型：小数多，大数稀有
        float r = Random.value;
        if (r < 0.5f) return Random.Range(10, 200);
        if (r < 0.8f) return Random.Range(200, 500);
        if (r < 0.95f) return Random.Range(500, 800);
        return Random.Range(800, 1001); // 极少出现高价
    }
}
