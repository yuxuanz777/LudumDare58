using System;
using UnityEngine;

[Serializable]
public class GameItems
{
    public string id;
    public string name;
    public int price;
    public int value; 
    public string genre;

    public GameItems(string id, string name, int price, int value, string genre)
    {
        this.id = id; this.name = name; this.price = price; this.value = value; this.genre = genre;
    }
}
