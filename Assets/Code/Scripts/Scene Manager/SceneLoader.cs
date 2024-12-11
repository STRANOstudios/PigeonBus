using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class SceneLoader : Singleton<SceneLoader>
{
    public static SceneLoader instance;

    [Title("UI Settings")]
    [SerializeField, Required] private Image blackScreen;
    [SerializeField, MinValue(0f)] private float fadeDuration = 1f;

    [Title("Loading Settings")]
    [SerializeField, Range(1f, 10f)] private float maxLoadingTime = 5f; // Maximum loading time before showing loading screen
    [SerializeField, Required] private GameObject loadingScreen; // Loading screen GameObject

    [Title("Debug")]
    [SerializeField] private bool _debug = false;

    public static Action<string> OnSwitchScene;
    public static Action OnSceneLoadComplete;

    private void Awake()
    {
#if UNITY_EDITOR
        Application.targetFrameRate = 60;
#else
        Application.targetFrameRate = 30;
#endif
    }

    private void OnEnable()
    {
        OnSwitchScene += LoadScene;
    }

    private void OnDisable()
    {
        OnSwitchScene -= LoadScene;
    }

    private void Start()
    {
        blackScreen.color = new Color(0, 0, 0, 0);
    }

    /// <summary>
    /// Loads a scene with fade-in and fade-out transition.
    /// </summary>
    /// <param name="sceneName">Name of the scene to load.</param>
    public void LoadScene(string sceneName)
    {
        if (sceneName is null or "")
        {
            if (_debug) Debug.Log("Scene name cannot be null or empty.");
            return;
        }

        StartCoroutine(TransitionToScene(sceneName));
    }

    /// <summary>
    /// Reloads the current scene with fade-in and fade-out transition.
    /// </summary>
    /// <param name="sceneName">Name of the scene to reload.</param>
    public void ReLoadScene(string sceneName)
    {
        if (sceneName is null or "")
        {
            if (_debug) Debug.Log("Scene name cannot be null or empty.");
            return;
        }

        StartCoroutine(TransitionToScene(sceneName, false));
    }

    private IEnumerator TransitionToScene(string sceneName, bool isFadeIn = true)
    {
        if (isFadeIn) yield return StartCoroutine(FadeIn());
        else blackScreen.color = new Color(0, 0, 0, 1);

        // Start loading screen if it takes more than a set amount of time
        loadingScreen?.SetActive(true);

        // Asynchronous scene loading
        yield return StartCoroutine(LoadSceneAsync(sceneName));

        yield return StartCoroutine(FadeOut());

        loadingScreen?.SetActive(false);

        OnSceneLoadComplete?.Invoke();
    }

    private IEnumerator FadeIn()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 1);
    }

    private IEnumerator FadeOut()
    {
        float timerOut = 0f;
        while (timerOut < fadeDuration)
        {
            timerOut += Time.unscaledDeltaTime;
            float alpha = Mathf.Lerp(1, 0, timerOut / fadeDuration);
            blackScreen.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        blackScreen.color = new Color(0, 0, 0, 0);
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        // Timer to check if loading exceeds the max loading time
        float loadStartTime = Time.unscaledTime;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f && Time.unscaledTime - loadStartTime >= maxLoadingTime)
            {
                // Show loading screen if loading time exceeds the max limit
                loadingScreen?.SetActive(true);
            }

            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        // Hide the loading screen once the scene is activated
        loadingScreen?.SetActive(false);
    }

    /// <summary>
    /// Loads multiple scenes in additive mode, such as the main scene and UI.
    /// </summary>
    /// <param name="sceneNames">Array of scene names to load.</param>
    public void LoadMultipleScenes(string[] sceneNames)
    {
        StartCoroutine(LoadMultipleScenesAsync(sceneNames));
    }

    private IEnumerator LoadMultipleScenesAsync(string[] sceneNames)
    {
        foreach (string sceneName in sceneNames)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                }
                yield return null;
            }
        }
    }
}
