using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalDoorController : MonoBehaviour
{
    public bool IsLocked = false;
    public Sprite InteractIcon;
    public Sprite LockedIcon;
    public GameObject IconParent;
    public GameObject DoorObject;
    public GameObject LightsParent;
    public float OpeningSpeed = 0.5f;
    public float PauseBetweenOpeningStages = 0.1f;

    public bool IsOpen;
    private bool IsPlayerNear = false;
    private bool AreDoorOpening = false;
    private float InitialPosition;
    private float TargetPosition;
    private float DoorHeight;
    private void Awake()
    {
        InitialPosition = DoorObject.transform.position.y;
        DoorHeight = DoorObject.GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void Update()
    {
        if (IsPlayerNear && Input.GetKeyDown(InputManager.InteractKey))
        {
            if (!IsLocked && !AreDoorOpening)
            {
                if (IsOpen) CloseDoor();
                else OpenDoor();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SpriteRenderer spriteRenderer = IconParent.GetComponent<SpriteRenderer>();

            if (IsLocked)
            {
                spriteRenderer.sprite = LockedIcon;
            }
            else
            {
                spriteRenderer.sprite = InteractIcon;
            }
            IsPlayerNear = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SpriteRenderer spriteRenderer = IconParent.GetComponent<SpriteRenderer>();

            spriteRenderer.sprite = null;
            IsPlayerNear = false;
        }
    }

    public void OpenDoor()
    {
        StartCoroutine(ToggleDoor());

        LightsParent.SetActive(false);

        IsOpen = true;
    }

    public void CloseDoor()
    {
        StartCoroutine(ToggleDoor());

        IsOpen = false;
    }

    private IEnumerator ToggleDoor()
    {
        AreDoorOpening = true;

        if (!IsOpen)
        {
            float targetY = InitialPosition + DoorHeight;
            while (Mathf.Abs(DoorObject.transform.position.y - targetY) > 0.01f)
            {
                float newY = Mathf.MoveTowards(DoorObject.transform.position.y, targetY, OpeningSpeed * Time.deltaTime);
                DoorObject.transform.position = new Vector3(DoorObject.transform.position.x, newY, DoorObject.transform.position.z);
                yield return null;
            }
        }
        else
        {
            float targetY = InitialPosition;
            while (Mathf.Abs(DoorObject.transform.position.y - targetY) > 0.01f)
            {
                float newY = Mathf.MoveTowards(DoorObject.transform.position.y, targetY, OpeningSpeed * Time.deltaTime);
                DoorObject.transform.position = new Vector3(DoorObject.transform.position.x, newY, DoorObject.transform.position.z);
                yield return null;
            }

            LightsParent.SetActive(true);
        }

        AreDoorOpening = false;
    }
}
