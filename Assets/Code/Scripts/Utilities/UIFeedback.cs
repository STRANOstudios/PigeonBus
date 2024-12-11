using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class UIFeedback : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, MinValue(0)] private float _duration = 0.5f;
    [SerializeField] private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Title("References")]
    [SerializeField, Required] private Image _image;

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

    private Tween alphaAnimation;

    private void OnEnable()
    {
        BusStopValidator.OnBusStopReached += Feedback;
        GameManager.Reset += ResetAlpha;
    }

    private void OnDisable()
    {
        BusStopValidator.OnBusStopReached -= Feedback;
        GameManager.Reset -= ResetAlpha;
    }

    private void Feedback(int value)
    {
        if (_debug) Debug.Log("Feedback: " + value);
        if (value < 0)
        {
            FadeInOut();
        }
    }

    private void ResetAlpha()
    {
        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, 0f);
        alphaAnimation.Kill();
    }

    public void FadeInOut()
    {
        if(_debug) Debug.Log("FadeInOut");

        // Reset the alpha to fully visible
        Color startColor = _image.color;

        alphaAnimation = _image.DOFade(1f, _duration).SetEase(_fadeCurve).OnComplete(() =>
        {
            ResetAlpha();
        });
    }
}
