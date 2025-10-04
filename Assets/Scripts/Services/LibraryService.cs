using System.Collections.Generic;
using UnityEngine;

public class LibraryService
{
    //public IReadOnlyList<GameItems> Items => _items;
    public List<GameItems> _items = new List<GameItems>();

    public void Add(GameItems item)
    {
        item.predictedDelta = Random.Range(-GameController.Instance.fluctuationPerTurn, GameController.Instance.fluctuationPerTurn);
        _items.Add(item);
    }
    public bool Remove(GameItems item) => _items.Remove(item);

    public int TotalValue()
    {
        int sum = 0;
        for (int i = 0; i < _items.Count; i++)
            sum += _items[i].value;
        return sum;
    }

    public void RandomFluctuation(int minDelta, int maxDelta)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            int d = _items[i].predictedDelta;
            _items[i].value = Mathf.Max(0, _items[i].value + d);

            _items[i].predictedDelta = Random.Range(minDelta, maxDelta);
        }
    }

    public void RandomZeroOneItem()
    {
        if (_items.Count == 0) return;
        int idx = Random.Range(0, _items.Count);
        _items[idx].value = 0;
    }
}