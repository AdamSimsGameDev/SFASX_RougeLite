using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Biome
{
    public string name;
    [Space]
    public List<EnvironmentTile> accessible = new List<EnvironmentTile>();
    public List<EnvironmentTile> inaccessible = new List<EnvironmentTile>();
    [Space]
    public EnemySpawnData[] enemies;
    [Space]
    [Range(0.0F, 1.0F)] public float accessiblePercentage;
} 

[System.Serializable]
public class EnemySpawnData
{
    public Enemy enemy;
    public int value;
    public int maxValue;
}