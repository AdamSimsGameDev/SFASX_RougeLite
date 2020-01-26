using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityFireball : Ability
{
    public override int maxCooldown => 3;
    public override int manaCost => 30;
    public override bool canUseOnSelf => false;
    public override int damage => 10;
    public override int range => 3;
    public override bool useWeapon => true;

    public override string name => "Fireball";
    public override string description => "Cast a ball of fire towards an enemy.";

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

        // the current lerp
        float lerp = 0.0F;

        // the start position
        Vector3 startPosition = ourEntity.transform.position + Vector3.up * 5F;
        // the end position
        Vector3 endPosition = target.transform.position + Vector3.up * 5F;

        // because we want to wait until both the animation is finished AND the attack has hit
        // we can declare the start time here
        float startTime = ourEntity.animator.GetCurrentAnimatorStateInfo(0).length;

        // distance between start and end points
        float distance = Vector3.Distance(startPosition, endPosition);
        // the speed in which the projectile moves
        float speed = 50.0F;

        do
        {
            if (graphics == null)
            {
                // reduce the startTime over time
                startTime -= Time.deltaTime;
            }
            else
            {
                lerp = Mathf.Clamp01(lerp + (Time.deltaTime * speed) / distance);
                // set the position
                graphics.transform.position = Vector3.Lerp(startPosition, endPosition, lerp);
                // look at the end point
                graphics.transform.LookAt(endPosition);
            }
            // wait until the end of frame
            yield return null;
        }
        while (lerp != 1.0F);

        // get the particle system
        Transform ps = graphics.transform.Find("ParticleSystem");
        ps.parent = null;
        ps.GetComponent<ParticleSystem>().Stop();

        // destroy the graphics
        Object.Destroy(graphics);

        // damage the entity
        target.Damage(damage, DamageType.Fire);

        // if the start time is > 0 then we have to wait for the animation to finish
        if (startTime > 0.0F)
        {
            yield return new WaitForSeconds(startTime);
        }

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
        // create the projectile graphics
        GameObject go = Object.Instantiate((GameObject)Resources.Load("Abilities/Fireball"));
        graphics = go;

        // returns as we don't want the animation to do damage
        return;
    }
}
