using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : Entity
{
    // the destination that the character is currently looking at pathing towards.
    public EnvironmentTile targetTile { get; set; }
    // the previous destination that the character was looking at pathing towards.
    private EnvironmentTile lastTargetTile { get; set; }

    // the player's current ability
    public string currentAbility;
    // the player's current hovered ability
    public string hoveredAbility;

    // the path visualiser attached to this character
    [HideInInspector] public PathVisualiser pathVisualiser;

    // the last attacked enemy
    private Enemy target;

    protected override void OnUpdate()
    {
        // if the character is now looking at a different tile, revisualise the ability
        if (targetTile != lastTargetTile)
        {
            if (currentAbility != "")
            {
                if (mana >= abilities[currentAbility].manaCost && !abilities[currentAbility].isCooldown)
                {
                    abilities[currentAbility].Visualise(targetTile);
                }
            }
            lastTargetTile = targetTile;
        }

        // update the characters moving animation
        animator.SetBool("IsWalking", IsMoving);
    }

    /// <summary>
    /// Initializes the player's variables when the level starts.
    /// </summary>
    public void Init()
    {
        pathVisualiser = Instantiate((GameObject)Resources.Load("PathVisualiser")).GetComponent<PathVisualiser>();
        pathVisualiser.current = currentPosition;

        AddAbility("move");
        AddAbility("basic_attack");

        foreach (KeyValuePair<string, Ability> kvp in abilities)
        {
            kvp.Value.Init(this);
        }

        SetWeapon(Global.instance.weapon);
        for (int i = 0; i < 4; i++)
        {
            SetArmour(i, Global.instance.armour[i]);
        }
    }

    /// <summary>
    /// Adds an ability to the player's ability list.
    /// </summary>
    /// <param name="tag"></param>
    public void AddAbility(string tag)
    {
        abilities.Add(tag, Ability.abilities[tag]);
    }

    /// <summary>
    /// Sets the current ability.
    /// </summary>
    /// <param name="ability"></param>
    public void SetCurrentAbility (string ability)
    {
        // if the last ability wasn't null, clear it's visualisation
        if (currentAbility != "")
            abilities[currentAbility].ClearVisualisation();

        // if the new ability isn't null we need to do a few checks
        if (ability != "")
        {
            // re-initialize the ability
            abilities[ability].Init(this);

            // if the ability is on cooldown however we don't want the player to be able to use it.
            if (abilities[ability].isCooldown)
                return;
            // the same applies if the player doesn't have enough mana
            if (mana < abilities[ability].manaCost)
                return;
        }
        // if neither of these are true we set the current ability.
        currentAbility = ability;

        // set the visualisation
        if (currentAbility != "")
        {
            abilities[currentAbility].Visualise(targetTile);
        }
    }
    /// <summary>
    /// Sets the hovered ability.
    /// </summary>
    /// <param name="ability"></param>
    public void SetHoveredAbility(string ability)
    {
        hoveredAbility = ability;
    }

    /// <summary>
    /// Use an ability as the player and set it's hovered ability to be empty.
    /// </summary>
    /// <param name="isAI"></param>
    /// <param name="ability"></param>
    /// <param name="targetTile"></param>
    public override void UseAbility(bool isAI, string ability, EnvironmentTile targetTile)
    {
        // is on cooldown
        base.UseAbility(isAI, ability, targetTile);

        // if the current ability wasn't on cooldown originally
        // set the hovered ability and current ability to be empty
        if (ability != "")
        {
            SetHoveredAbility("");
        }
    }

    /// <summary>
    /// Finishes using the character's current ability.
    /// </summary>
    public override void FinishUsingAbility()
    {
        base.FinishUsingAbility();

        // sets the current ability to non-existant
        SetCurrentAbility("");
        // set the current menu to the previous menu
        Game.ui.ReturnToPreviousMenu();
        // update the path visualiser's current position (just in case it changed)
        pathVisualiser.current = currentPosition;
    }

    /// <summary>
    /// This is ran when the turn ends, updating the player's variables.
    /// </summary>
    public void EndTurn()
    {
        mana = maxMana;

        foreach(KeyValuePair<string, Ability> kvp in abilities)
        {
            Ability ab = kvp.Value;

            ab.EndTurn();
            ab.Init(this);
        }

        // reset the menu to the basic action menu
        Game.ui.SetCurrentMenu("actionsMenu");
    }

    /// <summary>
    /// The function that is ran when the player's feet touch the ground in the animations.
    /// </summary>
    public void Footstep()
    {

    }

    /// <summary>
    /// Damages the target enemy when the animation trigger is activated.
    /// </summary>
    public void Hit ()
    {
        int d = 0;
        if (lastUsedAbility.useWeapon)
        {
            // if we use the weapon damage, set the damage dealt to our attack damage
            d = weapon == "" ? attackDamage : ((Weapon)Inventory.instance.allItems[weapon]).damage;
        }
        else
        {
            // otherwise set it to the damage the ability does
            d = lastUsedAbility.damage;
        }
        abilities[currentAbility].DamageTarget(d, weapon == "" ? attackElement : ((Weapon)Inventory.instance.allItems[weapon]).element);
    }

    public override void Damage(int baseAmount, DamageType type)
    {
        base.Damage(baseAmount, type);

        animator.SetTrigger("Hit");
    }

    /// <summary>
    /// Handles the death of the player.
    /// </summary>
    public override void Die()
    {
        base.Die();

        Game.instance.Lose();
        CameraControls.MoveToPosition(transform);
    }
}
