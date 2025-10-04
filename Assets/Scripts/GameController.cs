using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [Header("State")]
    [SerializeField] int startingMoney = 1000;
    [SerializeField] int numOfGameItems = 3;
    [SerializeField] float soldMoneyOverValue = .8f;
    [SerializeField] int bonusMoney = 200;
    public int fluctuationPerTurn = 40;

    [Header("Progress Settings")]
    [SerializeField] int maxTurns = 10;
    [SerializeField] int startingTurn = 1;
    [SerializeField] int startingTargetValue = 50;
    [SerializeField] int targetIncrement = 50;

    int money;

    [Header("UI Refs")]
    [SerializeField] TMP_Text moneyText;
    [SerializeField] TMP_Text totalValueText;
    [SerializeField] TMP_Text turnText;
    [SerializeField] Transform marketContent;
    [SerializeField] Transform libraryContent;
    [SerializeField] GameObject victoryPanel;

    [Header("Game Over UI")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text gameOverMessageText;

    [Header("Events UI (Separated)")]
    [SerializeField] TMP_Text randomEventText;
    [SerializeField] TMP_Text failureEventText;
    [SerializeField] float randomEventMessageDuration = 3f;

    [Header("Random Event Logic")]
    [SerializeField] float eventProbabilityPerTurn = 0.4f;

    [Header("Main Menu Return")]
    [SerializeField] bool autoReturnToMenu = false;
    [SerializeField] float autoReturnDelay = 2f;

    [Header("Prefabs")]
    [SerializeField] MarketCardView marketGamePrefab;
    [SerializeField] LibraryRowView libraryGamePrefab;

    public static GameController Instance { get; private set; }
    public GameProgressService _progress;
    MarketService _market;
    LibraryService _library;
    EventService _events;

    Coroutine _randomEventHideRoutine;

    void Awake()
    {
        Instance = this;
        money = startingMoney;
        _market = new MarketService();
        _library = new LibraryService();
        _events = new EventService();
        _progress = new GameProgressService();

        _progress.Initialize(startingTurn, maxTurns, startingTargetValue, targetIncrement);
        _progress.OnTurnAdvanced += OnTurnAdvanced;
        _progress.OnVictory += OnVictoryAchieved;
        _progress.OnFailStrike += OnFailStrike;
        _progress.OnGameOver += OnGameOver;
    }

    void Start()
    {
        victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

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
        ClearRandomEvent();
        ClearFailureMessage();
    }

    void OnDestroy()
    {
        if (_progress != null)
        {
            _progress.OnTurnAdvanced -= OnTurnAdvanced;
            _progress.OnVictory -= OnVictoryAchieved;
            _progress.OnFailStrike -= OnFailStrike;
            _progress.OnGameOver -= OnGameOver;
        }
    }

    void OnTurnAdvanced(int newTurn, int newTarget) => UpdateTopBar();
    void OnFailStrike(int strike, string msg) => ShowFailureMessage(msg);

    void OnVictoryAchieved() => ShowVictory();

    void OnGameOver()
    {
        ShowFailureMessage("The library police gets REALLY ANGRY!");
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            if (gameOverMessageText != null)
                gameOverMessageText.text = "You are arrested by the game library police...";
        }
        Time.timeScale = 0;
        if (autoReturnToMenu) StartCoroutine(ReturnToMenuAfterDelay());
    }

    // UI Refresh
    void RefreshMarketUI()
    {
        marketContent.DestroyAllChildren();
        foreach (var item in _market._items)
        {
            var marketGame = Instantiate(marketGamePrefab, marketContent);
            marketGame.Bind(item, OnBuyClicked, money >= item.price);
        }
    }

    void RefreshLibraryUI()
    {
        libraryContent.DestroyAllChildren();
        foreach (var item in _library._items)
        {
            int n;
            if(item.genre =="RPG")
            {
                n = item.GetRPGCount(_library._items);
            }
            else             
            {
                n = 0;
            }
            int currentValue = item.GetCurrentValue(_library._items, n);
            var libraryGame = Instantiate(libraryGamePrefab, libraryContent);
            libraryGame.Bind(item, OnSellClicked, currentValue, n);
        }
        UpdateTopBar();
    }

    void UpdateTopBar()
    {
        moneyText.text = $"Money: ${money}";
        totalValueText.text = $"Total Value: ${_library.TotalValue()}";
        turnText.text = $"Turn: {_progress.CurrentTurn}/{_progress.MaxTurns} | Goal: {_progress.CurrentTargetValue}";
    }

    // ---- Random Event UI ----
    void ShowRandomEventMessage(string msg)
    {
        if (randomEventText == null) return;
        if (_randomEventHideRoutine != null) StopCoroutine(_randomEventHideRoutine);
        randomEventText.text = msg;
        if (!string.IsNullOrEmpty(msg))
            _randomEventHideRoutine = StartCoroutine(HideRandomEventAfterDelay());
    }

    IEnumerator HideRandomEventAfterDelay()
    {
        yield return new WaitForSeconds(randomEventMessageDuration);
        ClearRandomEvent();
    }

    void ClearRandomEvent()
    {
        if (randomEventText != null)
            randomEventText.text = "";
    }

    // ---- Failure UI ----
    void ShowFailureMessage(string msg)
    {
        if (failureEventText == null) return;
        failureEventText.text = msg;
    }

    void ClearFailureMessage()
    {
        if (failureEventText != null)
            failureEventText.text = "";
    }

    void TriggerRandomEvent()
    {
        if (_progress.HasVictory || _progress.HasGameOver) return;
        var e = _events.TryTrigger(eventProbabilityPerTurn);
        if (!_progress.HasGameOver)
            ShowRandomEventMessage(e != null ? e.name : "");
    }

    // ---- Actions ----
    public void OnBuyClicked(GameItems item)
    {
        if (item == null || money < item.price) return;
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
        if (_progress.HasVictory || _progress.HasGameOver) return;

        _market.Generate(numOfGameItems);
        RefreshMarketUI();

        // Puzzle ½ÒÏþ
        foreach (var item in _library._items)
        {
            if (item.genre == "Puzzle")
            {
                var puzzle = item as Game_Puzzle;
                if (puzzle != null && !puzzle.isRevealed)
                    puzzle.RevealValue();
            }
        }

        // --- Horror ÓÎÏ·¼ì²é ---
        bool horrorTriggered = false;
        foreach (var item in _library._items)
        {
            if (item.genre == "Horror")
            {
                var horror = item as Game_Horror;
                if (horror != null && horror.TryScareLibraryPolice())
                {
                    horrorTriggered = true;
                    break;
                }
            }
        }

        int totalValueBeforeFluctuation = _library.TotalValue();
        var result = _progress.Evaluate(horrorTriggered ? _progress.CurrentTargetValue : totalValueBeforeFluctuation);

        if (_progress.HasGameOver || result == TurnAdvanceResult.Victory)
            return;

        _library.RandomFluctuation(-fluctuationPerTurn, fluctuationPerTurn);
        RefreshLibraryUI();

        TriggerRandomEvent();
    }

    void ShowVictory()
    {
        victoryPanel.SetActive(true);
        Time.timeScale = 0;

        // Mark first victory for main menu special logo
        if (PlayerPrefs.GetInt(PersistentKeys.HasUltimateLogo, 0) == 0)
        {
            PlayerPrefs.SetInt(PersistentKeys.HasUltimateLogo, 1);
            PlayerPrefs.Save();
        }

        if (autoReturnToMenu) StartCoroutine(ReturnToMenuAfterDelay());
    }

    IEnumerator ReturnToMenuAfterDelay()
    {
        yield return new WaitForSecondsRealtime(autoReturnDelay);
        OnReturnToMenu();
    }

    // UI Buttons
    public void OnRestart()
    {
        Time.timeScale = 1f;
        victoryPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        money = startingMoney;
        _library._items.Clear();
        _market._items.Clear();

        _progress.Reset(startingTurn, startingTargetValue);

        ClearRandomEvent();
        ClearFailureMessage();

        _market.Generate(numOfGameItems);
        RefreshMarketUI();
        RefreshLibraryUI();
        UpdateTopBar();
    }

    public void OnReturnToMenu()
    {
        SceneLoader.LoadMainMenu();
    }
}