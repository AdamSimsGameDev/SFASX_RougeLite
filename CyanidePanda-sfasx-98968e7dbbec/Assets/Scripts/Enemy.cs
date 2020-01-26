using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : Entity
{
    [Header("Drops")]
    public DropTable dropTable;

    // whether this entity is currently processing it's turn.
    public bool IsCurrentlyProcessingTurn { get; set; }
    // the ability that this enemy uses to move.
    public Ability moveAbility;
    // the ability that this enemy uses to perform a basic attack.
    public Ability attackAbility;
    // the enemies secondary ability.
    public Ability secondaryAbility;

    public bool avoidPlayer;

    private StatText healthText;

    protected override void OnAwake()
    {
        base.OnAwake();
    }
    protected override void OnStart()
    {
        base.OnStart();
        SetupAbilities();
        UpdateHealthText();
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
        // update the enemies moving animation
        animator.SetBool("IsWalking", IsMoving);
    }

    /// <summary>
    /// Sets this enemies equipment using points.
    /// </summary>
    /// <param name="points"></param>
    public virtual void SetEquipment(int points)
    {
        return;
    }

    public virtual void SetEquipment(int points, List<string> armour, Dictionary<string, int> weaponDict)
    {
        List<string> weaponTags = new List<string>();
        foreach (KeyValuePair<string, int> kvp in weaponDict)
        {
            weaponTags.Insert(0, kvp.Key);
        }

        // set the remaining points to points
        int remainingPoints = points;

        // weapon
        for (int i = 0; i < weaponTags.Count; i++)
        {
            if (remainingPoints >= weaponDict[weaponTags[i]])
            {
                SetWeapon(weaponTags[i]);
                remainingPoints -= weaponDict[weaponTags[i]];
                break;
            }
        }

        // armour
        // chestplate first
        for (int i = 0; i < armour.Count; i++)
        {
            int price = ((Armour)Inventory.instance.allItems[armour[i] + "_chestplate"]).defense;
            if (remainingPoints >= price)
            {
                SetArmour(1, armour[i] + "_chestplate");
                remainingPoints -= price;
                break;
            }
        }
        // then leggings
        for (int i = 0; i < armour.Count; i++)
        {
            int price = ((Armour)Inventory.instance.allItems[armour[i] + "_leggings"]).defense;
            if (remainingPoints >= price)
            {
                SetArmour(2, armour[i] + "_leggings");
                remainingPoints -= price;
                break;
            }
        }
        // then helmet
        for (int i = 0; i < armour.Count; i++)
        {
            int price = ((Armour)Inventory.instance.allItems[armour[i] + "_helmet"]).defense;
            if (remainingPoints >= price)
            {
                SetArmour(0, armour[i] + "_helmet");
                remainingPoints -= price;
                break;
            }
        }
        // then boots
        for (int i = 0; i < armour.Count; i++)
        {
            int price = ((Armour)Inventory.instance.allItems[armour[i] + "_boots"]).defense;
            if (remainingPoints >= price)
            {
                SetArmour(3, armour[i] + "_boots");
                remainingPoints -= price;
                break;
            }
        }
    }
    
    /// <summary>
    /// Sets up the enemies movement, attack and secondary abilities.
    /// </summary>
    protected virtual void SetupAbilities()
    {
        return;
    }

    /// <summary>
    /// Handles the processing of an enemies turn. (Enemy AI)
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator ProcessTurn()
    {
        // get the player object
        Character player = Game.character;

        IsCurrentlyProcessingTurn = true;

        // the way that our AI will work is the following:
        // - the first thing that the AI will do is see if their secondary ability is avaliable.
        // - if the ability is avaliable, we will check what the target of that ability is and then attempt to path towards it.
        // - if the ability is NOT avaliable, we will path towards the player and attempt to hit it with our regular attack.
        // this will form the basis of our AI.

        // whether we will use the secondary ability.
        bool useSecondary = false;
        // whether or not we can attempt to attack as well.
        bool attemptAttack = false;
        // whether or not the player has moved.
        bool hasMoved = false;

        // first we check the secondary ability exists, if not we can skip this bit
        if (secondaryAbility != null)
        {
            // if it does exist we check if it is on cooldown.
            // we also need to check if there are any objects in the scene that the ability can be used on.
            if (!secondaryAbility.isCooldown || secondaryAbility.GetTarget() != null)
            {
                // if not we know that we can use this ability
                useSecondary = true;
            }
        }

        // the next step is finding a suitable entity to use the ability on.
        if (useSecondary)
        {
            // if we are able to use the secondary ability we will!
            Entity target = secondaryAbility.GetTarget();

            if (target != null)
            {
                // if we aren't close enough to the target we need to get closer.
                // to test this we solve the path from ourself to the other entity.
                int distance = Environment.instance.Solve(target.currentPosition, currentPosition).Count - 1;
                if (distance > secondaryAbility.range)
                {
                    // set the last used ability
                    lastUsedAbility = moveAbility;
                    // if we are too far away, move closer to the target
                    yield return moveAbility.Use(true, target.currentPosition);
                    hasMoved = true;

                    // after we finish the move we can test our distance again
                    distance = Environment.instance.Solve(target.currentPosition, currentPosition).Count - 1;
                    if (distance <= secondaryAbility.range)
                    {
                        // set the last used ability
                        lastUsedAbility = secondaryAbility;
                        // in this case if we are close enough we can use the ability
                        yield return secondaryAbility.Use(true, target.currentPosition);
                    }
                    else
                    {
                        // if not we can attempt to use the first ability.
                        attemptAttack = true;
                    }
                }
                else
                {
                    // set the last used ability
                    lastUsedAbility = secondaryAbility;
                    // if we are close enough we can just use the ability
                    yield return secondaryAbility.Use(true, target.currentPosition);
                }
            }
            else
            {
                // if we cant find someone to heal, we can attempt to attack instead
                attemptAttack = true;
            }
        }

        // if we can't use the secondary ability we can attempt to attack
        if (useSecondary == false || attemptAttack == true)
        {
            // we need to first get the target
            Entity target = attackAbility.GetTarget();

            // and make sure we are close enough
            int distance = Environment.instance.Solve(target.currentPosition, currentPosition).Count - 1;
            if (distance > attackAbility.range)
            {
                // if we aren't in range and we haven't already moved
                if (!hasMoved)
                {
                    distance = Environment.instance.Solve(target.currentPosition, currentPosition).Count - 1;

                    if (avoidPlayer)
                    {
                        if (distance > attackAbility.range)
                        {
                            yield return FleePlayer();
                            hasMoved = true;
                        }
                    }
                    else
                    {
                        // set the last used ability
                        lastUsedAbility = moveAbility;
                        // if we are too far away, move closer to the target
                        yield return moveAbility.Use(true, target.currentPosition);
                        hasMoved = true;
                    }

                    // we then check the distance again
                    distance = Environment.instance.Solve(target.currentPosition, currentPosition).Count - 1;
                    if (distance <= attackAbility.range)
                    {
                        // set the last used ability
                        lastUsedAbility = attackAbility;
                        // if this time we are close enough we can attack
                        yield return attackAbility.Use(true, target.currentPosition);
                        // flee after attacking
                        if (!hasMoved)
                        {
                            yield return FleePlayer();
                            hasMoved = true;
                        }
                    }
                }
            }
            else
            {
                // set the last used ability
                lastUsedAbility = attackAbility;
                // if we are close enough we can attack
                yield return attackAbility.Use(true, target.currentPosition);
            }
        }

        IsCurrentlyProcessingTurn = false;
    }

    private IEnumerator FleePlayer ()
    {
        EnvironmentTile furthest = null;
        int curdist = 100;

        // run away by looping through the range and finding the furthest away spot from the player
        for (int i = -moveAbility.range; i < moveAbility.range + 1; i++)
        {
            for (int j = -moveAbility.range; j < moveAbility.range + 1; j++)
            {
                // if the tile is within range and exits
                EnvironmentTile tile = Environment.instance.GetTile(currentPosition.GridPosition.x + i, currentPosition.GridPosition.y + j);
                if (tile != null)
                {
                    if (tile.State == EnvironmentTile.TileState.None)
                    {
                        List<EnvironmentTile> path = Environment.instance.Solve(tile, currentPosition);
                        if (path != null)
                        {
                            // gets the distance between ourselves and the tile
                            int testDistance = path.Count - 1;
                            if (testDistance > moveAbility.range && testDistance < curdist)
                            {
                                // if this distance is the highest, set it to the 'furthest'
                                curdist = testDistance;
                                furthest = tile;
                            }
                        }
                    }
                }
            }
        }

        // set the last used ability
        lastUsedAbility = moveAbility;
        // once we have the furthest tile move to it
        yield return moveAbility.Use(true, furthest);
    }
    
    /// <summary>
    /// Handles the attack animation.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator Attack()
    {
        // make the enemy look at the player, and the player look at the enemy
        Game.character.transform.LookAt(currentPosition.Position);
        transform.LookAt(Game.character.currentPosition.Position);

        // we start the attack animation
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    /// <summary>
    /// Handles the animation event that is ran during the attack animation.
    /// </summary>
    public virtual void Hit()
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

        // damages the player.
        Game.character.Damage(d, weapon == "" ? attackElement : ((Weapon)Inventory.instance.allItems[weapon]).element);
    }

    /// <summary>
    /// Handles the aniamtion event that is ran during the walking animation.
    /// </summary>
    public void Footstep()
    {
        // this will play footsteps.
    }

    /// <summary>
    /// Heals the enemy.
    /// </summary>
    /// <param name="amount"></param>
    public override void Heal(int amount)
    {
        base.Heal(amount);

        // update the health text
        UpdateHealthText();
    }

    /// <summary>
    /// Damages the enemy.
    /// </summary>
    /// <param name="baseAmount"></param>
    public override void Damage(int baseAmount, DamageType type)
    {
        base.Damage(baseAmount, type);

        // update the health text
        UpdateHealthText();
    }

    /// <summary>
    /// Handles the death of this enemy.
    /// </summary>
    public override void Die()
    {
        base.Die();
        Destroy(healthText.gameObject);

        // handle the drop table
        // first add all of the 'definite' drops
        for (int i = 0; i < dropTable.definiteItems.Length; i++)
        {
            DropItem d = dropTable.definiteItems[i];
            Inventory.instance.PickupItem(d);
        }
        // and then add the random 'chance' drops
        DropItem[] items = dropTable.GetChanceItem();
        for (int i = 0; i < items.Length; i++)
        {
            Inventory.instance.PickupItem(items[i]);
        }

        // add the currency based on the random value
        Global.instance.AddCurrency(dropTable.coins.GetRandom());

        // remove this enemy from the enemy manager
        EnemyManager.instance.enemies.Remove(this);
    }

    /// <summary>
    /// Updates the health UI above the enemy.
    /// </summary>
    protected void UpdateHealthText()
    {
        // update the health text
        if (healthText == null)
        {
            healthText = GetComponentInChildren<StatText>();
        }
        healthText.value = health;
        healthText.maxValue = maxHealth;
    }
}

