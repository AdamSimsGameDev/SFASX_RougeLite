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
    [SerializeField] private Transform CharacterStart;

    public bool IsPlaying;
    public bool IsTurnRunning;
    public int CurrentTurn;

    public bool isFreeLooking;

    public CameraControls player;

    private RaycastHit[] mRaycastHits;
    
    private readonly int NumberOfRaycastHits = 1;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;

        ui = GetComponent<GameUI>();
    }
    private void Start()
    {
        mRaycastHits = new RaycastHit[NumberOfRaycastHits];
        character = Instantiate(Character, transform); 
        ShowMenu(true);
    }
    private void Update()
    {
        if (IsPlaying)
        {
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
                /*Ray screenClick = MainCamera.ScreenPointToRay(Input.mousePosition);
                int hits = Physics.RaycastNonAlloc(screenClick, mRaycastHits);
                if (hits > 0)
                {
                    EnvironmentTile tile = mRaycastHits[0].transform.GetComponent<EnvironmentTile>();
                    character.targetTile = tile;
                }
                else
                {
                    character.targetTile = null;
                }*/

                int x = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.x / 10.0F) + Environment.instance.Size.x / 2;
                int y = Mathf.RoundToInt(GameObject.Find("Cursor").transform.position.z / 10.0F) + Environment.instance.Size.y / 2;
                character.targetTile = Environment.instance.GetTile(x, y);

                if (character.targetTile == null)
                {
                    character.targetTile = character.currentPosition;
                }

                if (Input.GetButtonDown("Use") && CurrentTurn >= 0)
                {
                    character.UseCurrentAbility();
                }
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
        Environment.instance.GenerateWorld();
    }

    public void Exit()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }

    private void StartGame ()
    {
        character.transform.position = Environment.instance.StartTile.Position;
        character.transform.rotation = Quaternion.identity;
        character.currentPosition = Environment.instance.StartTile;

        Environment.instance.StartTile.Occupier = character.gameObject;
        Environment.instance.StartTile.State = EnvironmentTile.TileState.Player;

        CurrentTurn = 0;
        character.Init();

        CameraControls.MoveToPosition(character.transform.position);
        EnemyManager.instance.Initialize(2);

        IsPlaying = true;
    }

    public void SetPlayerAbility (string ability)
    {
        character.SetCurrentAbility(ability);
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
}
