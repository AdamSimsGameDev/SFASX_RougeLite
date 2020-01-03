using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Ability
{
    public int currentCooldown;

    public abstract int maxCooldown { get; }
    public abstract int staminaCost { get; }

    public bool isCooldown { get { return currentCooldown != 0; } }

    protected List<EnvironmentTile> tiles = new List<EnvironmentTile>();
    protected Character player;

    public abstract void Init(Character player);
    public abstract bool Use(EnvironmentTile targetTile);
    public abstract void Visualise(EnvironmentTile targetTile);
    public abstract void ClearVisualisation();

    public virtual void EndTurn()
    {
        currentCooldown = Mathf.Clamp(currentCooldown - 1, 0, maxCooldown + 1);
    }
}
