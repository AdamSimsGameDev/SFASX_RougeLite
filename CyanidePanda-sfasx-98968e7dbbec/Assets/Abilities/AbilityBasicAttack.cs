using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBasicAttack : Ability
{
    public override int maxCooldown => 1;
    public override int staminaCost => 1;
    public override bool canUseOnSelf => false;

    // calculate the possible tiles on the initialization of this ability.
    public override void Init(Character player)
    {
        this.player = player;

        tiles.Clear();

        List<EnvironmentTile> enemyTiles = Environment.instance.GetAllTilesOfType(EnvironmentTile.TileState.Enemy);

        int attackRange = 2;

        for (int i = -attackRange; i < attackRange + 1; i++)
        {
            for (int j = -attackRange; j < attackRange + 1; j++)
            {
                Vector2Int target = player.currentPosition.GridPosition + new Vector2Int(i, j);
                if (Vector2.Distance(player.currentPosition.GridPosition, target) <= attackRange)
                {
                    EnvironmentTile tile = Environment.instance.GetTile(target.x, target.y);
                    if (enemyTiles.Contains(tile))
                    {
                        tiles.Add(tile);
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
            currentCooldown = maxCooldown;

            player.transform.rotation = Quaternion.LookRotation(targetTile.Position - player.currentPosition.Position, Vector3.up);
            targetTile.Occupier.GetComponent<Enemy>().Damage(1); // replace 1 in future with weapon damage

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
        BorderVisualiser.Visualise(tiles, Color.red);
    }

    // clears the visualisation of the path and border
    public override void ClearVisualisation()
    {
        BorderVisualiser.Clear();
    }
}
