using UnityEngine;
using UnityEngine.Events; // Required for UnityAction

public class Timer : MonoBehaviour
{
    [Header("Timer variables")]
    public float currentTime;
    public bool startFromZero = true;
    public float duration = 10f;

    public bool isTimerRunning = false;
    
    private float startTime = 0;

    [Header("Timeout Action")]
    public UnityEvent onTimeout;

    private void Awake()
    {
        if (startFromZero)
            currentTime = 0f;
        else
            currentTime = startTime > 0 ? startTime : duration;
    }

    private void Update()
    {
        
        if (isTimerRunning)
        {
            if (startFromZero)
            {
                currentTime += Time.deltaTime;
                if (currentTime >= duration)
                {
                    isTimerRunning = false;
                    currentTime = duration; // Clamp to duration
                    Timeout();
                }
            }
            else
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0)
                {
                    isTimerRunning = false;
                    currentTime = 0; // Clamp to zero
                    Timeout();
                }
            }
        }
    }

    public void StartTimer(float newDuration)
    {
        duration = newDuration;
        ResetTimer();
        isTimerRunning = true;
    }

    public void PauseTimer()
    {
        isTimerRunning = false;
    }

    public void ResumeTimer()
    {
        isTimerRunning = true;
    }

    public void ResetTimer()
    {
        currentTime = startFromZero ? 0f : (startTime > 0 ? startTime : duration);
    }

    private void Timeout()
    {
        onTimeout?.Invoke();
    }
}