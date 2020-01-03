using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private float SingleNodeMoveTime = 0.5f;

    // stats
    public int health;
    public int maxHealth;
    [Space]
    public int stamina;
    public int maxStamina;

    // the characters current position
    public EnvironmentTile currentPosition { get; set; }

    // the destination that the character is currently looking at pathing towards.
    public EnvironmentTile targetTile { get; set; }
    // the previous destination that the character was looking at pathing towards.
    private EnvironmentTile lastTargetTile { get; set; }

    public bool IsMoving { get; set; }

    private Ability currentAbility;

    // the path visualiser attached to this character
    public PathVisualiser pathVisualiser;

    private void Start()
    {
        pathVisualiser = Instantiate((GameObject)Resources.Load("PathVisualiser")).GetComponent<PathVisualiser>();
    }
    public void Init()
    {
        pathVisualiser.current = currentPosition;

        currentAbility = new AbilityMove();
        currentAbility.Init(this);
    }
    private void Update()
    {
        // if the character is now looking at a different tile, revisualise the ability
        if (targetTile != lastTargetTile)
        {
            if (currentAbility != null) currentAbility.Visualise(targetTile);
            lastTargetTile = targetTile;
        }
    }

    public void UseCurrentAbility ()
    {
        if (stamina >= currentAbility.staminaCost && !currentAbility.isCooldown)
        {
            if (currentAbility.Use(targetTile))
            {
                stamina -= currentAbility.staminaCost;

                SetCurrentAbility(-1);
            }
        }
    }
    public void SetCurrentAbility (int ability)
    {
        currentAbility.ClearVisualisation();
        currentAbility = null;

        if (ability == -1)
            return;
        
        // currentAbility = abilities[ability];
    }

    private IEnumerator DoMove(Vector3 position, Vector3 destination)
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
    private IEnumerator DoGoTo(List<EnvironmentTile> route)
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

                // remove the last path position from the visualiser
                pathVisualiser.RemoveAtTile(route[count]);

                // set the last position to accessible.
                currentPosition.State = EnvironmentTile.TileState.None;
                currentPosition.Occupier = null;

                // get the new position
                currentPosition = route[count];

                // set the current position to inaccessible.
                currentPosition.State = EnvironmentTile.TileState.Player;
                currentPosition.Occupier = gameObject;

                position = next;
            }

            IsMoving = false;
        }

        // update the path visualiser's current position
        pathVisualiser.current = currentPosition;
    }
    public void GoTo(List<EnvironmentTile> route)
    {
        // Clear all coroutines before starting the new route so 
        // that clicks can interupt any current route animation
        StopAllCoroutines();
        StartCoroutine(DoGoTo(route));
    }
}
