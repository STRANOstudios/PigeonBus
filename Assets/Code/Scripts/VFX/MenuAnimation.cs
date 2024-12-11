using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using UnityEngine;
using UnityEngine.Events;

public class MenuAnimation : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField, Tooltip("The transform to animate"), Required] private RectTransform _MainTransform;
    [SerializeField, Tooltip("The canvas group to animate"), Required] private CanvasGroup _HUDCanvasGroup;
    [SerializeField, Tooltip("The end position of the animation")] private Vector2 _endPosition;

    [FoldoutGroup("Animation Out"), SerializeField, MinValue(0), Tooltip("The duration of the animation"), LabelText("Duration")] private float _durationOut = 2f;
    [FoldoutGroup("Animation Out"), SerializeField, Tooltip("The ease type of the animation"), LabelText("Ease Type")] private Ease _easeTypeOut = Ease.OutBounce;
    [FoldoutGroup("Animation Out"), SerializeField, Tooltip("Event triggered when the animation completes."), LabelText("OnComplete")] private OnCompleteEvent _onCompleteOut = new();

    [FoldoutGroup("Animation In"), SerializeField, MinValue(0), Tooltip("The duration of the animation"), LabelText("Duration")] private float _durationIn = 2f;
    [FoldoutGroup("Animation In"), SerializeField, Tooltip("The ease type of the animation"), LabelText("Ease Type")] private Ease _easeTypeIn = Ease.OutBounce;
    [FoldoutGroup("Animation In"), SerializeField, Tooltip("Event triggered when the animation completes."), LabelText("OnComplete")] private OnCompleteEvent _onCompleteIn = new();

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

    [Serializable] public class OnCompleteEvent : UnityEvent { }

    /// <summary>
    /// Starts the animation of the menu element moving out
    /// </summary>
    public void MoveOut()
    {
        if (_debug) Debug.Log("Moving out");

        _MainTransform.DOAnchorPos(_endPosition, _durationOut).SetEase(_easeTypeOut).OnComplete(() => _onCompleteOut?.Invoke()).SetUpdate(true);
        _HUDCanvasGroup.DOFade(1f, _durationOut).SetUpdate(true);
    }

    /// <summary>
    /// Starts the animation of the menu element moving in
    /// </summary>
    public void MoveIn()
    {
        if (_debug) Debug.Log("Moving in");

        _MainTransform.DOAnchorPos(Vector2.zero, _durationIn).SetEase(_easeTypeIn).OnComplete(() => _onCompleteIn?.Invoke()).SetUpdate(true);
        _HUDCanvasGroup.DOFade(0f, _durationIn).SetUpdate(true);
    }
}
