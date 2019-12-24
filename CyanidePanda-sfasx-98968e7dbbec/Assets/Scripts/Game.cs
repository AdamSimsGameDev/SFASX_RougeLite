using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Game : MonoBehaviour
{
    public static Game instance;

    [SerializeField] private Camera MainCamera;
    [SerializeField] private Character Character;
    [SerializeField] private Canvas Menu;
    [SerializeField] private Canvas Hud;
    [SerializeField] private Transform CharacterStart;

    public bool IsTurnRunning;
    public int CurrentTurn;
    [Space]
    public StatBar healthBar;
    public StatBar staminaBar;

    private RaycastHit[] mRaycastHits;
    private Character mCharacter;

    private readonly int NumberOfRaycastHits = 1;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;
    }
    private void Start()
    {
        mRaycastHits = new RaycastHit[NumberOfRaycastHits];
        mCharacter = Instantiate(Character, transform); 
        ShowMenu(true);
    }

    private void Update()
    {
        healthBar.value = mCharacter.health;
        healthBar.maxValue = mCharacter.maxHealth;

        staminaBar.value = mCharacter.stamina;
        staminaBar.maxValue = mCharacter.maxStamina;

        if (!mCharacter.IsMoving)
        {
            Ray screenClick = MainCamera.ScreenPointToRay(Input.mousePosition);
            int hits = Physics.RaycastNonAlloc(screenClick, mRaycastHits);
            if (hits > 0)
            {
                EnvironmentTile tile = mRaycastHits[0].transform.GetComponent<EnvironmentTile>();
                mCharacter.targetTile = tile;
            }

            // Check to see if the player has clicked a tile and if they have, try to find a path to that 
            // tile. If we find a path then the character will move along it to the clicked tile. 
            if (Input.GetMouseButtonDown(0) &&mCharacter.possibleMoves.Contains(mCharacter.targetTile) && mCharacter.stamina > 0)
            {
                List<EnvironmentTile> route = Environment.instance.Solve(mCharacter.currentPosition, mCharacter.targetTile);
                mCharacter.GoTo(route);

                mCharacter.stamina--;
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
                mCharacter.transform.position = CharacterStart.position;
                mCharacter.transform.rotation = CharacterStart.rotation;
                Environment.instance.CleanUpWorld();
            }
            else
            {
                mCharacter.transform.position = Environment.instance.Start.Position;
                mCharacter.transform.rotation = Quaternion.identity;
                mCharacter.currentPosition = Environment.instance.Start;

                Environment.instance.Start.Occupier = mCharacter.gameObject;
                Environment.instance.Start.State = EnvironmentTile.TileState.Player;

                mCharacter.FindPossibleMoves();
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

    // turn system
    public void NextTurn()
    {
        StartCoroutine(DoNextTurn());
    }

    private IEnumerator DoNextTurn ()
    {
        IsTurnRunning = true;

        yield return new WaitForSeconds(1.0F);

        IsTurnRunning = false;
        CurrentTurn++;

        mCharacter.FindPossibleMoves();
    }
}
