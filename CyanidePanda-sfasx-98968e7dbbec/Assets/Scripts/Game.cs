using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour
{
    public static Game instance;
    public static Character character;
    public static GameUI ui;

    [SerializeField] private Camera MainCamera;
    [SerializeField] private Character Character;
    [SerializeField] private Canvas Menu;
    [SerializeField] private Canvas Hud;
    [SerializeField] private Canvas WinScreen;
    [SerializeField] private Transform CharacterStart;

    public bool IsPlaying;
    public bool IsTurnRunning;
    public bool IsLevelEnded;
    public int CurrentTurn;

    public bool isFreeLooking;

    public CameraControls player;

    private RaycastHit[] mRaycastHits;
    
    private readonly int NumberOfRaycastHits = 1;

    private void Awake()
    {
        // seed the level with the given seed
        Random.InitState(Global.instance.currentLevel.seed);

        if (instance != null)
            Destroy(gameObject);
        instance = this;

        ui = GetComponent<GameUI>();
    }
    private void Start()
    {
        mRaycastHits = new RaycastHit[NumberOfRaycastHits];
        character = Instantiate(Character, transform); 
        ShowMenu(false);
    }
    private void Update()
    {
        if (IsPlaying && !IsLevelEnded)
        {
            if (EnemyManager.instance.enemies.Count == 0)
            {
                // COMPLETE THE LEVEL :)
                StartCoroutine(EndLevel());
            }

            if (!IsTurnRunning)
            {
                if (Input.GetButtonDown("Look"))
                {
                    isFreeLooking = !isFreeLooking;

                    if (isFreeLooking)
                    {
                        ui.SetCurrentMenu("");
                    }
                    else
                    {
                        ui.ReturnToPreviousMenu();
                    }
                }
            }

            if (!character.IsMoving)
            {
                int x = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.x / 10.0F) + Environment.instance.Size.x / 2;
                int y = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.z / 10.0F) + Environment.instance.Size.y / 2;
                character.targetTile = Environment.instance.GetTile(x, y);

                if (character.targetTile == null)
                {
                    character.targetTile = character.currentPosition;
                }

                if (Input.GetButtonDown("Use") && CurrentTurn >= 0 && character.IsProcessingAbility == false)
                {
                    character.UseAbility(false, character.currentAbility, character.targetTile);
                }
            }
        }

        if (IsLevelEnded)
        {
            if (Input.GetButtonDown("Use"))
            {
                Global.instance.BackToWorldMap(true);
            }
        }
    }

    public void ShowMenu(bool show)
    {
        if (Menu != null && Hud != null)
        {
            Menu.enabled = show;
            Hud.enabled = !show;

            if( show )
            {
                character.transform.position = CharacterStart.position;
                character.transform.rotation = CharacterStart.rotation;
                Environment.instance.CleanUpWorld();
                CurrentTurn = -1;

                IsPlaying = false;
            }
            else
            {
                StartGame();
            }
        }
    }

    public void Generate()
    {
        // create the correct environment from the biome
        Environment.instance.GenerateWorld(Global.instance.currentLevel.biome);
    }

    public void Exit()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

    private void StartGame ()
    {
        Generate();

        character.transform.position = Environment.instance.StartTile.Position;
        character.transform.rotation = Quaternion.identity;
        character.currentPosition = Environment.instance.StartTile;

        Environment.instance.StartTile.Occupier = character.gameObject;
        Environment.instance.StartTile.State = EnvironmentTile.TileState.Player;

        CameraControls.MoveToPosition(character.transform.position);
        EnemyManager.instance.Initialize(Environment.instance.biomes[(int)Global.instance.currentLevel.biome]);

        for (int i = 0; i < 5; i++)
        {
            if (Global.instance.abilities[i] != "")
            {
                character.AddAbility(Global.instance.abilities[i]);
            }
        }

        character.SetWeapon(Global.instance.weapon);
        for (int i = 0; i < 4; i++)
        {
            character.SetArmour(i, Global.instance.armour[i]);
        }

        ui.StartGame();

        IsPlaying = true;

        CurrentTurn = 0;

        character.Init();
        ui.SetCurrentMenu("actionsMenu");
    }

    public void SetPlayerAbility (string ability)
    {
        character.SetCurrentAbility(ability);
    }
    public void SetPlayerHoveredAbility(string ability)
    {
        if (!IsPlaying)
            return;
        character.SetHoveredAbility(ability);
    }

    // turn system
    public void EndTurn()
    {
        StartCoroutine(ProcessTurn());
    }
    private IEnumerator ProcessTurn ()
    {
        IsTurnRunning = true;

        yield return EnemyManager.instance.ProcessTurn();

        CameraControls.MoveToPosition(character.currentPosition.Position);

        IsTurnRunning = false;
        CurrentTurn++;

        character.EndTurn();
        ui.EndTurn();
    }

    // end the level
    private IEnumerator EndLevel ()
    {
        IsLevelEnded = true;

        foreach (KeyValuePair<string, Ability> ab in Ability.abilities)
            ab.Value.currentCooldown = 0;

        float timer = 0.0F;
        do
        {
            timer = Mathf.Clamp01(timer + Time.deltaTime);

            Hud.GetComponent<CanvasGroup>().alpha = 1.0F - timer;
            WinScreen.GetComponent<CanvasGroup>().alpha = timer;

            yield return null;
        }
        while (timer != 1.0F);
    }
}
