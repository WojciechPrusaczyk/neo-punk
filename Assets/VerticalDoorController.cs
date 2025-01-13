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
    public float OpeningSpeed = 0.5f;
    public float PauseBetweenOpeningStages = 0.1f;

    private bool IsOpen;
    private bool IsPlayerNear = false;
    private bool AreDoorOpening = false;

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

        float targetY = IsOpen ? 0 : gameObject.transform.localScale.y;
        float startY = DoorObject.transform.position.y;
        float direction = IsOpen ? -1 : 1;

        while (Mathf.Abs(DoorObject.transform.position.y - targetY) > 0.01f)
        {
            DoorObject.transform.position += new Vector3(0, direction * OpeningSpeed * Time.deltaTime, 0);

            yield return null;
        }

        DoorObject.transform.position = new Vector3(DoorObject.transform.position.x, targetY, DoorObject.transform.position.z);

        AreDoorOpening = false;
    }

}
