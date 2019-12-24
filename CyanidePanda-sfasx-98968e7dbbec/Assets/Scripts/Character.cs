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

    // possible move locations
    public List<EnvironmentTile> possibleMoves = new List<EnvironmentTile>();

    // the characters current position
    public EnvironmentTile currentPosition
    {
        get
        {
            return CurrentPosition;
        }
        set
        {
            CurrentPosition = value;
            if (IsMoving == false)
                pathVisualiser.current = value;
        }
    }
    private EnvironmentTile CurrentPosition;

    // the destination that the character is currently looking at pathing towards.
    public EnvironmentTile targetTile
    {
        get
        {
            return TargetTile;
        }
        set
        {
            SetDestination(value);
        }
    }
    private EnvironmentTile TargetTile;

    // the previous destination that the character was looking at pathing towards.
    private EnvironmentTile lastTargetTile { get; set; }

    public bool IsMoving { get; set; }

    // the path visualiser attached to this character
    public PathVisualiser pathVisualiser;

    private void Start()
    {
        pathVisualiser = Instantiate((GameObject)Resources.Load("PathVisualiser")).GetComponent<PathVisualiser>();
    }

    private void SetDestination (EnvironmentTile destination)
    {
        // compare the new destination to the last tile
        if (destination != lastTargetTile)
        {
            // if they aren't the same we want to update them
            lastTargetTile = destination;
            TargetTile = destination;

            // we also need to set the path visualisers destination.
            pathVisualiser.destination = targetTile;
            pathVisualiser.DoUpdate(possibleMoves);
        }
    }

    public void FindPossibleMoves ()
    {
        possibleMoves.Clear();

        // temporary range variable.
        int range = 3;

        for (int i = -range; i < range + 1; i++)
        {
            for (int j = -range; j < range + 1; j++)
            {
                Vector2Int target = currentPosition.GridPosition + new Vector2Int(i, j);
                if (Vector2.Distance(currentPosition.GridPosition, target) <= range)
                {
                    EnvironmentTile tile = Environment.instance.GetTile(target.x, target.y);
                    if (tile != null)
                    {
                        // if we can path to that tile, or the tile is our own
                        if (tile.State == EnvironmentTile.TileState.None)
                        {
                            List<EnvironmentTile> path = Environment.instance.Solve(currentPosition, tile);
                            // ensure that we can reach exists and that tile within the given number of steps.
                            if (path != null)
                            {
                                if (path.Count <= range + 1)
                                {
                                    possibleMoves.Add(tile);
                                }
                            }
                        }
                        else if (tile.Occupier == gameObject)
                        {
                            possibleMoves.Add(tile);
                        }
                    }
                }
            }
        }

        BorderVisualiser.instance.color = Color.blue;
        BorderVisualiser.instance.Generate(possibleMoves);
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

        // temporary
        FindPossibleMoves();

        // update the path visualiser's current position
        pathVisualiser.current = currentPosition;
        pathVisualiser.DoUpdate(possibleMoves);
    }
    public void GoTo(List<EnvironmentTile> route)
    {
        // Clear all coroutines before starting the new route so 
        // that clicks can interupt any current route animation
        StopAllCoroutines();
        StartCoroutine(DoGoTo(route));
    }
}
