using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : Enemy
{
    public override void SetEquipment(int points)
    {
        base.OnStart();

        // weapon
        Dictionary<string, int> weapon = new Dictionary<string, int>()
        {
            { "stick_wooden", 2 },
            { "dagger_iron", 6 },
            { "shortsword_iron", 10 }
        };
        List<string> weaponTags = new List<string>();
        foreach(KeyValuePair<string, int> kvp in weapon)
        {
            weaponTags.Insert(0, kvp.Key);
        }

        // armour
        List<string> armour = new List<string>()
        {
            "iron",
            "hardened_leather",
            "leather",
        };

        // regular bandits will favour weapons over armour.
        int remainingPoints = points;
        
        // weapon
        for (int i = 0; i < weaponTags.Count; i++)
        {
            if (remainingPoints >= weapon[weaponTags[i]])
            {
                SetWeapon(weaponTags[i]);
                remainingPoints -= weapon[weaponTags[i]];
                break;
            }
        }

        // armour
        // chestplate first
        for (int i = 0; i < armour.Count; i++)
        {
            int price = ((Armour)Inventory.instance.allItems[armour[i] + "_chestplate"]).defense;
            if (remainingPoints >= weapon[weaponTags[i]])
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
            if (remainingPoints >= weapon[weaponTags[i]])
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
            if (remainingPoints >= weapon[weaponTags[i]])
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
            if (remainingPoints >= weapon[weaponTags[i]])
            {
                SetArmour(3, armour[i] + "_boots");
                remainingPoints -= price;
                break;
            }
        }
    }

    protected override void SetupAbilities()
    {
        // initialize this enemies move ability
        moveAbility = new AbilityMove();
        moveAbility.Init(this);

        // initialize this enemies attack ability
        attackAbility = new AbilityBasicAttack();
        attackAbility.Init(this);
    }
}
