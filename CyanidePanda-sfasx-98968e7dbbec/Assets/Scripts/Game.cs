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

    public bool IsTurnRunning;
    public int CurrentTurn;

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
        if (!character.IsMoving)
        {
            Ray screenClick = MainCamera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(screenClick, mRaycastHits);
            if (hits > 0)
            {
                EnvironmentTile tile = mRaycastHits[0].transform.GetComponent<EnvironmentTile>();
                character.targetTile = tile;
            }
            else
            {
                character.targetTile = null;
            }

            if (character.targetTile == null)
            {
                character.targetTile = character.currentPosition;
            }

            if (Input.GetMouseButtonDown(0) && CurrentTurn >= 0)
            {
                character.UseCurrentAbility();
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
            }
            else
            {
                character.transform.position = Environment.instance.Start.Position;
                character.transform.rotation = Quaternion.identity;
                character.currentPosition = Environment.instance.Start;

                Environment.instance.Start.Occupier = character.gameObject;
                Environment.instance.Start.State = EnvironmentTile.TileState.Player;

                CurrentTurn = 0;
                character.Init();

                player.MoveToPosition(character.transform.position);
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

        yield return new WaitForSeconds(1.0F);

        IsTurnRunning = false;
        CurrentTurn++;

        character.EndTurn();
        ui.EndTurn();
    }
}
