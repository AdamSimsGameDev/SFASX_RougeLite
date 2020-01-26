using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// an enum that stores the possible targets of an ability
// this is used by the AI to determine who to use an ability on
public enum AbilityTarget { Self, Team, Other }
// this next enum is used to determine what specific entities the AI will attempt to target.
public enum AbilitySort { ByDistance, ByHealth }

[System.Serializable]
public abstract class Ability
{
    // a dictionary that stores all of the abilities that the player can have.
    public static Dictionary<string, Ability> abilities = new Dictionary<string, Ability>()
    {
        { "move", new AbilityMove() },
        { "basic_attack", new AbilityBasicAttack() },
        { "heal", new AbilityHeal() },
        { "fireball", new AbilityFireball() },
    };

    // the current cooldown of this ability
    public int currentCooldown;

    // the maximum cooldown of this ability
    public abstract int maxCooldown { get; }
    // the mana cost of this ability
    public abstract int manaCost { get; }
    // whether this ability can be used on the entity using it (this only matters for the player)
    public abstract bool canUseOnSelf { get; }
    // the damage in which the ability causes
    public abstract int damage { get; }
    // the damage in which the ability causes
    public abstract int range { get; }
    // whether the damage of the weapon should be used instead
    public abstract bool useWeapon { get; }

    // this ability's name.
    public abstract string name { get; }
    // this ability's description.
    public abstract string description { get; }

    // whether this ability is currently on cooldown
    public bool isCooldown { get { return currentCooldown != 0; } }

    // the tiles that this ability can be used on
    protected List<EnvironmentTile> tiles = new List<EnvironmentTile>();
    // the entity using this ability
    protected Entity ourEntity;
    // the target of this ability
    protected Entity target;

    public abstract void Init(Entity entity);

    public virtual IEnumerator Use(bool isAI, EnvironmentTile targetTile)
    {
        ourEntity.IsProcessingAbility = true;
        if (isAI)
        {
            yield return UseAI(targetTile);
        }
        else
        {
            yield return UsePlayer(targetTile);
        }
    }

    protected abstract IEnumerator UseAI(EnvironmentTile targetTile);
    protected abstract IEnumerator UsePlayer(EnvironmentTile targetTile);

    public virtual void EndAbilityUse()
    {
        currentCooldown = maxCooldown;
        ourEntity.mana -= manaCost;
    }

    public abstract void Visualise(EnvironmentTile targetTile);
    public abstract void ClearVisualisation();

    public virtual void EndTurn()
    {
        currentCooldown = Mathf.Clamp(currentCooldown - 1, 0, maxCooldown + 1);
    }

    public virtual void DamageTarget(int amount, DamageType type)
    {
        target.Damage(amount, type);
    }

    public virtual Entity GetTarget() { return null; }
}
