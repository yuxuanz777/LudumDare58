using System;

public enum TurnAdvanceResult
{
    None,          // 目标未达成（可能触发失败计数）
    Advanced,      // 达成并进入下一回合
    Victory        // 胜利
}

public class GameProgressService
{
    public int CurrentTurn { get; private set; }
    public int MaxTurns { get; private set; }
    public int CurrentTargetValue { get; private set; }
    public int TargetIncrement { get; private set; }
    public bool HasVictory { get; private set; }
    public bool HasGameOver { get; private set; }
    public int FailStrikes => _failStrikes;

    public event Action<int, int> OnTurnAdvanced;      // (newTurn, newTarget)
    public event Action OnVictory;
    public event Action<int, string> OnFailStrike;     // (strikeCount, message)
    public event Action OnGameOver;

    bool _initialized;
    int _failStrikes;

    public void Initialize(int startingTurn, int maxTurns, int startingTarget, int targetIncrement)
    {
        CurrentTurn = startingTurn;
        MaxTurns = maxTurns;
        CurrentTargetValue = startingTarget;
        TargetIncrement = targetIncrement;
        HasVictory = false;
        HasGameOver = false;
        _failStrikes = 0;
        _initialized = true;
    }

    public void Reset(int startingTurn, int startingTarget)
    {
        CurrentTurn = startingTurn;
        CurrentTargetValue = startingTarget;
        HasVictory = false;
        HasGameOver = false;
        _failStrikes = 0;
    }

    public TurnAdvanceResult Evaluate(int libraryTotalValue)
    {
        if (!_initialized)
            throw new InvalidOperationException("GameProgressService not initialized.");

        if (HasVictory) return TurnAdvanceResult.Victory;
        if (HasGameOver) return TurnAdvanceResult.None;

        // 未达到目标 -> 失败一次
        if (libraryTotalValue < CurrentTargetValue)
        {
            _failStrikes++;
            string msg = GetFailMessage(_failStrikes);
            OnFailStrike?.Invoke(_failStrikes, msg);

            if (_failStrikes >= 3)
            {
                HasGameOver = true;
                OnGameOver?.Invoke();
            }
            return TurnAdvanceResult.None;
        }

        // 达成目标 -> 若已在最后一回合则直接胜利
        if (CurrentTurn >= MaxTurns)
        {
            HasVictory = true;
            OnVictory?.Invoke();
            return TurnAdvanceResult.Victory;
        }

        // 普通前进
        CurrentTurn++;
        CurrentTargetValue += TargetIncrement;
        OnTurnAdvanced?.Invoke(CurrentTurn, CurrentTargetValue);
        return TurnAdvanceResult.Advanced;
    }

    string GetFailMessage(int strike)
    {
        switch (strike)
        {
            case 1: return "The library police is watching you now!";
            case 2: return "The library police starts to criticize your game!";
            case 3: return "The library police gets REALLY ANGRY!";
            default: return "The library police is furious!";
        }
    }
}