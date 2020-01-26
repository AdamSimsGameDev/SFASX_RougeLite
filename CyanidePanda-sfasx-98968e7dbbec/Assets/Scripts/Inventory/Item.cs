using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType { Normal, Fire, Water, Grass, Light, Dark }

public class Item
{
    // the name of the item
    public string name;
    // the key for this item in the dictionary
    public string key;
    // the item's description
    public string description;

    // the maximum stackSize of the item
    public int stackSize = 99;
    // the current stored amount of the item
    public int amount;

    // the amount this item is bought for
    public int value;

    // the function that is ran when the item is used in the inventory.
    public virtual void Use()
    {
        return;
    }

    public Item (int value, string name, string description)
    {
        this.name = name;
        this.description = description;
        this.value = value;
    }

    /// <summary>
    /// Sets the stored amount of this item.
    /// </summary>
    /// <param name="amount"></param>
    public void SetAmount(int amount)
    {
        this.amount = amount;
    }
    /// <summary>
    /// Increments the stored amount by a value.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int IncrementAmount(int value)
    {
        int a = amount + value;
        if (a > stackSize)
        {
            amount = stackSize;
            return a - stackSize;
        }
        else
        {
            amount = a;
            return 0;
        }
    }
}

public class Consumable : Item
{
    public int healthRegen;
    public int manaRegen;

    public Consumable(int value, string name, string description, int healthRegen, int manaRegen) : base(value, name, description)
    {
        this.healthRegen = healthRegen;
        this.manaRegen = manaRegen;
    }

    public override void Use()
    {
        if (Global.instance.IsPlaying == false)
            return;

        Game.character.Heal(healthRegen);
        Game.character.ManaUp(manaRegen);
        amount--;

        InventoryUI.instance.UpdateInventory();
    }
}

public class Weapon : Item
{
    public int range;
    public int damage;

    public DamageType element;

    public string resourcePath;

    public bool isEquipped
    {
        get
        {
            return key == Global.instance.weapon;
        }
    }

    public Weapon(int value, string name, string description, DamageType element, int range, int damage, string resourcePath) : base(value, name, description)
    {
        this.range = range;
        this.damage = damage;

        this.element = element;

        this.resourcePath = resourcePath;
    }

    public override void Use()
    {
        if (isEquipped)
        {
            Global.instance.SetWeapon("");
        }
        else
        {
            Global.instance.SetWeapon(key);
        }
        InventoryUI.instance.UpdateInventory();
    }
}

public class Armour : Item
{
    public enum ArmourType { Helmet, Chestplate, Leggings, Boots }

    public ArmourType type;
    public DamageType element;
    public int defense;
    public string resourcePath;

    public bool isEquipped
    {
        get
        {
            return key == Global.instance.armour[(int)type];
        }
    }

    public Armour(int value, string name, string description, ArmourType type, DamageType element, int defense, string resourcePath) : base(value, name, description)
    {
        this.type = type;
        this.element = element;

        this.defense = defense;
        this.resourcePath = resourcePath;
    }

    public override void Use()
    {
        if (isEquipped)
        {
            Global.instance.SetArmour((int)type, "");
        }
        else
        {
            Global.instance.SetArmour((int)type, key);
        }
        InventoryUI.instance.UpdateInventory();
    }
}
