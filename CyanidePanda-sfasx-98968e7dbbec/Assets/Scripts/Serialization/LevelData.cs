using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BiomeType { Meadow, Desert, Magma }

[System.Serializable]
public class LevelData
{
    // whether this level has been completed.
    public bool isCompleted;
    // the seed used to generate this level's map.
    public int seed;
    // what biome the level generates in.
    public BiomeType biome;

    public void Init()
    {
        seed = Random.Range(int.MinValue, int.MaxValue);
    }
    public void Init(int seed)
    {
        this.seed = seed;
    }
}