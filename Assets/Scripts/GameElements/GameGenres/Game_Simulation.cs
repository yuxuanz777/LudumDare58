using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game_Simulation : GameItems
{
    public Game_Simulation(string id, string name, int price, int baseValue)
        : base(id, name, price, baseValue, "Simulation")
    {
        // 预测值会由 LibraryService 或 GameController 初始化，不在这里生成
    }

    // 与普通游戏一样返回当前实际 value
    public override int GetCurrentValue(List<GameItems> library, int n = 0)
    {
        return value;
    }

    // 可选：提供一个专用接口供 UI 调用以显示预测
    //public string GetPredictionDisplay()
    //{
    //    int predict = predictedDelta;
    //    string sign = predict >= 0 ? "+" : "";
    //    return $"{sign}{predict}";
    //}
}
