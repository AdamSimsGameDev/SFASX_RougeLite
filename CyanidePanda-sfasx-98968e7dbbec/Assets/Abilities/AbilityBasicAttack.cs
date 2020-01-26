using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityBasicAttack : Ability
{
    public override int maxCooldown => 1;
    public override int manaCost => 0;
    public override bool canUseOnSelf => false;
    public override int damage => 1;
    public override int range => 1;
    public override bool useWeapon => true;

    public override string name => "Basic Attack";
    public override string description => "Attack a target up to 1 tile away.";

    // calculate the possible tiles on the initialization of this ability.
    public override void Init(Entity entity)
    {
        this.ourEntity = entity;

        // clear the existing tiles
        tiles.Clear();

        // get all tiles in the map that contain an enemy.
        List<EnvironmentTile> enemyTiles = Environment.instance.GetAllTilesOfType(EnvironmentTile.TileState.Enemy);

        // define the attack range
        int attackRange = ourEntity.attackRange;

        // loop through the attack range on both the X and Y
        for (int i = -attackRange; i < attackRange + 1; i++)
        {
            for (int j = -attackRange; j < attackRange + 1; j++)
            {
                // get the position based on the new x and y offsets
                Vector2Int target = entity.currentPosition.GridPosition + new Vector2Int(i, j);
                // if the distance is equal to or less than the attack range we can add that tile.
                // we do this to ensure that the attack range is circular.
                if (Vector2.Distance(entity.currentPosition.GridPosition, target) <= attackRange)
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

    protected override IEnumerator UseAI(EnvironmentTile targetTile)
    {
        currentCooldown = maxCooldown;

        Entity entity = targetTile.Occupier.GetComponent<Entity>();
        entity.LookAt(ourEntity.currentPosition.Position);
        ourEntity.LookAt(entity.currentPosition.Position);

        CameraControls.MoveToPosition(ourEntity.transform);

        yield return DoAttack(entity);
    }

    // when the ability is used, check location and then path if possible.
    protected override IEnumerator UsePlayer(EnvironmentTile targetTile)
    {
        if (tiles.Contains(targetTile) && targetTile != ourEntity.currentPosition)
        {
            EndAbilityUse();

            Entity entity = targetTile.Occupier.GetComponent<Entity>();
            entity.LookAt(ourEntity.currentPosition.Position);
            ourEntity.LookAt(entity.currentPosition.Position);

            ClearVisualisation();

            yield return DoAttack(entity);
        }
        else
        {
            ourEntity.IsProcessingAbility = false;
        }
    }

    /// <summary>
    /// Attacks a given entity.
    /// </summary>
    /// <param name="target"></param>
    public IEnumerator DoAttack(Entity target)
    {
        base.target = target;
        ourEntity.animator.SetTrigger("Attack");

        yield return new WaitForSeconds(ourEntity.animator.GetCurrentAnimatorStateInfo(0).length);

        ourEntity.FinishUsingAbility();
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
            BorderVisualiser.Visualise(tiles, Color.red);
        }
    }

    // clears the visualisation of the path and border
    public override void ClearVisualisation()
    {
        if (ourEntity is Character)
        {
            BorderVisualiser.Clear();
        }
    }

    public override Entity GetTarget()
    {
        // get the player
        return Game.character;
    }
}
