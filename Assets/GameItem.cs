using System;
using UnityEngine;

[Serializable]
public class GameItem
{
    public string id;
    public string name;
    public int price;
    public int value;   // 当前市场价值
    public string genre;

    public GameItem(string id, string name, int price, int value, string genre)
    {
        this.id = id; this.name = name; this.price = price; this.value = value; this.genre = genre;
    }
}
