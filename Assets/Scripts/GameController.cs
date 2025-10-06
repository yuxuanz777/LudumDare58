using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static EventService;

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

    [Header("Failure Message (Animated)")]
    [SerializeField] FailureMessageView failureMessageView; // 新增动画组件
    // 如果你仍想保留原来的 Text，可留着：
    [SerializeField, Tooltip("旧的直接文本引用(可空)")] TMP_Text failureEventTextLegacy;

    [Header("Styled Event Popup")]
    [SerializeField] CanvasGroup eventGroup;
    [SerializeField] Image leftIcon;
    [SerializeField] Image rightIcon;
    [SerializeField] TMP_Text eventText;
    [SerializeField] float eventAnimDuration = 0.5f;
    [SerializeField] float eventStayDuration = 2.5f;

    [System.Serializable]
    public class EventStyleEntry
    {
        public string key;
        public EventService.EventVisualStyle style;
    }

    [Header("Event Style Assets")]
    [SerializeField] List<EventStyleEntry> eventStyles = new();

    [Header("Random Event Logic")]
    [SerializeField] float eventProbabilityPerTurn = 0.8f;

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

    Coroutine _styledEventRoutine;

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

        foreach (var entry in eventStyles)
        {
            if (entry != null && !string.IsNullOrEmpty(entry.key) && entry.style != null)
                _events.RegisterStyle(entry.key, entry.style);
        }

        // Play gameplay BGM at start
        if (AudioManager.Instance != null && AudioManager.Instance.gameBGM != null)
            AudioManager.Instance.PlayBGM(AudioManager.Instance.gameBGM);

        _market.Generate(numOfGameItems);
        RefreshMarketUI();
        RefreshLibraryUI();
        UpdateTopBar();
        ClearFailureMessage();
        ClearStyledEvent();
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
                gameOverMessageText.text = "Ooops!\r\nYou are arrested by the library police...\r\nPlay more games!";
        }

        // PLAY DEFEAT BGM (added null safety)
        var am = AudioManager.Instance;
        if (am != null && am.defeatBGM != null)
            am.PlayBGM(am.defeatBGM);

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
            if (item.genre == "RPG")
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
        moneyText.text = $"${money}";
        totalValueText.text = $"Total Value: ${_library.TotalValue()} / Goal: ${_progress.CurrentTargetValue}";
        turnText.text = $"Turn: {_progress.CurrentTurn}/{_progress.MaxTurns}";
    }

    

    // ---- Styled Event Popup ----
    void ShowStyledEvent(GameEvent e)
    {
        if (e == null || eventGroup == null || eventText == null)
        {
            ClearStyledEvent();
            return;
        }
        if (_styledEventRoutine != null) StopCoroutine(_styledEventRoutine);

        var autoLayout = eventGroup.GetComponent<EventDisplayAutoLayout>();
        if (autoLayout != null)
            autoLayout.ApplyStyle(e.key); // 根据事件类型调整样式

        _styledEventRoutine = StartCoroutine(AnimateEventPopup(e));
    }

    IEnumerator AnimateEventPopup(GameEvent e)
    {
        var style = _events.GetStyle(e.key);

        // Apply style
        eventText.text = e.name;
        if (style != null)
        {
            eventText.color = style.textColor;
            if (style.font != null) eventText.font = style.font;
            if (leftIcon != null)
            {
                leftIcon.sprite = style.leftIcon;
                leftIcon.enabled = style.leftIcon != null;
            }
            if (rightIcon != null)
            {
                rightIcon.sprite = style.rightIcon;
                rightIcon.enabled = style.rightIcon != null;
            }
        }
        else
        {
            if (leftIcon != null) leftIcon.enabled = false;
            if (rightIcon != null) rightIcon.enabled = false;
        }

        eventGroup.gameObject.SetActive(true);
        eventGroup.alpha = 0f;
        float t = 0f;
        Vector3 baseScale = Vector3.one * 0.2f;
        Vector3 peakScale = style != null ? style.enterScale : Vector3.one * 1.3f;
        Transform holder = eventGroup.transform;
        holder.localScale = baseScale;

        while (t < eventAnimDuration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / eventAnimDuration);
            float ease = 1f - Mathf.Pow(1f - n, 3); // cubic ease-out
            eventGroup.alpha = ease;
            // Overshoot bounce feeling
            float overshoot = Mathf.Sin(ease * Mathf.PI) * 0.08f;
            holder.localScale = Vector3.LerpUnclamped(baseScale, peakScale, ease) * (1f + overshoot);
            yield return null;
        }

        // Settle to 1
        t = 0f;
        Vector3 finalScale = Vector3.one;
        Vector3 startScale = holder.localScale;
        float settleDuration = eventAnimDuration * 0.4f;
        while (t < settleDuration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / settleDuration);
            float ease = Mathf.SmoothStep(0, 1, n);
            holder.localScale = Vector3.Lerp(startScale, finalScale, ease);
            yield return null;
        }
        holder.localScale = finalScale;

        // Stay
        yield return new WaitForSeconds(eventStayDuration);

        // Fade out
        t = 0f;
        float fadeDuration = eventAnimDuration * 0.6f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float n = Mathf.Clamp01(t / fadeDuration);
            eventGroup.alpha = 1f - n;
            holder.localScale = Vector3.Lerp(finalScale, finalScale * 0.9f, n);
            yield return null;
        }

        ClearStyledEvent();
    }

    void ClearStyledEvent()
    {
        if (eventGroup != null)
        {
            eventGroup.alpha = 0f;
            eventGroup.gameObject.SetActive(false);
        }
        if (eventText != null) eventText.text = "";
        if (leftIcon != null) leftIcon.enabled = false;
        if (rightIcon != null) rightIcon.enabled = false;
    }

    // Failure (delegated)
    void ShowFailureMessage(string msg)
    {
        if (failureMessageView != null)
            failureMessageView.Show(msg);
        else if (failureEventTextLegacy != null) // 兼容旧字段
            failureEventTextLegacy.text = msg;
    }

    void ClearFailureMessage()
    {
        if (failureMessageView != null)
            failureMessageView.Clear();
        if (failureEventTextLegacy != null)
            failureEventTextLegacy.text = "";
    }

    void TriggerRandomEvent()
    {
        if (_progress.HasVictory || _progress.HasGameOver) return;
        var e = _events.TryTrigger(eventProbabilityPerTurn);
        if (e != null)
        {
            // Show fancy popup
            ShowStyledEvent(e);
            AudioManager.Instance.PlaySound(AudioManager.Instance.eventSound);
        }
        else
        {
            // No event this turn
        }
    }

    // ---- Actions ----
    public void OnBuyClicked(GameItems item)
    {
        if (item == null || money < item.price) return;
        if (AudioManager.Instance != null && AudioManager.Instance.buySound != null)
            AudioManager.Instance.PlaySound(AudioManager.Instance.buySound);
        money -= item.price;
        _market.Remove(item);
        _library.Add(item);
        RefreshMarketUI();
        RefreshLibraryUI();
    }

    public void OnSellClicked(GameItems item)
    {
        if (item == null) return;
        if (AudioManager.Instance != null && AudioManager.Instance.sellSound != null)
            AudioManager.Instance.PlaySound(AudioManager.Instance.sellSound);
        money += Mathf.RoundToInt(item.value * soldMoneyOverValue);
        _library.Remove(item);
        RefreshLibraryUI();
        RefreshMarketUI();
    }

    public void OnEndTurn()
    {
        if (_progress.HasVictory || _progress.HasGameOver) return;
        if (AudioManager.Instance != null && AudioManager.Instance.clickSound != null)
            AudioManager.Instance.PlaySound(AudioManager.Instance.clickSound);

        _market.Generate(numOfGameItems);
        RefreshMarketUI();

        foreach (var item in _library._items)
        {
            if (item.genre == "Puzzle")
            {
                var puzzle = item as Game_Puzzle;
                if (puzzle != null && !puzzle.isRevealed)
                    puzzle.RevealValue();
            }
        }

        // --- Horror 游戏检查 ---
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
    IEnumerator GlowText(TMP_Text text)
    {
        if (text == null) yield break;

        // 避免被顶层材质/渐变覆盖
        text.enableVertexGradient = false;

        float t = 0f;
        while (true)
        {
            t += Time.unscaledDeltaTime; // 不受 Time.timeScale 影响
            float k = (Mathf.Sin(t * 2f) + 1f) * 0.5f; // 0~1
            Color from = new Color32(150, 220, 255, 255);
            Color to = Color.white;

            // 对 TMP 用 faceColor 更稳妥
            text.faceColor = Color.Lerp(from, to, k);

            yield return null;
        }
    }

    void ShowVictory()
    {
        victoryPanel.SetActive(true);

        // PLAY VICTORY BGM (added)
        var am = AudioManager.Instance;
        if (am != null && am.victoryBGM != null)
            am.PlayBGM(am.victoryBGM);

        var victoryText = victoryPanel.GetComponentInChildren<TMP_Text>();
        //StartCoroutine(GlowText(victoryText));
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

        ClearFailureMessage();
        ClearStyledEvent();

        // Restore gameplay BGM after restart
        if (AudioManager.Instance != null && AudioManager.Instance.gameBGM != null)
            AudioManager.Instance.PlayBGM(AudioManager.Instance.gameBGM);

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