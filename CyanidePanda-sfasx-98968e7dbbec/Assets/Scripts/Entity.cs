using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{   
    public int health;
    public int maxHealth;
    [Space]
    public int mana;
    public int maxMana;

    private Transform overheadCanvas;

    [Header("Armour & Weapons")]
    // the humanoid entity is similar to the actual entity, except this has options for armour and weapons
    // an array of length 4 to store the tag for each piece of armour (helmet, chestplate, leggings and boots).
    public string[] armour = new string[4];
    // the tag for the weapon the entity has equipped.
    public string weapon;

    // the entity's root bone, used for armour and such
    private Transform rootBone;

    // the current graphics for this entity's armour.
    private GameObject[] armourGraphics = new GameObject[4];
    // the current graphics for this entity's weapon.
    private GameObject weaponGraphics;
    // the last armour that the entity was wearing in each slot
    private string[] lastArmour = new string[4];
    // the last weapon that the entity had equipped
    private string lastWeapon;

    // skinned mesh renderer
    private SkinnedMeshRenderer baseMesh;

    [Header("Attack Stats")]
    // the amount of damage this entity does
    public int attackDamage;
    // the range in which this entity can attack
    public int attackRange;
    // the damage element
    public DamageType attackElement;
    // the defense element
    public DamageType defenseElement;

    // the characters current position
    public EnvironmentTile currentPosition { get; set; }

    // whether the player is currently moving or not.
    public bool IsMoving { get; set; }
    public bool IsDead { get; set; }

    // whether an ability is being processed
    [HideInInspector] public bool IsProcessingAbility;
    // a dictionary of this player's abilities
    public Dictionary<string, Ability> abilities = new Dictionary<string, Ability>();

    // this character's animator.
    [HideInInspector] public Animator animator;

    // the last ability that was used
    protected Ability lastUsedAbility;

    private void Awake()
    {
        OnAwake();
    }
    private void Start()
    {
        OnStart();
    }
    private void Update()
    {
        OnUpdate();
    }

    // a set of virtual functions to allow child classes to also call Update, Start and Awake
    protected virtual void OnAwake()
    {
        animator = GetComponent<Animator>();

        rootBone = transform.Find("Armature").Find("root");
        baseMesh = GetComponentInChildren<SkinnedMeshRenderer>();

        if (transform.GetComponentInChildren<Canvas>())
            overheadCanvas = transform.GetComponentInChildren<Canvas>().transform;
    }
    protected virtual void OnStart()
    {

    }
    protected virtual void OnUpdate()
    {

    }

    public virtual void UseAbility(bool isAI, string ability, EnvironmentTile targetTile)
    {
        if (IsDead)
            return;
        if (ability == "")
            return;

        if (mana >= abilities[ability].manaCost && !abilities[ability].isCooldown)
        {
            lastUsedAbility = abilities[ability];
            StartCoroutine(abilities[ability].Use(isAI, targetTile));
        }
    }
    public virtual void FinishUsingAbility()
    {
        // we can now stop processing the ability.
        IsProcessingAbility = false;
    }

    /// <summary>
    /// Heals the entity by a set amount.
    /// </summary>
    /// <param name="amount"></param>
    public virtual void Heal(int amount)
    {
        health += amount;
        if (health > maxHealth)
            health = maxHealth;

        // create the damage indicator
        CreateDamageIndicator(-health);
    }
    /// <summary>
    /// Increases the mana of the entity by a set amount.
    /// </summary>
    /// <param name="amount"></param>
    public virtual void ManaUp(int amount)
    {
        mana += amount;
        if (mana > maxMana)
            mana = maxMana;
    }

    /// <summary>
    /// Damages the enemy.
    /// </summary>
    /// <param name="baseAmount"></param>
    public virtual void Damage(int baseAmount, DamageType type)
    {
        // calculate the total defense of the equipped armour
        int totalDefense = 0;
        for (int i = 0; i < 4; i++)
        {
            if (armour[i] == "")
                continue;

            // get the armour
            Armour a = ((Armour)Inventory.instance.allItems[armour[i]]);

            // get the modifier
            float modifier = Entity.GetDamageModifier(a.element, type);

            // the total defense is incremented based on the armours defense and modifier
            totalDefense += Mathf.CeilToInt(a.defense / modifier);
        }
        // calculate the damage done when taking armour into account
        // we want there to be a minimum value however, which we can consider to be 20% of the original damage
        int damage = Mathf.Clamp(baseAmount - totalDefense, 0, int.MaxValue);

        // remove the health
        health -= damage;
        // handle death
        if (health <= 0)
            Die();

        // create the damage indicator
        CreateDamageIndicator(damage);
    }

    /// <summary>
    /// Creates the damage indicator in the health UI.
    /// </summary>
    /// <param name="amount"></param>
    protected void CreateDamageIndicator(int amount)
    {
        GameObject di = (GameObject)Resources.Load("UI/DamageIndicator");

        GameObject go = Instantiate(di);
        go.transform.SetParent(overheadCanvas, false);
        go.GetComponent<RectTransform>().anchoredPosition = new Vector3(0.0F, 40.0F, 0.0F);

        go.GetComponent<DamageIndicator>().SetValue(amount * -1);
    }

    /// <summary>
    /// Handles the death of the entity.
    /// </summary>
    public virtual void Die()
    {
        // when an entity dies we want to remove it from the environment grid.
        Environment.instance.GetTile(currentPosition.GridPosition.x, currentPosition.GridPosition.y).State = EnvironmentTile.TileState.None;

        // we also want to tell the entity that it is dead.
        IsDead = true;
        // and play the death animation.
        animator.SetTrigger("Die");
        // weapon graphics
        if (weaponGraphics != null) Destroy(weaponGraphics);
    }

    /// <summary>
    /// Makes the entity look at a given position.
    /// </summary>
    /// <param name="position"></param>
    public void LookAt(Vector3 position)
    {
        transform.rotation = Quaternion.LookRotation(position - transform.position, Vector3.up);
    }

    /// <summary>
    /// Sets the value of an armour slot to the tag provided.
    /// </summary>
    /// <param name="piece">The slot.</param>
    /// <param name="tag">The tag for the armour item.</param>
    public void SetArmour(int piece, string tag)
    {
        // update the last armour
        lastArmour[piece] = armour[piece];
        // set the value
        armour[piece] = tag;
        // update the armour visuals
        UpdateArmourVisuals();
    }

    /// <summary>
    /// Sets the current weapon to the tag.
    /// </summary>
    /// <param name="tag"></param>
    public void SetWeapon(string tag)
    {
        // update the last weapon
        lastWeapon = weapon;
        // set the value
        weapon = tag;
        // update the armour visuals
        UpdateWeaponVisuals();
    }

    /// <summary>
    /// Updates the graphics to reflect changes to the armour.
    /// </summary>
    private void UpdateArmourVisuals()
    {
        // loop through each armour piece
        for (int i = 0; i < 4; i++)
        {
            // check to see if the armour has changed
            if (armour[i] != lastArmour[i])
            {
                // if it has we want to delete the last armour graphics (if it existed)
                if (armourGraphics[i] != null)
                {
                    Destroy(armourGraphics[i]);
                    armourGraphics[i] = null;
                }

                // once we have deleted the last armour we need to create the new armour
                // so long as the new tag isnt ""
                if (armour[i] != "")
                {
                    // we first need to get the item from the inventory
                    Item item = Inventory.instance.allItems[armour[i]];
                    // we then want to make sure it is cast as an armour item
                    Armour a = (Armour)item;

                    // we load the resources from the armour's string
                    GameObject prefab = (GameObject)Resources.Load(a.resourcePath);
                    // once we have loaded it we can instantiate it
                    GameObject go = Instantiate(prefab);
                    // and set it as a child of our transform
                    go.transform.parent = transform;
                    // and set it's localPosition to Vector3.zero
                    go.transform.localPosition = Vector3.zero;

                    // we then need to setup its skinned mesh renderer's root and bounds properly
                    SkinnedMeshRenderer rend = go.GetComponent<SkinnedMeshRenderer>();
                    rend.rootBone = rootBone;
                    rend.localBounds = baseMesh.localBounds;
                    rend.bones = baseMesh.bones;

                    // we then update the armour graphics array with the new object
                    armourGraphics[i] = go;
                }
            }
        }
    }

    /// <summary>
    /// Updates the graphics to reflect changes to the weapon.
    /// </summary>
    private void UpdateWeaponVisuals()
    {
        // check to see if the armour has changed
        if (weapon != lastWeapon)
        {
            // if it has we want to delete the last armour graphics (if it existed)
            if (weaponGraphics != null)
            {
                Destroy(weaponGraphics);
                weaponGraphics = null;
            }

            // once we have deleted the last armour we need to create the new armour
            // so long as the new tag isnt ""
            if (weapon != "")
            {
                // we first need to get the item from the inventory
                Item item = Inventory.instance.allItems[weapon];
                // we then want to make sure it is cast as an armour item
                Weapon w = (Weapon)item;

                // we load the resources from the armour's string
                GameObject prefab = (GameObject)Resources.Load(w.resourcePath);
                // once we have loaded it we can instantiate it
                GameObject go = Instantiate(prefab);
                // and set it as a child of our transform
                go.transform.parent = transform;
                // and set it's localPosition to Vector3.zero
                go.transform.localPosition = Vector3.zero;

                // we then need to setup its skinned mesh renderer's root and bounds properly
                SkinnedMeshRenderer rend = go.GetComponent<SkinnedMeshRenderer>();
                rend.rootBone = rootBone;
                rend.localBounds = baseMesh.localBounds;
                rend.bones = baseMesh.bones;

                // we then update the armour graphics array with the new object
                weaponGraphics = go;
            }
        }
    }

    /// <summary>
    /// Returns a damage modifier based on attack and defense types.
    /// </summary>
    /// <param name="armour"></param>
    /// <param name="weapon"></param>
    /// <returns></returns>
    public static float GetDamageModifier(DamageType armour, DamageType weapon)
    {
        // we then check the armour element and compare it to the weapon element
        switch (armour)
        {
            case DamageType.Normal:
                return 1.0F;
            case DamageType.Light:
                if (weapon == DamageType.Dark)
                    return 2.0F;
                else if (weapon == DamageType.Light)
                    return 0.5F;
                else
                    return 1.0F;
            case DamageType.Dark:
                if (weapon == DamageType.Light)
                    return 2.0F;
                else if (weapon == DamageType.Dark)
                    return 0.5F;
                else
                    return 1.0F;
            case DamageType.Fire:
                if (weapon == DamageType.Grass)
                    return 2.0F;
                else if (weapon == DamageType.Fire || weapon == DamageType.Water)
                    return 0.5F;
                else
                    return 1.0F;
            case DamageType.Water:
                if (weapon == DamageType.Fire)
                    return 2.0F;
                else if (weapon == DamageType.Water || weapon == DamageType.Grass)
                    return 0.5F;
                else
                    return 1.0F;
            case DamageType.Grass:
                if (weapon == DamageType.Water)
                    return 2.0F;
                else if (weapon == DamageType.Grass || weapon == DamageType.Fire)
                    return 0.5F;
                else
                    return 1.0F;
        }
        return 1.0F;
    }
}
