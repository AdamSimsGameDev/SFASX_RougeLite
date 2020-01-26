using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : Enemy
{
    protected override void SetupAbilities()
    {
        return;
    }

    public override IEnumerator ProcessTurn()
    {
        IsCurrentlyProcessingTurn = true;

        // the dummy will do nothing on it's turn, but we'll wait 1.0F seconds anyway.
        yield return new WaitForSeconds(1.0F);

        IsCurrentlyProcessingTurn = false;
    }
}
