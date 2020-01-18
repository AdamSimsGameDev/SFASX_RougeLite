using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : Enemy
{
    public override IEnumerator ProcessTurn()
    {
        // get the player object
        Character player = Game.character;

        IsCurrentlyProcessingTurn = true;

        int moveRange = 3;

        // the test enemy will move towards the player
        // this will be frightening to watch
        List<EnvironmentTile> route = Environment.instance.Solve(currentPosition, player.currentPosition);
        route.RemoveAt(route.Count - 1);

        List<EnvironmentTile> shortRoute = new List<EnvironmentTile>();

        if (route.Count <= moveRange)
        {
            shortRoute = route;
        }
        else
        {
            for (int i = 0; i < moveRange; i++)
            {
                shortRoute.Add(route[i]);
            }
        }

        yield return DoGoTo(shortRoute);

        IsCurrentlyProcessingTurn = false;
    }
}
