using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Game_Horror : GameItems
{
    public bool abilityTriggeredThisTurn;

    public Game_Horror(string id, string name, int price, int baseValue)
        : base(id, name, price, baseValue, "Horror")
    {
        abilityTriggeredThisTurn = false;
    }

    public override int GetCurrentValue(List<GameItems> library, int n = 0)
    {
        return value;
    }

    // 在 EndTurn 时调用的被动技能触发检测
    public bool TryScareLibraryPolice(float triggerChance = 0.1f)
    {
        abilityTriggeredThisTurn = false;

        if (Random.value < triggerChance)
        {
            abilityTriggeredThisTurn = true;
            return true;
        }
        return false;
    }
}
