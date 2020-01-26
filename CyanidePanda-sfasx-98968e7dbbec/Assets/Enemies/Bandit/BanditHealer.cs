using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BanditHealer : Enemy
{
    public override void SetEquipment(int points)
    {
        // weapon
        Dictionary<string, int> weapon = new Dictionary<string, int>()
        {
            { "healing_staff", 0 },
        };

        // armour
        List<string> armour = new List<string>()
        {
            "iron",
            "hardened_leather",
            "leather",
        };

        SetEquipment(points, armour, weapon);
    }

    protected override void SetupAbilities()
    {
        // initialize this enemies move ability
        moveAbility = new AbilityMove();
        moveAbility.Init(this);

        // initialize this enemies attack ability
        attackAbility = new AbilityBasicAttack();
        attackAbility.Init(this);

        // initialize this enemies secondary ability
        secondaryAbility = new AbilityHeal();
        secondaryAbility.Init(this);
    }
}
