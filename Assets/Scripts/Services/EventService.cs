using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static GameController;

public class EventService
{
    readonly List<GameEvent> _pool = new List<GameEvent>();

    [System.Serializable]
    public class EventVisualStyle
    {
        public Color textColor = Color.white;
        public TMP_FontAsset font;
        public Sprite leftIcon;
        public Sprite rightIcon;
        public Vector3 enterScale = Vector3.one * 1.25f;
    }

    // Registered styles by event key
    readonly Dictionary<string, EventVisualStyle> eventStyles = new();

    public void Init(
        MarketService market,
        LibraryService library,
        System.Action addMoneyCallback,
        System.Action refreshMarketUI,
        System.Action refreshLibraryUI,
        System.Action updateTopBar)
    {
        _pool.Clear();

        // Autumn Sale
        _pool.Add(new GameEvent(
            "sale",
            "Steam Autumn Sale! All game -50%",
            () =>
            {
                foreach (var item in market._items)
                    item.price = Mathf.Max(1, item.price / 2);
                refreshMarketUI();
            }));

        // Work (bonus money)
        _pool.Add(new GameEvent(
            "work",
            "Who transferred this money to me? +200$",
            () =>
            {
                addMoneyCallback();
                updateTopBar();
            }));
    }

    public void RegisterStyle(string key, EventVisualStyle style)
    {
        if (string.IsNullOrEmpty(key) || style == null) return;
        eventStyles[key] = style;
    }

    public EventVisualStyle GetStyle(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        eventStyles.TryGetValue(key, out var style);
        return style;
    }

    public GameEvent TryTrigger(float probability)
    {
        if (_pool.Count == 0) return null;
        if (Random.value > probability) return null;
        int idx = Random.Range(0, _pool.Count);
        var e = _pool[idx];
        e.effect?.Invoke();
        return e;
    }
}