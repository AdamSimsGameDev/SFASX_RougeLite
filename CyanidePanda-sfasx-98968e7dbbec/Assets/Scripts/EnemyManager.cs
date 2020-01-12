using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    public List<Enemy> enemies = new List<Enemy>();
    public GameObject enemyObject;

    private void Awake()
    {
        instance = this;
    }
    public void Initialize(int enemyCount)
    {
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

        for (int i = 0; i < enemyCount; i++)
        {
            CreateEnemy(ref tiles);
        }
    }

    private void CreateEnemy(ref List<EnvironmentTile> possiblePositions)
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
}
