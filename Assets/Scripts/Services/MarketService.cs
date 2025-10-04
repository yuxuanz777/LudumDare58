using System.Collections.Generic;
using UnityEngine;

public class MarketService
{
    public IReadOnlyList<GameItems> Items => _items;
    readonly List<GameItems> _items = new List<GameItems>();

    public void Generate(int count)
    {
        _items.Clear();
        for (int i = 0; i < count; i++)
        {
            _items.Add(new GameItems(
                System.Guid.NewGuid().ToString(),
                NameGenerator.FunnyName(),
                Random.Range(50, 300),
                Random.Range(50, 300),
                NameGenerator.RandomGenre()
            ));
        }
    }

    public bool Remove(GameItems item) => _items.Remove(item);
}