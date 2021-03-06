﻿using System.Collections;
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
    }
}
