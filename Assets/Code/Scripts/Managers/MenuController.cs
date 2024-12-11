using UnityEngine;

[DisallowMultipleComponent]
public class MenuController : MonoBehaviour
{
    public void PlayButton()
    {
        Time.timeScale = 1;
    }

    public void ExitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    public void PauseButton()
    {
        Time.timeScale = 0;
    }
}