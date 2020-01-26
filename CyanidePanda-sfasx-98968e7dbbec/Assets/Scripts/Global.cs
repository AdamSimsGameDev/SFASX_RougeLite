using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Global : MonoBehaviour
{
    public static Global instance;

    // the player's stored abilities
    public string[] abilities = new string[5];
    [Space]
    // the player's currently worn armour
    public string[] armour = new string[4];
    // the player's currently equipped weapon
    public string weapon;
    [Space]
    // whether the game is currently playing
    public bool IsPlaying;
    // whether the level has been completed successfully.
    public bool IsLevelCompleted;

    public List<LevelData> levels = new List<LevelData>();
    public LevelData currentLevel { get { return levels[levelIndex];} }

    // the current level's index.
    private int levelIndex = -1;
    // the current save slot.
    private int currentSaveSlot;

    // the load state of the worldmap scene.
    private AsyncOperation asyncLoad;
    // whether the worldmap scene is currently loading.
    private bool isLoading = false;

    // the current selected level
    [HideInInspector] public int selectedLevel;
    // the next level's biome
    private BiomeType nextBiome;
    // the amount of times a biome has been seen
    private int consecutiveBiomes;

    // the current amount of money the player has collected in this level
    public int currency = 0;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        StartCoroutine(DoLoadGame());
    }

    /// <summary>
    /// Get the next biome.
    /// </summary>
    /// <returns></returns>
    public BiomeType GetBiome()
    {
        // store the biome
        BiomeType b = nextBiome;

        // calculate the next biome, if a change is needed
        float r = Random.Range(0.0F, 1.0F);
        float newBiomeChance = Mathf.Clamp((consecutiveBiomes - 1) * 0.35F, 0.0F, 1.0F); // 0%, 0%, 35%, 70%, 100% (we always get a new biome after 5 in a row)
        if (r <= newBiomeChance)
        {
            // create a biome list, so that we can add all of the biomes EXCEPT our current biome
            List<BiomeType> biomeList = new List<BiomeType>();
            foreach(BiomeType biome in System.Enum.GetValues(typeof(BiomeType)))
            {
                if (biome == b)
                    continue;
                biomeList.Add(biome);
            }

            // get the new biome from the list randomly
            nextBiome = biomeList[Random.Range(0, biomeList.Count)];

            // reset the consecutive biomes
            consecutiveBiomes = 0;
        }
        else
        {
            // increase the new biome chance
            consecutiveBiomes++;
        }

        // return the stored biome
        return b;
    }

    /// <summary>
    /// Adds currency.
    /// </summary>
    /// <param name="amount">The amount to add</param>
    public void AddCurrency(int amount)
    {
        currency += amount;
    }

    /// <summary>
    /// Sets the value of an armour slot to the tag provided.
    /// </summary>
    /// <param name="piece">The slot.</param>
    /// <param name="tag">The tag for the armour item.</param>
    public void SetArmour(int piece, string tag)
    {
        // set the value
        armour[piece] = tag;
        // update the armour visuals
        if (IsPlaying)
        {
            Game.character.SetArmour(piece, tag);
        }
    }

    /// <summary>
    /// Sets the current weapon to the tag.
    /// </summary>
    /// <param name="tag"></param>
    public void SetWeapon(string tag)
    {
        // set the value
        weapon = tag;
        // update the armour visuals
        if (IsPlaying)
        {
            Game.character.SetWeapon(tag);
        }
    }

    /// <summary>
    /// Loads a level with a given index.
    /// </summary>
    /// <param name="index"></param>
    public void LoadLevel(int index)
    {
        SaveGame(currentSaveSlot);
        // set the current level index.
        levelIndex = index;
        // load the in-game scene.
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Returns to the world map.
    /// </summary>
    /// <param name="win"></param>
    public void BackToWorldMap(bool win)
    {
        if (win)
        {
            IsLevelCompleted = true;
            SceneManager.LoadScene(1);
        }
        else
        {
            IsLevelCompleted = false;
            SceneManager.LoadScene(1);
        }
        IsPlaying = false;
    }

    /// <summary>
    /// Returns to the main menu.
    /// </summary>
    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        switch (scene.buildIndex)
        {
            case 0: // menu
                break;
            case 1: // world map
                WorldMap map = FindObjectOfType<WorldMap>();
                if (levels.Count == 0)
                {
                    // create the first level
                    map.AddNewLevel();
                    map.CreateLevelGraphics(0, true);
                }
                else
                {
                    // otherwise render the levels
                    map.CreateLevels(false);

                    // teleport the camera
                    CameraControls.instance.transform.position = new Vector3(3.0F * selectedLevel, 0.0F, -10.0F);

                    // create a new level
                    if (IsLevelCompleted && !levels[levelIndex].isCompleted)
                    {
                        map.AddNewLevel();
                        map.CreateLevelGraphics(levels.Count - 1, false);

                        IsLevelCompleted = false;
                        levels[levelIndex].isCompleted = true;

                        SaveGame();
                    }
                }
                break;
            case 2: // in game
                // set is playing
                IsPlaying = true;
                break;
        }
    }

    /// <summary>
    /// Creates new game data for a given slot.
    /// </summary>
    /// <param name="slot"></param>
    public void CreateNewGame(int slot)
    {
        SaveGame(slot);
    }

    /// <summary>
    /// Saves the game data into the current data slot.
    /// </summary>
    public void SaveGame()
    {
        SaveGame(currentSaveSlot);
    }
    /// <summary>
    /// Saves the game data into a given data slot.
    /// </summary>
    /// <param name="slot"></param>
    public void SaveGame(int slot)
    {
        GameData data = new GameData();
        data.currency = currency;
        data.abilities = abilities;
        data.lastSelectedLevel = selectedLevel;
        data.nextBiome = (int)nextBiome;
        data.levels = levels.ToArray();
        data.consecutiveBiomes = consecutiveBiomes;

        data.weapon = weapon;
        data.armour = armour;

        List<InventoryData> inventory = new List<InventoryData>();
        foreach(KeyValuePair<string, Item> kvp in Inventory.instance.allItems)
        {
            Item item = kvp.Value;
            string key = kvp.Key;

            if (item.amount != 0)
            {
                inventory.Add(new InventoryData(key, item.amount));
            }
        }
        data.inventory = inventory;

        FileStream stream = File.OpenWrite(Application.persistentDataPath + "/slot" + slot.ToString() + ".save");
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, data);
        stream.Close();
    }

    /// <summary>
    /// Load the game data from a given slot.
    /// </summary>
    /// <param name="slot"></param>
    public void LoadGame(int slot)
    {
        if (!isLoading)
        {
            isLoading = true;
            FileStream stream = File.OpenRead(Application.persistentDataPath + "/slot" + slot.ToString() + ".save");
            BinaryFormatter formatter = new BinaryFormatter();
            GameData d = (GameData)formatter.Deserialize(stream);

            // load the data         
            currency = d.currency;
            // set the abilities
            abilities = d.abilities;
            // add each of the levels to the 'levels' list
            for (int i = 0; i < d.levels.Length; i++)
            {
                levels.Add(d.levels[i]);
            }
            // set the next biome
            nextBiome = (BiomeType)d.nextBiome;

            // set selected level
            selectedLevel = d.lastSelectedLevel;
            // set consecutive biomes
            consecutiveBiomes = d.consecutiveBiomes;

            // set the weapon and armour
            weapon = d.weapon;
            armour = d.armour;

            // get the inventory data
            Inventory.instance.Clear();
            foreach(InventoryData i in d.inventory)
            {
                Inventory.instance.allItems[i.tag].SetAmount(i.amount);
            }

            // load the next level.
            asyncLoad.allowSceneActivation = true;

            // close the stream
            stream.Close();

            // set the current save slot to the index
            currentSaveSlot = slot;
        }
    }

    /// <summary>
    /// Load the game scene
    /// </summary>
    private IEnumerator DoLoadGame()
    {
        asyncLoad = SceneManager.LoadSceneAsync(1);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}