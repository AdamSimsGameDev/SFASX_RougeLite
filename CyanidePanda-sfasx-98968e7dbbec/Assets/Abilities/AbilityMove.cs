using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMove : Ability
{
    [SerializeField] private float SingleNodeMoveTime = 0.5f;

    public override int maxCooldown => 1;
    public override int manaCost => 0;
    public override bool canUseOnSelf => false;
    public override int damage => 0;
    public override int range => 3;
    public override bool useWeapon => false;

    public override string name => "Move";
    public override string description => "Moves the player up to 3 tiles.";

    // calculate the possible tiles on the initialization of this ability.
    public override void Init(Entity entity)
    {
        this.ourEntity = entity;
        
        tiles.Clear();

        for (int i = -range; i < range + 1; i++)
        {
            for (int j = -range; j < range + 1; j++)
            {
                Vector2Int target = entity.currentPosition.GridPosition + new Vector2Int(i, j);
                if (Vector2.Distance(entity.currentPosition.GridPosition, target) <= range)
                {
                    EnvironmentTile tile = Environment.instance.GetTile(target.x, target.y);
                    if (tile != null)
                    {
                        // if we can path to that tile, or the tile is our own
                        if (tile.State == EnvironmentTile.TileState.None)
                        {
                            List<EnvironmentTile> path = Environment.instance.Solve(entity.currentPosition, tile);
                            // ensure that we can reach exists and that tile within the given number of steps.
                            if (path != null)
                            {
                                if (path.Count <= range + 1)
                                {
                                    tiles.Add(tile);
                                }
                            }
                        }
                        else if (tile.Occupier == entity.gameObject)
                        {
                            tiles.Add(tile);
                        }
                    }
                }
            }
        }
    }

    protected override IEnumerator UseAI(EnvironmentTile targetTile)
    {
        // the test enemy will move towards the player
        // this will be frightening to watch
        List<EnvironmentTile> route = Environment.instance.Solve(ourEntity.currentPosition, targetTile);
        route.RemoveAt(route.Count - 1);

        // create a second list which will be a shortened version of the route.
        // this route will be the first n amount of tiles, with n being the movement range.
        List<EnvironmentTile> shortRoute = new List<EnvironmentTile>();

        if (route.Count <= range)
        {
            shortRoute = route;
        }
        else
        {
            for (int i = 0; i < range; i++)
            {
                shortRoute.Add(route[i]);
            }
        }

        currentCooldown = maxCooldown;

        CameraControls.MoveToPosition(ourEntity.transform);

        yield return DoGoTo(shortRoute);
    }
    protected override IEnumerator UsePlayer(EnvironmentTile targetTile)
    {
        if (tiles.Contains(targetTile) && targetTile != ourEntity.currentPosition)
        {
            currentCooldown = maxCooldown;
            ourEntity.mana -= manaCost;

            List<EnvironmentTile> route = Environment.instance.Solve(ourEntity.currentPosition, targetTile);
            CameraControls.MoveToPosition(ourEntity.transform);

            ClearVisualisation();

            yield return DoGoTo(route);
        }
        else
        {
            ourEntity.IsProcessingAbility = false;
        }
    }

    // the function that is ran at the end of a turn
    public override void EndTurn()
    {
        base.EndTurn();
    }

    // visualise the path and border
    public override void Visualise(EnvironmentTile targetTile)
    {
        if (ourEntity is Character)
        {
            Character character = ourEntity as Character;
            BorderVisualiser.Visualise(tiles, Color.blue);
            character.pathVisualiser.DoUpdate(targetTile, tiles);
        }
    }

    // clears the visualisation of the path and border
    public override void ClearVisualisation()
    {
        if (ourEntity is Character)
        {
            Character character = ourEntity as Character;
            BorderVisualiser.Clear();
            character.pathVisualiser.Clear();
        }
    }

    /// <summary>
    /// Moves the player between two points.
    /// </summary>
    /// <param name="position"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    private IEnumerator DoMove(Vector3 position, Vector3 destination)
    {
        // Move between the two specified positions over the specified amount of time
        if (position != destination)
        {
            ourEntity.transform.rotation = Quaternion.LookRotation(destination - position, Vector3.up);

            Vector3 p = ourEntity.transform.position;
            float t = 0.0f;

            while (t < SingleNodeMoveTime)
            {
                t += Time.deltaTime;
                p = Vector3.Lerp(position, destination, t / SingleNodeMoveTime);
                ourEntity.transform.position = p;
                yield return null;
            }
        }
    }

    /// <summary>
    /// Handles the pathfinding for the player.
    /// </summary>
    /// <param name="route"></param>
    /// <returns></returns>
    public IEnumerator DoGoTo(List<EnvironmentTile> route)
    {
        // Move through each tile in the given route
        if (route != null)
        {
            ourEntity.IsMoving = true;

            Vector3 position = ourEntity.currentPosition.Position;
            for (int count = 0; count < route.Count; ++count)
            {
                Vector3 next = route[count].Position;
                yield return DoMove(position, next);

                // set the last position to accessible.
                ourEntity.currentPosition.State = EnvironmentTile.TileState.None;
                ourEntity.currentPosition.Occupier = null;

                // get the new position
                ourEntity.currentPosition = route[count];

                // set the current position to inaccessible.
                ourEntity.currentPosition.State = ourEntity is Character ? EnvironmentTile.TileState.Player : EnvironmentTile.TileState.Enemy;
                ourEntity.currentPosition.Occupier = ourEntity.gameObject;

                position = next;
            }

            ourEntity.IsMoving = false;

            foreach (KeyValuePair<string, Ability> kvp in ourEntity.abilities)
            {
                kvp.Value.Init(ourEntity);
            }
        }

        if (CameraControls.instance.attachedTarget == ourEntity.transform)
            CameraControls.instance.attachedTarget = null;

        // finish the ability
        ourEntity.FinishUsingAbility();
    }

    /// <summary>
    /// Starts the pathfinding for the player.
    /// </summary>
    /// <param name="route"></param>
    public void GoTo(List<EnvironmentTile> route)
    {
        // Clear all coroutines before starting the new route so 
        // that clicks can interupt any current route animation
        ourEntity.StopAllCoroutines();
        ourEntity.StartCoroutine(DoGoTo(route));
    }
}
