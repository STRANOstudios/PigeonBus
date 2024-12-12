using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

public class WinLoseController : MonoBehaviour
{
    [Title("References")]
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private GameObject _winScreen;
    [SerializeField] private GameObject _loseScreen;

    [Title("Settings")]
    [SerializeField] private float _duration = 1f;
    [SerializeField, Tooltip("Called when the player wins")] private ConditionEvent _onWin = new();
    [SerializeField, Tooltip("Called when the player loses")] private ConditionEvent _onLose = new();

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

    [Serializable] public class ConditionEvent : UnityEvent { }

    private void Start() { }

    private void OnEnable()
    {
        GameManager.Win += Win;
        GameManager.Lose += Lose;
        GameManager.Reset += Reset;
    }
    private void OnDisable()
    {
        GameManager.Win -= Win;
        GameManager.Lose -= Lose;
        GameManager.Reset -= Reset;
    }

    /// <summary>
    /// Called when the player wins
    /// </summary>
    private void Win()
    {
        if (!enabled) return;
        if (_debug) Debug.Log("Win");

        _winScreen.SetActive(true);

        if (_panel != null) FadeIn();

        _onWin?.Invoke();
    }

    /// <summary>
    /// Called when the player loses
    /// </summary>
    private void Lose()
    {
        if (!enabled) return;
        if (_debug) Debug.Log("Lose");

        _loseScreen.SetActive(true);

        if (_panel != null) FadeIn();

        _onLose?.Invoke();
    }

    /// <summary>
    /// Called when the player resets
    /// </summary>
    private void Reset()
    {
        if (!enabled) return;
        if (_debug) Debug.Log("Reset");

        _winScreen.SetActive(false);
        _loseScreen.SetActive(false);

        if (_loseScreen != null) FadeOut();
    }

    /// <summary>
    /// Fades the canvas in
    /// </summary>
    public void FadeIn()
    {
        if (_debug) Debug.Log("FadeIn");

        _panel.DOFade(1f, _duration).SetUpdate(true).OnComplete(() =>
        {
            if (_debug) Debug.Log("Canvas Enabled");
            _panel.interactable = true;
            _panel.blocksRaycasts = true;
        });
    }

    /// <summary>
    /// Fades the canvas out
    /// </summary>
    public void FadeOut()
    {
        if (_debug) Debug.Log("FadeOut");

        _panel.DOFade(0f, _duration).SetUpdate(true).OnComplete(() =>
        {
            if (_debug) Debug.Log("Canvas Disabled");
            _panel.interactable = false;
            _panel.blocksRaycasts = false;
        });
    }
}
