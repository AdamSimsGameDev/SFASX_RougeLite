using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderVisualiser : MonoBehaviour
{
    public static BorderVisualiser instance;

    public GameObject[] piecePrefabs;
    public GameObject cornerPrefab;

    public Material material;
    public Material transparent;

    public Color color;

    private void Awake()
    {
        instance = this;
    }

    public static void Visualise(List<EnvironmentTile> tiles, Color color)
    {
        instance.color = color;
        instance.Visualise(tiles);
    }
    public void Visualise (List<EnvironmentTile> tiles)
    {
        material.color = color;
        transparent.color = new Color(color.r, color.g, color.b, 0.25F);

        // remove the old borders
        Transform[] children = GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == transform)
                continue;
            Destroy(children[i].gameObject);
        }

        // create the new ones
        for(int i = 0; i < tiles.Count; i++)
        {
            Vector3 position = tiles[i].transform.position + new Vector3(5.0F, 2.5F, 5.0F);

            // find out what DEC value it is
            int dec = 0;
            for (int j = 0; j < 4; j++)
            {
                if (tiles.Contains(tiles[i].Connections[j]))
                    dec += (int)Mathf.Pow(2, j);
            }

            // create the base object
            GameObject g = Instantiate(piecePrefabs[dec]);
            g.transform.position = position;
            g.transform.parent = transform;
            g.GetComponent<Renderer>().sharedMaterial = material;
            g.transform.Find("Transparent").GetComponent<Renderer>().sharedMaterial = transparent;

            // create any additional corners that may be needed
            for (int j = 0; j < 4; j++)
            {
                if (!tiles.Contains(tiles[i].Corners[j]))
                {
                    GameObject c = Instantiate(cornerPrefab);
                    c.transform.position = position;
                    c.transform.parent = transform;
                    c.transform.rotation = Quaternion.Euler(-90.0F, (180.0F + (90.0F * j)), 0.0F);
                    c.GetComponent<Renderer>().sharedMaterial = material;
                }
            }
        }
    }

    public static void Clear()
    {
        // remove the old borders
        Transform[] children = instance.GetComponentsInChildren<Transform>(true);
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == instance.transform)
                continue;
            Destroy(children[i].gameObject);
        }
    }
}
