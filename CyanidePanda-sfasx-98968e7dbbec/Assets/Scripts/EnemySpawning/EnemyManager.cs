using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;
    public List<Enemy> enemies = new List<Enemy>();

    private void Awake()
    {
        instance = this;
    }
    public void Initialize(Biome biome)
    {
        // work out all of the enemies we want to spawn
        int points = 4 + (Global.instance.selectedLevel * 4);
        int remaining = points;

        // we can now distribute these points however we want
        // first we get the list of enemies
        List<EnemySpawnData> enemyList = new List<EnemySpawnData>();
        for (int i = 0; i < biome.enemies.Length; i++)
        {
            enemyList.Add(biome.enemies[i]);
        }

        // sort the list
        enemyList.Sort((p1, p2) => p1.value.CompareTo(p2.value));
        enemyList.Reverse();

        // the method for spawning will be simple, we want to spawn the most expensive enemies first
        // however we don't want to spend the entire amount on one enemy, as that would be silly.
        // instead we want at least two enemies on the board at once.
        // this means we can never spawn an enemy with a value that is more than half of the points.
        List<Enemy> enemiesToSpawn = new List<Enemy>();

        // half point
        int halfPoint = Mathf.FloorToInt(points / 2.0F);

        // whilst we have points remaining
        while (remaining != 0)
        {
            // check to see if we can afford anything
            int purchasable = -1;
            for (int i = 0; i < enemyList.Count; i++)
            {
                // we check that it is affordable AND it is cheaper than half of the point requirement
                if (enemyList[i].value <= remaining && enemyList[i].value <= halfPoint)
                {
                    purchasable = i;
                    break;
                }
            }

            // if we can't purchase anything just break out
            if (purchasable == -1)
                break;

            // we then remove the points
            remaining -= enemyList[purchasable].value;
            // and add the enemy to the list
            enemiesToSpawn.Add(enemyList[purchasable].enemy);
        }

        // we can then go about spawning the enemies
        Vector3 playerPos = Game.character.transform.position;
        playerPos.y = 0.0F;

        List<EnvironmentTile> tiles = Environment.instance.GetAllTilesOfType(EnvironmentTile.TileState.None);
        for (int i = 0; i < tiles.Count;)
        {
            EnvironmentTile e = tiles[i];

            Vector3 tilePos = e.Position;
            tilePos.y = 0.0F;

            float distance = Vector3.Distance(tilePos, playerPos);
            if (distance <= 20.0F)
            {
                tiles.Remove(e);
                continue;
            }
            i++;
        }

        for (int i = 0; i < enemiesToSpawn.Count; i++)
        {
            CreateEnemy(ref tiles, enemiesToSpawn[i].gameObject);
        }
    }

    private void CreateEnemy(ref List<EnvironmentTile> possiblePositions, GameObject enemyObject)
    {
        bool positionFound = false;
        EnvironmentTile position = null;

        do
        {
            int r = Random.Range(0, possiblePositions.Count);

            position = possiblePositions[r];
            possiblePositions.RemoveAt(r);

            positionFound = true;
        }
        while (positionFound == false);

        GameObject go = Instantiate(enemyObject);
        go.transform.position = position.Position;

        Enemy en = go.GetComponent<Enemy>();
        en.currentPosition = position;

        position.Occupier = go;
        position.State = EnvironmentTile.TileState.Enemy;

        enemies.Add(en);
    }

    public IEnumerator ProcessTurn ()
    {
        for (int i = 0; i < enemies.Count;)
        {
            if (enemies[i].IsDead)
            {
                enemies.RemoveAt(i);
                continue;
            }
            i++;
        }

        foreach(Enemy e in enemies)
        {
            yield return e.ProcessTurn();
        }

        CameraControls.instance.attachedTarget = null;

        Debug.Log("AI Turns Processed");
    }
}
