using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    readonly List<GameEvent> _pool = new List<GameEvent>();

    public void Init(MarketService market, LibraryService library, System.Action addMoneyCallback, System.Action refreshMarketUI, System.Action refreshLibraryUI, System.Action updateTopBar)
    {
        _pool.Clear();

        _pool.Add(new GameEvent("Steam Autumn Sale! All listed game -50%", () =>
        {
            foreach (var item in market._items)
                item.price = Mathf.Max(1, item.price / 2);
            refreshMarketUI();
        }));
        // priceFluctuation, ²»Ì«ºÃÓÃdebug
        //_pool.Add(new GameEvent("A game is outdated, its value drops to 0!", () =>
        //{
        //    library.RandomZeroOneItem();
        //    refreshLibraryUI();
        //}));

        // bonusMoney
        _pool.Add(new GameEvent("You earn bonus from somewhere! +200$", () =>
        {
            addMoneyCallback();
            updateTopBar();
        }));
    }

    public GameEvent TryTrigger(float probability)
    {
        if (_pool.Count == 0) return null;
        if (Random.value > probability) return null;
        int idx = Random.Range(0, _pool.Count);
        var e = _pool[idx];
        e.effect.Invoke();
        return e;
    }
}