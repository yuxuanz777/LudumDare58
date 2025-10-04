using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game_RPG : GameItems
{
    public Game_RPG(string id, string name, int price, int value)
        : base(id, name, price, value, "RPG") { }

 
    public override int GetCurrentValue(List<GameItems> library, int n)
    {

        // 每拥有一款 RPG，所有 RPG 的价值提升 10%
        float multiplier = 1f + 0.1f * n;
        return Mathf.RoundToInt(value * multiplier);
    }
    
}
