using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityMove : Ability
{
    public override int maxCooldown => 1;
    public override int staminaCost => 1;
    public override bool canUseOnSelf => false;

    // calculate the possible tiles on the initialization of this ability.
    public override void Init(Character player)
    {
        this.player = player;
        
        tiles.Clear();

        // temporary range variable.
        int range = 3;

        for (int i = -range; i < range + 1; i++)
        {
            for (int j = -range; j < range + 1; j++)
            {
                Vector2Int target = player.currentPosition.GridPosition + new Vector2Int(i, j);
                if (Vector2.Distance(player.currentPosition.GridPosition, target) <= range)
                {
                    EnvironmentTile tile = Environment.instance.GetTile(target.x, target.y);
                    if (tile != null)
                    {
                        // if we can path to that tile, or the tile is our own
                        if (tile.State == EnvironmentTile.TileState.None)
                        {
                            List<EnvironmentTile> path = Environment.instance.Solve(player.currentPosition, tile);
                            // ensure that we can reach exists and that tile within the given number of steps.
                            if (path != null)
                            {
                                if (path.Count <= range + 1)
                                {
                                    tiles.Add(tile);
                                }
                            }
                        }
                        else if (tile.Occupier == player.gameObject)
                        {
                            tiles.Add(tile);
                        }
                    }
                }
            }
        }
    }

    // when the ability is used, check location and then path if possible.
    public override bool Use(EnvironmentTile targetTile)
    {
        if (tiles.Contains(targetTile) && targetTile != player.currentPosition)
        {
            List<EnvironmentTile> route = Environment.instance.Solve(player.currentPosition, player.targetTile);
            player.GoTo(route);

            CameraControls.MoveToPosition(player.transform);

            currentCooldown = maxCooldown;

            return true;
        }

        return false;
    }

    // the function that is ran at the end of a turn
    public override void EndTurn()
    {
        base.EndTurn();
    }

    // visualise the path and border
    public override void Visualise(EnvironmentTile targetTile)
    {
        BorderVisualiser.Visualise(tiles, Color.blue);
        player.pathVisualiser.DoUpdate(targetTile, tiles);
    }

    // clears the visualisation of the path and border
    public override void ClearVisualisation()
    {
        BorderVisualiser.Clear();
        player.pathVisualiser.Clear();
    }
}
