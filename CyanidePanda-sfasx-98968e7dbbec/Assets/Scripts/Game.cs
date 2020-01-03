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

            if (Input.GetMouseButtonDown(0) && CurrentTurn >= 0)
            {
                mCharacter.UseCurrentAbility();
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
                CurrentTurn = -1;
            }
            else
            {
                mCharacter.transform.position = Environment.instance.Start.Position;
                mCharacter.transform.rotation = Quaternion.identity;
                mCharacter.currentPosition = Environment.instance.Start;

                Environment.instance.Start.Occupier = mCharacter.gameObject;
                Environment.instance.Start.State = EnvironmentTile.TileState.Player;

                CurrentTurn = 0;
                mCharacter.Init();
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
    }
}
