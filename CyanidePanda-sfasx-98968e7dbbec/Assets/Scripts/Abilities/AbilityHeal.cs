using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHeal : Ability
{
    public override int maxCooldown => 3;
    public override int manaCost => 30;
    public override bool canUseOnSelf => true;
    public override int damage => 0;
    public override int range => 3;
    public override bool useWeapon => false;

    public override string name => "Heal";
    public override string description => "Heals the player 50HP.";

    // calculate the possible tiles on the initialization of this ability.
    public override void Init(Entity entity)
    {
        this.ourEntity = entity;

        tiles.Clear();
        tiles.Add(ourEntity.currentPosition);
    }

    public override Entity GetTarget()
    {
        List<EnvironmentTile> tiles = Environment.instance.GetAllTilesOfType(EnvironmentTile.TileState.Enemy);

        Entity target = null;
        float lowest = 1000.0F;
        for (int i = 0; i < tiles.Count; i++)
        {
            Entity entity = tiles[i].Occupier.GetComponent<Entity>();
            if (entity == ourEntity)
                continue;

            int distance = Environment.instance.Solve(ourEntity.currentPosition, entity.currentPosition).Count;
            float basePercentage = (entity.health / (float)entity.maxHealth);
            Debug.Log(entity.name + ": " + basePercentage);

            if (basePercentage == 1.0F)
                continue;

            float percentage = basePercentage + (distance * 0.1F);

            if (percentage < lowest)
            {
                target = entity;
                lowest = percentage;
            }
        }

        return target;
    }

    protected override IEnumerator UseAI(EnvironmentTile targetTile)
    {
        Entity e = targetTile.Occupier.GetComponent<Entity>();

        currentCooldown = maxCooldown;

        CameraControls.MoveToPosition(ourEntity.transform);
        ourEntity.FinishUsingAbility();

        yield return new WaitForSeconds(0.1F);

        e.Heal(30);
        CameraControls.MoveToPosition(e.transform);

        yield return new WaitForSeconds(1.0F);
    }
    protected override IEnumerator UsePlayer(EnvironmentTile targetTile)
    {
        if (tiles.Contains(targetTile))
        {
            currentCooldown = maxCooldown;
            ourEntity.mana -= manaCost;

            ClearVisualisation();

            ourEntity.Heal(50);
            ourEntity.FinishUsingAbility();

            yield return null;
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
}
