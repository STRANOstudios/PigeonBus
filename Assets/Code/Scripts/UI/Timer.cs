using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required] private TMP_Text _timerText;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

    private void OnEnable()
    {
        GameManager.OnTimeUpdate += UpdateTimer;
    }

    private void OnDisable()
    {
        GameManager.OnTimeUpdate -= UpdateTimer;
    }

    private void Start()
    {
        _timerText.text = "00 : 00";
    }

    /// <summary>
    /// Updates the timer
    /// </summary>
    /// <param name="time"></param>
    private void UpdateTimer(float time)
    {
        if (_debug) Debug.Log("Timer: " + time);

        // Normalize the time aspect
        _timerText.text = $"{(int)time / 60:00} : {(int)time % 60:00}";
    }
}
