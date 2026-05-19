using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private TextMeshProUGUI fpsText;

    [Header("Settings")]
    [SerializeField] private float updateInterval = 0.5f;

    private float deltaTimeAccumulator = 0f;
    private int frameCount = 0;
    private float timeLeft;

    void Start()
    {
        timeLeft = updateInterval;
        
        // Try to get the component automatically if not assigned in Inspector
        if (fpsText == null)
        {
            fpsText = GetComponent<TextMeshProUGUI>();
        }
    }

    void Update()
    {
        // Use unscaledDeltaTime so the counter works even if Time.timeScale is 0 (paused game)
        timeLeft -= Time.unscaledDeltaTime;
        deltaTimeAccumulator += Time.unscaledDeltaTime;
        frameCount++;

        // Interval ended - update the UI
        if (timeLeft <= 0f)
        {
            if (fpsText != null)
            {
                float fps = frameCount / deltaTimeAccumulator;
                float frameTimeMs = (deltaTimeAccumulator / frameCount) * 1000f;

                // F0 hides decimals for FPS, F1 shows one decimal place for milliseconds
                fpsText.text = string.Format("{0:F0} FPS\n{1:F1} ms", fps, frameTimeMs);
            }

            // Reset counters for the next interval
            timeLeft = updateInterval;
            deltaTimeAccumulator = 0f;
            frameCount = 0;
        }
    }
}