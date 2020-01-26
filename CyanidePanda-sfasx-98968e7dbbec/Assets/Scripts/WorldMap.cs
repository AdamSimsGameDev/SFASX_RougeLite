using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    public GameObject[] mapElements;
    public GameObject connectionElement;

    public bool canMove;

    private int lastLevel = -1;
    private bool stickFix;
    private float canSelectLevelTimer;
    private float stickFixTimer;

    private void Update()
    {
        if (canMove)
        {
            canSelectLevelTimer -= Time.deltaTime;

            if (Input.GetAxis("Horizontal") > 0.0F || Input.GetKeyDown(KeyCode.D))
            {
                if (!stickFix)
                {
                    Global.instance.selectedLevel = Mathf.Clamp(Global.instance.selectedLevel + 1, 0, Global.instance.levels.Count - 1);
                    stickFix = true;
                }
            }
            else if (Input.GetAxis("Horizontal") < 0.0F || Input.GetKeyDown(KeyCode.A))
            {
                if (!stickFix)
                {
                    Global.instance.selectedLevel = Mathf.Clamp(Global.instance.selectedLevel - 1, 0, Global.instance.levels.Count - 1);
                    stickFix = true;
                }
            }
            else
            {
                stickFix = false;
            }

            if (Input.GetButtonDown("Use") && canSelectLevelTimer <= 0.0F)
            {
                Global.instance.LoadLevel(Global.instance.selectedLevel);
            }
        }
        else
        {
            canSelectLevelTimer = 0.05F;
        }

        if (Global.instance.selectedLevel != lastLevel)
        {
            CameraControls.MoveToPosition(new Vector3(3.0F * Global.instance.selectedLevel, 0.0F, -10.0F));
            lastLevel = Global.instance.selectedLevel;
        }

        if (Input.GetKeyDown(KeyCode.F))
            AddNewLevel();
    }

    public void SetCanMove(bool canMove)
    {
        this.canMove = canMove;
    }

    /// <summary>
    /// Creates a new level and adds it to the list.
    /// </summary>
    public void AddNewLevel()
    {
        LevelData level = new LevelData();
        level.Init();
        level.biome = Global.instance.GetBiome();

        Global.instance.biomeDifficulties[(int)level.biome] += 4;

        Global.instance.levels.Add(level);
    }
    /// <summary>
    /// Creates the graphics for a level at a set index.
    /// </summary>
    /// <param name="index"></param>
    public void CreateLevelGraphics(int index, bool preExisted)
    {
        LevelData level = Global.instance.levels[index];

        GameObject go = Instantiate(mapElements[(int)level.biome], new Vector3(3.0F * index, 10.0F, 0.0F), Quaternion.identity);
        go.name = level.seed.ToString();

        MapLevelObject mlo = go.GetComponent<MapLevelObject>();
        if (preExisted)
            mlo.dontAnimate = true;

        // create the connection
        if (index != 0)
        {
            // instantiate the object
            GameObject conn = Instantiate(connectionElement, new Vector3(3.0F * (index - 1), 0.0F, 0.0F), Quaternion.identity);
            // set the current object's connection
            go.GetComponent<MapLevelObject>().connection = conn.GetComponent<MapConnection>();
        }
    }

    /// <summary>
    /// Renders the levels from the Global class.
    /// </summary>
    public void CreateLevels (bool animate)
    {
        if (animate)
        {
            StartCoroutine(DoCreateLevels());
        }
        else
        {
            for (int i = 0; i < Global.instance.levels.Count; i++)
            {
                CreateLevelGraphics(i, true);
            }
        }
    }

    /// <summary>
    /// Renders the levels from the the Global class over time. 
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoCreateLevels()
    {
        for (int i = 0; i < Global.instance.levels.Count; i++)
        {
            CreateLevelGraphics(i, false);
            yield return new WaitForSeconds(0.25F);
        }
    }
}