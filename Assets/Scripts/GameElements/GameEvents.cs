using System;

public class GameEvent
{
    public string name;
    public Action effect;

    public GameEvent(string name, Action effect)
    {
        this.name = name;
        this.effect = effect;
    }
}