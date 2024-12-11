using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class CameraShaker : MonoBehaviour
{
    [Title("Cameras")]
    [SerializeField, Tooltip("Virtual Cameras to apply the shake effect.")]
    private List<CinemachineVirtualCamera> cameras = new();

    [BoxGroup("Default Shake")]
    [SerializeField, Required, Tooltip("Default noise profile for the shake."), NoiseSettingsProperty]
    private NoiseSettings defaultNoise;
    [BoxGroup("Default Shake")]
    [SerializeField, MinValue(0f), Tooltip("Default amplitude for the shake.")]
    private float defaultAmplitude = 1f;

    [SerializeField, Tooltip("List of shake presets.")] List<ShakePreset> shakeQueue = new();

    [Title("Debug")]
    [SerializeField] private bool _debug;

    [Button("Start Shake")]
    public void StartShake() => StartShake(shakeQueue[UnityEngine.Random.Range(0, shakeQueue.Count)]);

    private List<CinemachineBasicMultiChannelPerlin> perlinComponents = new();

    private void Start()
    {
        // Get perlin components
        perlinComponents = cameras
            .Select(cam => cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>())
            .ToList();

        ApplyShakeSettings(defaultNoise, defaultAmplitude);
    }

    private void OnEnable()
    {
        BusStopValidator.OnBusStopReached += Feedback;
    }

    private void OnDisable()
    {
        BusStopValidator.OnBusStopReached -= Feedback;
    }

    private void ApplyShakeSettings(NoiseSettings noise, float amplitude)
    {
        foreach (var perlin in perlinComponents)
        {
            perlin.m_NoiseProfile = noise;
            perlin.m_AmplitudeGain = amplitude;
        }
    }

    private void StartShake(ShakePreset preset)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(preset));
    }
    /// <summary>
    /// Starts a new shake
    /// </summary>
    /// <param name="index"></param>
    public void FeedbackByIndex(int index = 0)
    {
        StartShake(shakeQueue[index]);
    }

    /// <summary>
    /// Starts a new shake
    /// </summary>
    /// <param name="index"></param>
    public void Feedback(int value)
    {
        if (value < 0)
        {
            StartShake(shakeQueue[0]);
        }
    }

    private IEnumerator ShakeRoutine(ShakePreset preset)
    {
        ApplyShakeSettings(preset.noiseSettings, 0);

        float elapsedTime = 0f;
        while (elapsedTime < preset.duration)
        {
            elapsedTime += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsedTime / preset.duration);
            float intensity = preset.curve.Evaluate(normalizedTime) + defaultAmplitude;

            foreach (var perlin in perlinComponents)
            {
                perlin.m_AmplitudeGain = intensity;
            }

            yield return null;
        }

        ApplyShakeSettings(defaultNoise, defaultAmplitude);
    }

    [Serializable]
    public class ShakePreset
    {
        [HorizontalGroup("Settings", LabelWidth = 100)]
        [SerializeField, Required, NoiseSettingsProperty]
        public NoiseSettings noiseSettings;

        [HorizontalGroup("Settings")]
        [SerializeField, MinValue(0f), Tooltip("Duration of the shake.")]
        public float duration = 0.5f;

        [BoxGroup("Curve", false)]
        [SerializeField, Tooltip("Curve for the shake effect over time.")]
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }
}
