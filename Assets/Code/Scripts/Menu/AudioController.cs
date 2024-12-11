using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Sirenix.OdinInspector;

[DisallowMultipleComponent]
public class AudioController : MonoBehaviour
{
    // References to AudioMixer and UI controls
    [Title("References")]
    [SerializeField, Required] private AudioMixer _mixer;

    // Sliders and buttons for controlling master volume and mute
    [FoldoutGroup("Master"), SerializeField] private Slider _volumeMasterSlider;
    [FoldoutGroup("Master"), SerializeField] private Button _muteMasterButton;

    // Sliders and buttons for controlling music volume and mute
    [FoldoutGroup("Music"), SerializeField] private Slider _volumeMusicSlider;
    [FoldoutGroup("Music"), SerializeField] private Button _muteMusicButton;

    // Sliders and buttons for controlling SFX volume and mute
    [FoldoutGroup("SFX"), SerializeField] private Slider _volumeSFXSlider;
    [FoldoutGroup("SFX"), SerializeField] private Button _muteSFXButton;

    [Title("Debug")]
    [SerializeField] private bool _debug;

    // Class to hold all the settings data
    [Serializable]
    public class AudioSettings
    {
        public float volumeMaster;
        public float volumeMusic;
        public float volumeSFX;
        public bool isMasterMuted;
        public bool isMusicMuted;
        public bool isSFXMuted;
    }

    AudioSettings settings;

    private const string SETTINGS_KEY = "AudioSettings"; // Key for saving the whole audio settings

    private bool _settingsLoaded = false;

    private void Start()
    {
        LoadSettings();
    }

    private void OnEnable()
    {
        // Add listeners to sliders for real-time volume control
        _volumeMasterSlider?.onValueChanged.AddListener(OnMasterVolumeChanged);
        _volumeMusicSlider?.onValueChanged.AddListener(OnMusicVolumeChanged);
        _volumeSFXSlider?.onValueChanged.AddListener(OnSFXVolumeChanged);

        // Add listeners to buttons
        _muteMasterButton?.onClick.AddListener(() => ToggleMute(MixerGroup.Master));
        _muteMusicButton?.onClick.AddListener(() => ToggleMute(MixerGroup.Music));
        _muteSFXButton?.onClick.AddListener(() => ToggleMute(MixerGroup.Sfx));
    }

    private void OnDisable()
    {
        // Remove listeners to prevent potential memory leaks
        _volumeMasterSlider?.onValueChanged.RemoveListener(OnMasterVolumeChanged);
        _volumeMusicSlider?.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        _volumeSFXSlider?.onValueChanged.RemoveListener(OnSFXVolumeChanged);

        // Remove listeners from buttons
        _muteMasterButton?.onClick.RemoveListener(() => ToggleMute(MixerGroup.Master));
        _muteMusicButton?.onClick.RemoveListener(() => ToggleMute(MixerGroup.Music));
        _muteSFXButton?.onClick.RemoveListener(() => ToggleMute(MixerGroup.Sfx));
    }

    /// <summary>
    /// Loads audio settings from the save system.
    /// </summary>
    private void LoadSettings()
    {
        if (_debug) Debug.Log("Loading audio settings...");

        settings = SaveSystem.Exists(SETTINGS_KEY)
            ? SaveSystem.Load<AudioSettings>(SETTINGS_KEY)
            : new AudioSettings { volumeMaster = 0.75f, volumeMusic = 0.75f, volumeSFX = 0.75f, isMasterMuted = false, isMusicMuted = false, isSFXMuted = false };

        // Apply loaded settings to UI controls and mixer
        if (_volumeMasterSlider != null) _volumeMasterSlider.value = settings.volumeMaster;
        if (_volumeMusicSlider != null) _volumeMusicSlider.value = settings.volumeMusic;
        if (_volumeSFXSlider != null) _volumeSFXSlider.value = settings.volumeSFX;

        _settingsLoaded = true;

        ApplyVolume(MixerGroup.Master, settings.volumeMaster);
        ApplyVolume(MixerGroup.Music, settings.volumeMusic);
        ApplyVolume(MixerGroup.Sfx, settings.volumeSFX);

        ApplyMute(MixerGroup.Master, settings.isMasterMuted);
        ApplyMute(MixerGroup.Music, settings.isMusicMuted);
        ApplyMute(MixerGroup.Sfx, settings.isSFXMuted);
    }

    #region Slider Callbacks

    private void OnMasterVolumeChanged(float value)
    {
        ApplyVolume(MixerGroup.Master, value);
    }

    private void OnMusicVolumeChanged(float value)
    {
        ApplyVolume(MixerGroup.Music, value);
    }

    private void OnSFXVolumeChanged(float value)
    {
        ApplyVolume(MixerGroup.Sfx, value);
    }

    #endregion

    #region Setters and Getters Volume

    private void ApplyVolume(MixerGroup group, float value)
    {
        if (!_settingsLoaded) return;

        float volume = Mathf.Lerp(-80f, 20f, value);
        _mixer.SetFloat(group.ToString(), volume);
        SaveSettings();
        if (_debug) Debug.Log($"Applying volume for {group} to {volume}");
    }

    private float GetVolume(MixerGroup group)
    {
        return group switch
        {
            MixerGroup.Master => settings.volumeMaster,
            MixerGroup.Music => settings.volumeMusic,
            MixerGroup.Sfx => settings.volumeSFX,
            _ => 0.75f,
        };
    }

    #endregion

    #region Setters and Getters Mute

    private void ApplyMute(MixerGroup group, bool isMuted)
    {
        _mixer.SetFloat(group.ToString(), isMuted ? -80f : Mathf.Lerp(-80f, 20f, GetVolume(group)));
    }

    private void ToggleMute(MixerGroup group)
    {
        switch (group)
        {
            case MixerGroup.Master:
                settings.isMasterMuted = !settings.isMasterMuted;
                break;
            case MixerGroup.Music:
                settings.isMusicMuted = !settings.isMusicMuted;
                break;
            case MixerGroup.Sfx:
                settings.isSFXMuted = !settings.isSFXMuted;
                break;
        }
        SaveSettings();
        ApplyMute(group, GetMuteState(group));
    }

    private bool GetMuteState(MixerGroup group)
    {
        return group switch
        {
            MixerGroup.Master => settings.isMasterMuted,
            MixerGroup.Music => settings.isMusicMuted,
            MixerGroup.Sfx => settings.isSFXMuted,
            _ => false,
        };
    }

    private void SaveSettings()
    {
        settings = new AudioSettings
        {
            volumeMaster = _volumeMasterSlider.value,
            volumeMusic = _volumeMusicSlider.value,
            volumeSFX = _volumeSFXSlider.value,
            isMasterMuted = settings.isMasterMuted,
            isMusicMuted = settings.isMusicMuted,
            isSFXMuted = settings.isSFXMuted
        };

        SaveSystem.Save(settings, SETTINGS_KEY);
    }

    #endregion

    public enum MixerGroup
    {
        Master,
        Music,
        Sfx
    }
}
