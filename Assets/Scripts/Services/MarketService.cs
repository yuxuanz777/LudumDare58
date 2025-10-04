using System.Collections.Generic;
using UnityEngine;

public class MarketService
{
    //public IReadOnlyList<GameItems> Items => _items;
    public List<GameItems> _items = new List<GameItems>();

    public void Generate(int count)
    {
        _items.Clear();
        for (int i = 0; i < count; i++)
        {
            string genre = NameGenerator.RandomGenre();
            string id = System.Guid.NewGuid().ToString();
            string name = NameGenerator.FunnyName();
            int price = Random.Range(50, 300);
            int baseValue = Random.Range(50, 300);
            switch (genre)
            {
                case "RPG":
                    _items.Add(new Game_RPG(id, name, price, baseValue));
                    break;
                case "Strategy":
                    _items.Add(new Game_Strategy(id, name, price, baseValue, GameController.Instance._progress.CurrentTurn));
                    break;
                case "Puzzle":
                    _items.Add(new Game_Puzzle(id, name, price, baseValue));
                    break;
                case "Simulation":
                    _items.Add(new Game_Simulation(id, name, price, baseValue));
                    break;
                case "Horror":
                    _items.Add(new Game_Horror(id, name, price, baseValue));
                    break;
                default:
                    Debug.LogWarning($"Unknown genre: {genre}");
                    break;
            }
        }
    }

    public bool Remove(GameItems item) => _items.Remove(item);
}