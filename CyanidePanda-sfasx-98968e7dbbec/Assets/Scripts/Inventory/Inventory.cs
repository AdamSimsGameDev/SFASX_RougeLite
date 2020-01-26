using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    // a dictionary that stores all of the items in the game.
    // these items are the items that the player has, though they all start with an 'amount' of 0
    public Dictionary<string, Item> allItems = new Dictionary<string, Item>()
    {
        // leather
        { "leather_helmet", new Armour(
            value: 100,
            name: "Leather Helmet",
            description: "A simple tanned leather helmet.\nComes with goggles!",
            type: Armour.ArmourType.Helmet,
            element: DamageType.Normal,
            defense: 3,
            resourcePath: "Armour/Leather/LeatherHelmet"
            ) },
        { "leather_chestplate", new Armour(
            value: 150,
            name: "Leather Chestplate",
            description: "A simple tanned leather chestplate.",
            type: Armour.ArmourType.Chestplate,
            element: DamageType.Normal,
            defense: 5,
            resourcePath: "Armour/Leather/LeatherChestplate"
            ) },
        { "leather_leggings", new Armour(
            value: 120,
            name: "Leather Leggings",
            description: "Tanned leather leggings.",
            type: Armour.ArmourType.Leggings,
            element: DamageType.Normal,
            defense: 4,
            resourcePath: "Armour/Leather/LeatherPants"
            ) },
        { "leather_boots", new Armour(
            value: 80,
            name: "Leather Boots",
            description: "A set of tanned leather boots.",
            type: Armour.ArmourType.Boots,
            element: DamageType.Normal,
            defense: 2,
            resourcePath: "Armour/Leather/LeatherBoots"
            ) },
        // hardened leather
        { "hardened_leather_helmet", new Armour(
            value: 160,
            name: "Hardened Helmet",
            description: "A leather helmet hardened with iron.",
            type: Armour.ArmourType.Helmet,
            element: DamageType.Normal,
            defense: 5,
            resourcePath: "Armour/HardenedLeather/HardenedLeatherHelmet"
            ) },
        { "hardened_leather_chestplate", new Armour(
            value: 210,
            name: "Hardened Chestplate",
            description: "A leather chestplate hardened with iron.",
            type: Armour.ArmourType.Chestplate,
            element: DamageType.Normal,
            defense: 8,
            resourcePath: "Armour/HardenedLeather/HardenedLeatherChestplate"
            ) },
        { "hardened_leather_leggings", new Armour(
            value: 180,
            name: "Hardened Leggings",
            description: "Leather leggings hardened with iron.",
            type: Armour.ArmourType.Leggings,
            element: DamageType.Normal,
            defense: 6,
            resourcePath: "Armour/HardenedLeather/HardenedLeatherPants"
            ) },
        { "hardened_leather_boots", new Armour(
            value: 140,
            name: "Hardened Boots",
            description: "A set of leather boots hardened with iron.",
            type: Armour.ArmourType.Boots,
            element: DamageType.Normal,
            defense: 4,
            resourcePath: "Armour/HardenedLeather/HardenedLeatherBoots"
            ) },
        // iron
        { "iron_helmet", new Armour(
            value: 160,
            name: "Iron Helmet",
            description: "An iron helmet. Shiny.",
            type: Armour.ArmourType.Helmet,
            element: DamageType.Normal,
            defense: 8,
            resourcePath: "Armour/Iron/Helmet"
            ) },
        { "iron_chestplate", new Armour(
            value: 210,
            name: "Iron Chestplate",
            description: "An iron chestplate.",
            type: Armour.ArmourType.Chestplate,
            element: DamageType.Normal,
            defense: 11,
            resourcePath: "Armour/Iron/Chestplate"
            ) },
        { "iron_leggings", new Armour(
            value: 180,
            name: "Iron Leggings",
            description: "A pair of iron leggings, not flexible but durable.",
            type: Armour.ArmourType.Leggings,
            element: DamageType.Normal,
            defense: 9,
            resourcePath: "Armour/Iron/Pants"
            ) },
        { "iron_boots", new Armour(
            value: 140,
            name: "Iron Boots",
            description: "A set of iron boots, cold to the touch.",
            type: Armour.ArmourType.Boots,
            element: DamageType.Normal,
            defense: 6,
            resourcePath: "Armour/Iron/Boots"
            ) },

        // weapons
        { "shortsword_iron", new Weapon(
            value: 10,
            name: "Iron Shortsword",
            description: "A basic iron shortsword.",
            element: DamageType.Normal,
            range: 1,
            damage: 22,
            resourcePath: "Weapons/Shortsword_Iron"
            ) },
        { "dagger_iron", new Weapon(
            value: 10,
            name: "Iron Dagger",
            description: "As equally deadly as it is small.",
            element: DamageType.Normal,
            range: 1,
            damage: 16,
            resourcePath: "Weapons/Dagger_Iron"
            ) },
        { "stick_wooden", new Weapon(
            value: 5,
            name: "Stick",
            description: "A wooden stick.",
            element: DamageType.Normal,
            range: 1,
            damage: 10,
            resourcePath: "Weapons/Stick_Wooden"
            ) },

        // misc
        { "slimeball", new Item(
            value: 5,
            name: "Slimeball",
            description: "A gooey ball of slime."
            ) },

        // consumables
        { "small_health_potion", new Consumable(
            value: 30, 
            name: "Small Health Potion", 
            description: "", 
            healthRegen: 25, 
            manaRegen: 0) },
        { "medium_health_potion", new Consumable(
            value: 60,
            name: "Medium Health Potion",
            description: "",
            healthRegen: 50,
            manaRegen: 0) },
        { "large_health_potion", new Consumable(
            value: 120,
            name: "Large Health Potion",
            description: "",
            healthRegen: 100,
            manaRegen: 0) },

        { "beef_jerky", new Consumable(
            value: 15,
            name: "\"Beef\" Jerky", 
            description: "You're not quite sure it's beef but it tastes good?",
            healthRegen: 10,
            manaRegen: 0
            ) },
    };

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        foreach(KeyValuePair<string, Item> kvp in allItems)
        {
            kvp.Value.key = kvp.Key;
        }

        AddItem("small_health_potion", 4);
        AddItem("stick_wooden", 1);
    }

    /// <summary>
    /// Adds an item to the inventory if it doesn't exist, increments it if it does.
    /// </summary>
    /// <param name="tag"></param>
    /// <param name="amount"></param>
    /// <returns>The leftover items that couldn't be added.</returns>
    public int AddItem(string tag, int amount)
    {
        return allItems[tag].IncrementAmount(amount);
    }

    /// <summary>
    /// Attempts to pickup an item.
    /// </summary>
    /// <param name="item"></param>
    public void PickupItem(DropItem item)
    {
        int amount = item.amount.GetRandom();
        int r = AddItem(item.key, amount);
        Debug.Log(instance.allItems[item.key].amount);
        Game.ui.AddDropUI(item.key, amount - r);        
    }

    /// <summary>
    /// Clears the inventory.
    /// </summary>
    public void Clear()
    {
        foreach (KeyValuePair<string, Item> item in allItems)
            item.Value.SetAmount(0);
    }
}
