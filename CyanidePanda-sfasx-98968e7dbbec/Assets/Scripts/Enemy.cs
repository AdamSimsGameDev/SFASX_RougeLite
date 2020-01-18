using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] private float SingleNodeMoveTime = 0.5f;

    public EnvironmentTile currentPosition { get; set; }
    public bool IsAnimating;

    public bool IsCurrentlyProcessingTurn;
    public bool IsMoving;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        // we can tell whether we are animating based on the current animation state.
        // if the state is our 'Idle' animation, we know that we aren't doing any important animations.
        IsAnimating = !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
    }

    public virtual IEnumerator ProcessTurn ()
    {
        yield return null;
    }

    protected IEnumerator DoMove(Vector3 position, Vector3 destination)
    {
        // Move between the two specified positions over the specified amount of time
        if (position != destination)
        {
            transform.rotation = Quaternion.LookRotation(destination - position, Vector3.up);

            Vector3 p = transform.position;
            float t = 0.0f;

            while (t < SingleNodeMoveTime)
            {
                t += Time.deltaTime;
                p = Vector3.Lerp(position, destination, t / SingleNodeMoveTime);
                transform.position = p;
                yield return null;
            }
        }
    }
    protected IEnumerator DoGoTo(List<EnvironmentTile> route)
    {
        // Move through each tile in the given route
        if (route != null)
        {
            IsMoving = true;

            Vector3 position = currentPosition.Position;
            for (int count = 0; count < route.Count; ++count)
            {
                Vector3 next = route[count].Position;
                yield return DoMove(position, next);

                // set the last position to accessible.
                currentPosition.State = EnvironmentTile.TileState.None;
                currentPosition.Occupier = null;

                // get the new position
                currentPosition = route[count];

                // set the current position to inaccessible.
                currentPosition.State = EnvironmentTile.TileState.Enemy;
                currentPosition.Occupier = gameObject;

                position = next;
            }

            IsMoving = false;
        }
    }
    public void GoTo(List<EnvironmentTile> route)
    {
        // Clear all coroutines before starting the new route so 
        // that clicks can interupt any current route animation
        StopAllCoroutines();
        StartCoroutine(DoGoTo(route));
    }

    public void Damage(float damage)
    {
        Debug.Log("Enemy took " + damage + " damage!");
        animator.SetTrigger("Hit");
    }

    public void LookAt(Vector3 position)
    {
        transform.rotation = Quaternion.LookRotation(position - transform.position, Vector3.up);
    }
}
