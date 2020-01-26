using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBlink : Ability
{
    [SerializeField] private float SingleNodeMoveTime = 0.5f;

    public override int maxCooldown => 3;
    public override int manaCost => 25;
    public override bool canUseOnSelf => false;
    public override int damage => 0;
    public override int range => 3;
    public override bool useWeapon => false;

    public override string name => "Blink";
    public override string description => "Teleport to a location up to 3 tiles.";

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
                            tiles.Add(tile);
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
        currentCooldown = maxCooldown;

        CameraControls.MoveToPosition(ourEntity.transform);

        yield return Teleport(targetTile);
    }
    protected override IEnumerator UsePlayer(EnvironmentTile targetTile)
    {
        if (tiles.Contains(targetTile) && targetTile != ourEntity.currentPosition)
        {
            currentCooldown = maxCooldown;
            ourEntity.mana -= manaCost;

            ClearVisualisation();

            yield return Teleport(targetTile);
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
        }
    }

    // clears the visualisation of the path and border
    public override void ClearVisualisation()
    {
        if (ourEntity is Character)
        {
            Character character = ourEntity as Character;
            BorderVisualiser.Clear();
        }
    }

    private IEnumerator Teleport(EnvironmentTile target)
    {
        // set the last position to accessible.
        ourEntity.currentPosition.State = EnvironmentTile.TileState.None;
        ourEntity.currentPosition.Occupier = null;

        // get the new position
        ourEntity.currentPosition = target;

        // set the current position to inaccessible.
        ourEntity.currentPosition.State = ourEntity is Character ? EnvironmentTile.TileState.Player : EnvironmentTile.TileState.Enemy;
        ourEntity.currentPosition.Occupier = ourEntity.gameObject;

        ourEntity.transform.position = target.Position;

        yield return new WaitForSeconds(0.4F);

        foreach (KeyValuePair<string, Ability> kvp in ourEntity.abilities)
        {
            kvp.Value.Init(ourEntity);
        }

        ourEntity.FinishUsingAbility();
    }
}
