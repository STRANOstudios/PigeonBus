using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [Title("References")]
    [SerializeField, Required] private TMP_Text _scoreText;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

    private void OnEnable()
    {
        GameManager.OnScoreUpdate += UpdateScore;
    }

    private void OnDisable()
    {
        GameManager.OnScoreUpdate -= UpdateScore;
    }

    private void Start()
    {
        _scoreText.text = "0";
    }

    private void UpdateScore(int score)
    {
        if(_debug) Debug.Log("Score: " + score);
        _scoreText.text = score.ToString();
    }
}
