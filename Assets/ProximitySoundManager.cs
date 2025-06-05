using UnityEngine;

public class ProximitySoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] audioClips;

    [Header("AudioSource Settings (Set in Inspector or here)")]
    [SerializeField] private float minSoundDistance = 1f;
    [SerializeField] private float maxSoundDistance = 15f;

    private Player player => WorldGameManager.instance?.player;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        ConfigureAudioSource();

        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning("ProximitySoundManager: No audio clips assigned on " + gameObject.name, this);
            enabled = false;
            return;
        }
    }

    private void ConfigureAudioSource()
    {
        audioSource.spatialBlend = 1.0f;
        audioSource.minDistance = this.minSoundDistance;
        audioSource.maxDistance = this.maxSoundDistance;
        audioSource.loop = true;

        audioSource.rolloffMode = AudioRolloffMode.Custom;

        AnimationCurve customRolloffCurve = new AnimationCurve(
            new Keyframe(0f, 1.0f, 0f, -1f),
            new Keyframe(1f, 0.0f, -1f, 0f)
        );

        audioSource.SetCustomCurve(AudioSourceCurveType.CustomRolloff, customRolloffCurve);
    }

    private void Update()
    {
        if (player == null)
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            return;
        }

        float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);

        if (distanceToPlayer < audioSource.maxDistance)
        {
            if (!audioSource.isPlaying)
            {

                // Wybierz losowy dŸwiêk z tablicy audioClips
                AudioClip randomClip = audioClips[Random.Range(0, audioClips.Length)];
                audioSource.clip = randomClip;

                float baseVolume;
                if (WorldSoundFXManager.instance != null)
                {
                    baseVolume = WorldSoundFXManager.instance.sfxVolume;
                }
                else
                {
                    baseVolume = 1f;
                }

                audioSource.volume = baseVolume * 0.5f;

                audioSource.Play();
            }
        }
        else
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        float currentMin = Application.isPlaying && audioSource != null ? audioSource.minDistance : minSoundDistance;
        float currentMax = Application.isPlaying && audioSource != null ? audioSource.maxDistance : maxSoundDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentMin);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, currentMax);
    }
}