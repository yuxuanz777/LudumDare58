using System.Collections;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("State")]
    [SerializeField] int startingMoney = 1000;
    [SerializeField] int numOfGameItems = 3;
    [SerializeField] float soldMoneyOverValue = .8f;
    [SerializeField] int fluctuationPerTurn = 40;
    [SerializeField] int bonusMoney = 200;
    int money;

    [Header("UI Refs")]
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text totalValueText;
    [SerializeField] Transform marketContent;
    [SerializeField] Transform libraryContent;

    [Header("Prefabs")]
    [SerializeField] MarketCardView marketGamePrefab;
    [SerializeField] LibraryRowView libraryGamePrefab;

    [Header("Events UI")]
    [SerializeField] TMP_Text eventText;
    [SerializeField] float eventMessageDuration = 3f;
    [SerializeField] float eventProbabilityPerTurn = 0.4f;

    // Services
    MarketService _market;
    LibraryService _library;
    EventService _events;

    void Awake()
    {
        money = startingMoney;
        _market = new MarketService();
        _library = new LibraryService();
        _events = new EventService();
    }

    void Start()
    {
        _events.Init(
            _market,
            _library,
            addMoneyCallback: () => money += bonusMoney,
            refreshMarketUI: RefreshMarketUI,
            refreshLibraryUI: RefreshLibraryUI,
            updateTopBar: UpdateTopBar
        );

        _market.Generate(numOfGameItems);
        RefreshMarketUI();
        RefreshLibraryUI();
        UpdateTopBar();
        ShowEventMessage(""); // clear
    }

    // ---- UI Refresh ----
    void RefreshMarketUI()
    {
        marketContent.DestroyAllChildren();
        foreach (var item in _market.Items)
        {
            var marketGame = Instantiate(marketGamePrefab, marketContent);
            marketGame.Bind(item, OnBuyClicked, money >= item.price);
        }
    }

    void RefreshLibraryUI()
    {
        libraryContent.DestroyAllChildren();
        foreach (var item in _library.Items)
        {
            var libraryGame = Instantiate(libraryGamePrefab, libraryContent);
            libraryGame.Bind(item, OnSellClicked);
        }
        UpdateTopBar();
    }

    void UpdateTopBar()
    {
        moneyText.text = $"Money: ${money}";
        totalValueText.text = $"Total Value: ${_library.TotalValue()}";
        // Update interactability on market cards after money change
        for (int i = 0; i < marketContent.childCount; i++)
        {
            var card = marketContent.GetChild(i).GetComponent<MarketCardView>();          
        }
    }

    // ---- Events ----
    void ShowEventMessage(string msg)
    {
        StopAllCoroutines();
        eventText.text = msg;
        if (!string.IsNullOrEmpty(msg))
            StartCoroutine(HideMessageAfterDelay());
    }

    IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(eventMessageDuration);
        eventText.text = "";
    }

    void TriggerRandomEvent()
    {
        var e = _events.TryTrigger(eventProbabilityPerTurn);
        ShowEventMessage(e != null ? e.name : "");
    }

    // ---- Actions ----
    public void OnBuyClicked(GameItems item)
    {
        if (item == null) return;
        if (money < item.price) return;
        money -= item.price;
        _market.Remove(item);
        _library.Add(item);
        RefreshMarketUI();
        RefreshLibraryUI();
    }

    public void OnSellClicked(GameItems item)
    {
        if (item == null) return;
        money += Mathf.RoundToInt(item.value * soldMoneyOverValue);
        _library.Remove(item);
        RefreshLibraryUI();
        RefreshMarketUI();
    }

    public void OnEndTurn()
    {
        _market.Generate(numOfGameItems);
        RefreshMarketUI();

        _library.RandomFluctuation(-fluctuationPerTurn, fluctuationPerTurn);
        RefreshLibraryUI();

        TriggerRandomEvent();
    }
}