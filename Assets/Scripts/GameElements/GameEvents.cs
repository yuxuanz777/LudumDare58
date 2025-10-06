using System;

public class GameEvent
{
    public string key;
    public string name;
    public Action effect;

    public GameEvent(string key, string name, Action effect)
    {
        this.key = key;
        this.name = name;
        this.effect = effect;
    }
}