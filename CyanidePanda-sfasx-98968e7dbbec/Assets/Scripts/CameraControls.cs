using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float movementSpeed;
    public float movementTime;
    [Space]
    public bool isMoving;

    private Vector3 target;

    private Coroutine MoveCoroutine;

    private void Update()
    {
        if (Game.character.currentAbility == "" && !Game.instance.isFreeLooking)
            return;

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0.0F, Input.GetAxis("Vertical"));
        if (isMoving)
        {
            if (input.magnitude != 0.0F)
            {
                StopCoroutine(MoveCoroutine);
                isMoving = false;
            }
            else
            {
                return;
            }
        }

        transform.Translate(input * Time.deltaTime * movementSpeed);

        // clamp X
        float sizeX = Environment.instance.Size.x * 10;
        float halfX = (sizeX / 2.0F);

        if (transform.position.x < -halfX + 1.0F)
            transform.position = new Vector3(-halfX + 1.0F, transform.position.y, transform.position.z);
        else if (transform.position.x > halfX - 1.0F)
            transform.position = new Vector3(halfX - 1.0F, transform.position.y, transform.position.z);

        // clamp Z
        float sizeY = Environment.instance.Size.y * 10;
        float halfY = (sizeY / 2.0F);

        if (transform.position.z < -halfY + 1.0F)
            transform.position = new Vector3(transform.position.x, transform.position.y, -halfY + 1.0F);
        else if (transform.position.z > halfY - 1.0F)
            transform.position = new Vector3(transform.position.x, transform.position.y, halfY - 1.0F);
    }

    public void MoveToPosition (Vector3 targetPosition)
    {
        target = targetPosition;
        MoveCoroutine = StartCoroutine(MoveToPos());
    }

    private IEnumerator MoveToPos()
    {
        isMoving = true;

        Vector3 startPosition = transform.position;

        do
        {
            transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * 2.5F);

            yield return null;
        }
        while (Vector3.Distance(transform.position, target) > 0.25F);

        transform.position = target;

        isMoving = false;
    }
}
