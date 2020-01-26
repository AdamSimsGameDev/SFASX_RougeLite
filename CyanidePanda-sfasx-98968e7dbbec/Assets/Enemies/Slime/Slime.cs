using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy
{
    protected override void OnStart()
    {
        base.OnStart();
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
