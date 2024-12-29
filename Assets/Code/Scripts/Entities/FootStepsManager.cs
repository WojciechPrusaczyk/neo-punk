using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FootStepsManager : MonoBehaviour
{
    [Serializable]
    [Tooltip("List of floor types and their associated sounds.")]
    public class FloorTypes
    {
        [Tooltip("Note that name must be similar to name in floor's customTag name.")]
        [SerializeField] public string name;
        
        [Tooltip("List of possible steps variants.")]
        public List<AudioClip> effectVariants = null; 
    }
    
    public List<FloorTypes> floorTypes = new List<FloorTypes>();
    public float pauseBetweenSteps;
    public GameObject emitterEntity;

    private FloorDetector _floorDetector;
    private AudioSource _footstepsAudioSource;
    private bool isPlayingFootsteps;

    private void Awake()
    {
        _floorDetector = GetComponent<FloorDetector>();
        if (!_floorDetector) Debug.LogError("Not found associated floor detector in gameObject.");
        
        _footstepsAudioSource = GetComponent<AudioSource>();
        if (!_footstepsAudioSource) Debug.LogError("Not found associated audio source in gameObject.");
    }

    private void Update()
    {
        if (!isPlayingFootsteps && _floorDetector && _floorDetector.collidingObject)
        {
            CustomTags customTags = _floorDetector.collidingObject.GetComponent<CustomTags>();
            if (customTags)
            {
                foreach (var tagName in customTags.GetTags())
                {
                    foreach (var floorType in floorTypes)
                    {
                        if (floorType.name == tagName)
                        {
                            isPlayingFootsteps = true;
                            int index = floorTypes.FindIndex(ft => ft.name == tagName);
                            
                            StartCoroutine(PlayStep(index));
                        }
                    }
                }
            }
            else
            {
                int defaultIndex = floorTypes.FindIndex(ft => ft.name == "default");
                if (defaultIndex != -1)
                {
                    isPlayingFootsteps = true;
                    StartCoroutine(PlayStep(defaultIndex));
                }
            }
        }
    }

    private IEnumerator PlayStep(int floorTypeIndex)
    {
        while (_floorDetector && _floorDetector.collidingObject)
        {
            var collidingObject = _floorDetector.collidingObject;

            // Pobieranie tagów obiektu
            var customTags = collidingObject.GetComponent<CustomTags>()?.GetTags();
            if (customTags == null || !customTags.Contains(floorTypes[floorTypeIndex].name))
            {
                // Jeśli brak tagu, ustawiamy domyślny typ podłogi
                int defaultIndex = floorTypes.FindIndex(ft => ft.name == "default");
                if (defaultIndex != -1)
                {
                    floorTypeIndex = defaultIndex;
                }
            }

            // Pobranie dźwięków z odpowiedniego typu podłogi
            var effects = floorTypes[floorTypeIndex].effectVariants;
            var isEntityWalking = emitterEntity.GetComponent<Rigidbody2D>().velocity.magnitude > 0.1f;

            if (effects != null && effects.Count > 0 && isEntityWalking)
            {
                int randomIndex = UnityEngine.Random.Range(0, effects.Count);
                AudioClip stepSound = effects[randomIndex];
                AudioSource.PlayClipAtPoint(stepSound, transform.position);
            }

            yield return new WaitForSeconds(pauseBetweenSteps);
        }

        isPlayingFootsteps = false;
    }
}
