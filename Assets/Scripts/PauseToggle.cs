using UnityEngine;

public class PauseToggle : MonoBehaviour
{
    public static bool IsPaused { get; private set; }
    float savedTimeScale = 1f;

    public void Toggle()
    {
        SetPaused(!IsPaused);
    }

    public void SetPaused(bool pause)
    {
        if (pause == IsPaused) return;

        IsPaused = pause;
        if (pause)
        {
            savedTimeScale = Time.timeScale <= 0f ? 1f : Time.timeScale;
            Time.timeScale = 0f;  
        }
        else
        {
            Time.timeScale = savedTimeScale;
        }
    }

    void OnDisable()
    {
        if (IsPaused) Time.timeScale = savedTimeScale;
    }
}
