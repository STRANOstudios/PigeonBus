using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class FadeInOut : MonoBehaviour
{
    [Title("Settings")]
    [SerializeField] private float _duration = 1f;

    public void FadeIn(CanvasGroup canvas)
    {
        canvas.interactable = true;
        canvas.DOFade(1f, _duration);
    }

    public void FadeOut(CanvasGroup canvas)
    {
        canvas.interactable = false;
        canvas.DOFade(0f, _duration);
    }
}
