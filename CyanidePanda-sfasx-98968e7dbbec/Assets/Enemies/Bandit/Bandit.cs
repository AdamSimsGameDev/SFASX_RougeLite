using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : Enemy
{
    protected override void OnStart()
    {
        base.OnStart();

        int r = Random.Range(0, 4);
        switch (r)
        {
            case 0:
                SetArmour(0, "leather_helmet");
                break;
            case 1:
                SetArmour(1, "leather_chestplate");
                break;
            case 2:
                SetArmour(2, "leather_leggings");
                break;
            case 3:
                SetArmour(3, "leather_boots");
                break;
        }

        SetWeapon("test_stick");
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
