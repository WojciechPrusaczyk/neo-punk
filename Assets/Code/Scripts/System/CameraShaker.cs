using UnityEngine;
using Cinemachine;

public class CameraShaker : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCam;
    public float shakeDuration = 0.2f;
    public float shakeAmplitude = 1.2f;
    public float shakeFrequency = 2.0f;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeTimer;

    void Start()
    {
        if (virtualCam == null)
        {
            Debug.LogError("CameraShaker: Brak przypisanej virtualCam.");
            return;
        }

        noise = virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null)
        {
            Debug.LogError("CameraShaker: Brak CinemachineBasicMultiChannelPerlin na kamerze.");
        }
    }

    void Update()
    {
        if (noise == null) return;

        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                noise.m_AmplitudeGain = 0f;
                noise.m_FrequencyGain = 0f;
            }
        }
    }

    public void DoScreenShake()
    {
        if (noise == null) return;

        noise.m_AmplitudeGain = shakeAmplitude;
        noise.m_FrequencyGain = shakeFrequency;
        shakeTimer = shakeDuration;
    }
}