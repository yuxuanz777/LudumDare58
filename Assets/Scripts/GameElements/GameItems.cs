using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class GameItems
{
    public string id;
    public string name;
    public int price;
    public int value; 
    public string genre;
    // 新增：预先决定的波动值
    public int predictedDelta;

    public GameItems(string id, string name, int price, int value, string genre)
    {
        this.id = id; this.name = name; this.price = price; this.value = value; this.genre = genre;
        this.predictedDelta = 0;
    }
    // 允许不同类型根据整个库动态调整价值
    public int GetRPGCount(List<GameItems> library)
    {
        int n = 0;
        foreach (var g in library)
            if (g.genre == "RPG")
                n++;
        return n;
    }
    public virtual int GetCurrentValue(List<GameItems> library, int n)
    {
        return value;
    }
}
