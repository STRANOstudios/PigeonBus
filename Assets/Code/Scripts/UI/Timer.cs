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
        GameManager.OnTimeUpdate += UpdateScore;
    }

    private void OnDisable()
    {
        GameManager.OnTimeUpdate -= UpdateScore;
    }

    private void Start()
    {
        _timerText.text = "00 : 00";
    }

    private void UpdateScore(float time)
    {
        if (_debug) Debug.Log("Timer: " + time);
        _timerText.text = $"{((int)time / 60):00} : {((int)time % 60):00}";
    }
}
