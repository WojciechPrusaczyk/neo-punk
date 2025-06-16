using System;
using UnityEngine;

public class HomelessManAi : MonoBehaviour
{
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float interactionRange = 4f;

    private Player player;

    private Animator animator;
    private Transform appearanceTransform;
    private bool isWaving = false;
    private bool isAbleToTalk = false;
    public GameObject tooltip;

    private DialogScript dialogInterface;
    public GameObject dialogInterfaceObject;
    private UserInterfaceController userInterfaceController;
    public GameObject mainUserInterfaceControllerObject;

    private GameObject EventsPage;
    private EventFlagsSystem _EventsFlagsSystem;

    private void Awake()
    {
        player = FindFirstObjectByType<Player>();
        dialogInterface = dialogInterfaceObject.GetComponent<DialogScript>();
        userInterfaceController = mainUserInterfaceControllerObject.GetComponent<UserInterfaceController>();
        EventsPage = GameObject.Find("EventsFlags");
        _EventsFlagsSystem = EventsPage.GetComponent<EventFlagsSystem>();

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player GameObject has the tag 'Player'.");
        }

        appearanceTransform = transform.Find("Appearance");
        if (appearanceTransform == null)
        {
            Debug.LogError("Appearance child not found!");
        }
        else
        {
            animator = appearanceTransform.GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogError("Animator component missing on Appearance.");
            }
        }
    }

    private void Update()
    {
        if (player == null || appearanceTransform == null || animator == null) return;

        // Obracanie sprite w stronę gracza
        Vector3 directionToPlayer = player.transform.position - transform.position;
        Vector3 scale = appearanceTransform.localScale;
        scale.x = directionToPlayer.x >= 0 ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x); // Flip X jeśli gracz po prawej
        appearanceTransform.localScale = scale;

        // Sprawdzanie dystansu do gracza
        float distance = Vector3.Distance(transform.position, player.transform.position);
        isAbleToTalk = Vector3.Distance(transform.position, player.transform.position) <= interactionRange;

        tooltip.SetActive(isAbleToTalk);

        if (isAbleToTalk && Input.GetKeyDown(InputManager.InteractKey))
        {
            tooltip.SetActive(false);

            EnterDialog();
        }

        if ( distance <= detectionRange ) {
            if (!isWaving)
            {
                animator.SetTrigger("wave");
                isWaving = true;
            }
        } else {
            if (isWaving)
            {
                animator.SetTrigger("stopWaving");
                isWaving = false;
            }
        }
    }

    private void EnterDialog()
    {
        tooltip.SetActive(false);
        animator.SetTrigger("stopWaving");

        // Tymczasowo próbujemy zakończyć aktualną misję
        // To będzie można przenieść do innego miejsca
        PlayerObjectiveTracker.instance.FinishCurrentMission();

        if (!_EventsFlagsSystem.IsEventDone("homelessManFirstInteraction"))
        {
            dialogInterface.StartDialog(0);
            _EventsFlagsSystem.FinishEvent("homelessManFirstInteraction");
        }
        else if (!_EventsFlagsSystem.IsEventDone("doneFirstArena"))
            dialogInterface.StartDialog(1);
        else if (!_EventsFlagsSystem.IsEventDone("doneFirstTimeTrial"))
            dialogInterface.StartDialog(2);
        else if (!_EventsFlagsSystem.IsEventDone("hasPaid"))
            dialogInterface.StartDialog(3);
        else
            dialogInterface.StartDialog(4);
    }
}