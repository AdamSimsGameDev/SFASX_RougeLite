using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameData
{
    // this class will hold all of the data that needs to be saved and loaded
    // it is important that these variables are serializable, things like Vector3s wont work.

    // currency is the first value we need
    public int currency;
    // we also want to store what abilities the player currently has equipped
    public string[] abilities;

    // the last selected level
    public int lastSelectedLevel;
    // the last biome that was created
    public int nextBiome;
    // biome progress
    public int consecutiveBiomes;

    // weapon key
    public string weapon;
    // weapon armour
    public string[] armour = new string[4];

    // and the player's inventory.
    public List<InventoryData> inventory = new List<InventoryData>();

    // and finally level stuff
    public LevelData[] levels;
}