using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathVisualiser : MonoBehaviour
{
    public GameObject[] prefabs;

    public Material accessible;
    public Material inaccessible;

    private Dictionary<EnvironmentTile, GameObject> pathTiles = new Dictionary<EnvironmentTile, GameObject>();

    // the character's current position
    public EnvironmentTile current { get; set; }

    public void DoUpdate (EnvironmentTile target, List<EnvironmentTile> possibleTiles)
    {
        // destroy the pre-existing path
        Transform[] children = GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == transform)
                continue;
            Destroy(children[i].gameObject);
        }

        // clear the previous tile dictionary
        pathTiles.Clear();

        // ensure that the destination and current position actually exist
        if (target == null || current == null)
            return;

        // get the path between the current position and the destination.
        List<EnvironmentTile> path = Environment.instance.Solve(current, target);

        // ensure that the path exists
        if (path == null)
            return;

        // we then loop through each tile in the path and work out what piece we need to create, and with what rotation.
        for (int i = 0; i < path.Count; i++)
        {
            EnvironmentTile e = path[i];
            Vector3 position = e.transform.position + new Vector3(5.0F, 2.5F, 5.0F);

            int lastDirection = -1;
            int nextDirection = -1;

            GameObject g = null;

            // find the last direction
            if (i != 0)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (path[i - 1] == e.Connections[j])
                        lastDirection = j;
                }
            }
            // find the next direction
            if (i != path.Count - 1)
            {
                for (int j = 0; j < 4; j++)
                {
                    if (path[i + 1] == e.Connections[j])
                        nextDirection = j;
                }
            }

            if (e == current)
            {
                // do nothing
            }
            else if (e == target)
            {
                int inverseDirection = -1;
                switch(lastDirection)
                {
                    case 0:
                        inverseDirection = 2;
                        break;
                    case 1:
                        inverseDirection = 3;
                        break;
                    case 2:
                        inverseDirection = 0;
                        break;
                    case 3:
                        inverseDirection = 1;
                        break;
                }

                // end of the path
                g = Instantiate(prefabs[possibleTiles.Contains(path[i]) ? 1 : 4]);
                g.transform.position = position;
                g.transform.parent = transform;
                g.transform.Rotate(Vector3.up * 90.0F * inverseDirection);
            }
            else
            {
                // anywhere inbetween
                if ((lastDirection == 0 && nextDirection == 2) || (lastDirection == 2 && nextDirection == 0))
                {
                    g = Instantiate(prefabs[2]);
                    g.transform.position = position;
                    g.transform.parent = transform;
                }
                else if ((lastDirection == 1 && nextDirection == 3) || (lastDirection == 3 && nextDirection == 1))
                {
                    g = Instantiate(prefabs[2]);
                    g.transform.position = position;
                    g.transform.parent = transform;
                    g.transform.Rotate(Vector3.up * 90.0F);
                }
                else
                {
                    int rotation = 0;
                    if ((lastDirection == 1 && nextDirection == 2) || (lastDirection == 2 && nextDirection == 1))
                        rotation = 1;
                    else if ((lastDirection == 2 && nextDirection == 3) || (lastDirection == 3 && nextDirection == 2))
                        rotation = 2;
                    else if ((lastDirection == 3 && nextDirection == 0) || (lastDirection == 0 && nextDirection == 3))
                        rotation = 3;

                    g = Instantiate(prefabs[3]);
                    g.transform.position = position;
                    g.transform.parent = transform;
                    g.transform.Rotate(Vector3.up * 90.0F * rotation);
                }
            }

            if (g != null)
            {
                if (possibleTiles.Contains(path[i]))
                    g.GetComponent<Renderer>().sharedMaterial = accessible;
                else
                    g.GetComponent<Renderer>().sharedMaterial = inaccessible;
            }

            pathTiles.Add(path[i], g);
        }
    }

    public void RemoveAtTile(EnvironmentTile tile)
    {
        if (pathTiles.ContainsKey(tile))
        {
            Destroy(pathTiles[tile]);
            pathTiles.Remove(tile);
        }
    }

    public void Clear()
    {
        // destroy the pre-existing path
        Transform[] children = GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == transform)
                continue;
            Destroy(children[i].gameObject);
        }

        // clear the previous tile dictionary
        pathTiles.Clear();
    }

}
