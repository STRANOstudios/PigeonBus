using Sirenix.OdinInspector;
using System;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Title("Settings")]
    [SerializeField, MinValue(1)] private int _winningScore = 1;
    [SerializeField, MaxValue(0)] private int _losingScore = 0;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;
    [ShowIf("_debug"), ShowInInspector, ReadOnly] private float _timeScale => Time.timeScale;
    [Button]
    public void LoseButton() => Lose?.Invoke();
    [Button]
    public void WinButton() => Win?.Invoke();

    private int score = 0;
    private float time = 0f;

    public static event Action<int> OnScoreUpdate;
    public static event Action<float> OnTimeUpdate;
    public static event Action Win;
    public static event Action Lose;
    public static event Action Reset;

    private void Awake()
    {
        Time.timeScale = 0;
    }

    private void Start()
    {
        score = 0;
    }

    private void OnEnable()
    {
        BusStopValidator.OnBusStopReached += ScoreCounter;
    }

    private void OnDisable()
    {
        BusStopValidator.OnBusStopReached -= ScoreCounter;
    }

    private void ScoreCounter(int value)
    {
        if(_debug) Debug.Log("Score: " + value);

        score += value;

        OnScoreUpdate?.Invoke(score);
        OnTimeUpdate?.Invoke(Time.realtimeSinceStartup - time);

        if (score <= _losingScore)
        {
            if(_debug) Debug.Log("Lose");
            Lose?.Invoke();
        }
        if (score >= _winningScore)
        {
            if(_debug) Debug.Log("Win");
            Win?.Invoke();
        }
    }

    /// <summary>
    /// Reset the score
    /// </summary>
    public void ResetScore()
    {
        time = Time.realtimeSinceStartup;
        score = 0;
        OnScoreUpdate?.Invoke(score);
        Reset?.Invoke();
    }
}
