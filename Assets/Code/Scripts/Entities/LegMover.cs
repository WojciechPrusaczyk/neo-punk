using System.Collections;
using UnityEngine;

public class LegMover : MonoBehaviour
{
    public Transform legTarget; // Cel dla solvera Fabrik
    public Transform bodyTarget; // Punkt referencyjny na ciele
    public LayerMask groundLayer;
    public float hoverDist = 0.1f;
    public float groundCheckDistance = 2f;
    public float legMoveDist = 0.5f;
    public float liftDistance = 0.3f;
    public float legMovementSpeed = 5f;

    private Vector3 targetPoint;
    private Vector3 oldPos;
    private Vector3 halfWayPoint;
    private bool isMoving = false;

    void Start()
    {
        if (legTarget == null)
        {
            Debug.LogError($"{gameObject.name}: legTarget nie został przypisany!");
        }
    }

    void Update()
    {
        // Aktualizacja pozycji targetu względem ciała
        Vector3 desiredTargetPosition = bodyTarget.position;

        // Sprawdzenie, czy noga powinna wykonać krok
        if (!isMoving && Vector2.Distance(legTarget.position, desiredTargetPosition) > legMoveDist)
        {
            StartCoroutine(MoveLeg(desiredTargetPosition));
        }
    }

    IEnumerator MoveLeg(Vector3 newTargetPosition)
    {
        isMoving = true;
        oldPos = legTarget.position;
        targetPoint = FindGroundPosition(newTargetPosition);
        halfWayPoint = (targetPoint + legTarget.position) / 2;
        halfWayPoint.y += liftDistance;

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            legTarget.position = Vector3.Lerp(oldPos, halfWayPoint, elapsedTime);
            elapsedTime += Time.deltaTime * legMovementSpeed;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            legTarget.position = Vector3.Lerp(halfWayPoint, targetPoint, elapsedTime);
            elapsedTime += Time.deltaTime * legMovementSpeed;
            yield return null;
        }

        isMoving = false;
    }

    Vector3 FindGroundPosition(Vector3 checkPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPosition + Vector3.up, Vector2.down, groundCheckDistance, groundLayer);
        if (hit.collider != null)
        {
            return hit.point + (Vector2.up * hoverDist);
        }
        return checkPosition;
    }
}