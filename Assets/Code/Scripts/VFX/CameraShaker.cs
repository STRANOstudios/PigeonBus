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
        // Get the perlin components from the cameras
        // This assumes that each camera has a CinemachineBasicMultiChannelPerlin component attached
        perlinComponents = cameras
            .Select(cam => cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>())
            .ToList();

        // Apply the default shake settings to the perlin components
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

    /// <summary>
    /// Coroutine that shakes the camera based on the provided ShakePreset.
    /// </summary>
    /// <param name="preset">The ShakePreset that defines the shake settings.</param>
    /// <returns>An IEnumerator that yields null until the shake routine is complete.</returns>
    private IEnumerator ShakeRoutine(ShakePreset preset)
    {
        // Apply the initial shake settings
        ApplyShakeSettings(preset.noiseSettings, 0);

        float elapsedTime = 0f;

        // Loop until the shake duration has been reached
        while (elapsedTime < preset.duration)
        {
            elapsedTime += Time.deltaTime;

            // Calculate the normalized time (0 to 1) based on the elapsed time and duration
            float normalizedTime = Mathf.Clamp01(elapsedTime / preset.duration);

            // Calculate the intensity of the shake based on the normalized time and the preset's curve
            float intensity = preset.curve.Evaluate(normalizedTime) + defaultAmplitude;

            // Apply the intensity to each perlin component
            foreach (var perlin in perlinComponents)
            {
                perlin.m_AmplitudeGain = intensity;
            }

            yield return null;
        }

        ApplyShakeSettings(defaultNoise, defaultAmplitude);
    }

    /// <summary>
    /// Represents a shake preset, which defines the settings for a shake effect.
    /// </summary>
    [Serializable]
    public class ShakePreset
    {
        /// <summary>
        /// The noise settings for the shake effect.
        /// </summary>
        [HorizontalGroup("Settings", LabelWidth = 100)]
        [SerializeField, Required, NoiseSettingsProperty]
        public NoiseSettings noiseSettings;

        /// <summary>
        /// The duration of the shake effect.
        /// </summary>
        /// <remarks>
        /// This value must be greater than or equal to 0.
        /// </remarks>
        [HorizontalGroup("Settings")]
        [SerializeField, MinValue(0f), Tooltip("Duration of the shake.")]
        public float duration = 0.5f;

        /// <summary>
        /// The curve that defines the shake effect over time.
        /// </summary>
        /// <remarks>
        /// This curve is used to interpolate the shake effect's intensity over the duration.
        /// </remarks>
        [BoxGroup("Curve", false)]
        [SerializeField, Tooltip("Curve for the shake effect over time.")]
        public AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    }
}
