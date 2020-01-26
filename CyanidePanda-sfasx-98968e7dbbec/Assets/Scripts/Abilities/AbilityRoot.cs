using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityRoot : Ability
{
    public override int maxCooldown => 4;
    public override int manaCost => 25;
    public override bool canUseOnSelf => false;
    public override int damage => 4;
    public override int range => 2;
    public override bool useWeapon => true;

    public override string name => "Root";
    public override string description => "Roots an enemy for two turns, stopping it from moving.";

    public GameObject graphics;

    // calculate the possible tiles on the initialization of this ability.
    public override void Init(Entity entity)
    {
        this.ourEntity = entity;

        // clear the existing tiles
        tiles.Clear();

        // nullify the graphics
        graphics = null;

        // get all tiles in the map that contain an enemy.
        List<EnvironmentTile> enemyTiles = Environment.instance.GetAllTilesOfType(EnvironmentTile.TileState.Enemy);

        // loop through the attack range on both the X and Y
        for (int i = -range; i < range + 1; i++)
        {
            for (int j = -range; j < range + 1; j++)
            {
                // get the position based on the new x and y offsets
                Vector2Int target = entity.currentPosition.GridPosition + new Vector2Int(i, j);
                // if the distance is equal to or less than the attack range we can add that tile.
                // we do this to ensure that the attack range is circular.
                if (Vector2.Distance(entity.currentPosition.GridPosition, target) <= range)
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

        // finish using the ability
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

    /// <summary>
    /// Clears the ability visualisation.
    /// </summary>
    public override void ClearVisualisation()
    {
        if (ourEntity is Character)
        {
            BorderVisualiser.Clear();
        }
    }

    /// <summary>
    /// Gets the entity that the AI will target with this ability.
    /// </summary>
    /// <returns></returns>
    public override Entity GetTarget()
    {
        // get the player
        return Game.character;
    }

    /// <summary>
    /// Damages the target entity.
    /// </summary>
    /// <param name="amount"></param>
    /// <param name="type"></param>
    public override void DamageTarget(int amount, DamageType type)
    {
        // root the target
        target.RootTurns = 2;

        // returns as we don't want the animation to do damage
        return;
    }
}
