using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapLevelObject : MonoBehaviour
{
    public GameObject[] graphics;
    public MapConnection connection;

    private Vector3 start;
    private Vector3 end;
    [HideInInspector] public float lerp = 0.0F;

    public bool dontAnimate;

    private void Start()
    {
        int r = Random.Range(0, graphics.Length);
        for (int i = 0; i < graphics.Length; i++)
        {
            graphics[i].SetActive(i == r);
        }

        start = transform.position;
        end = start - (Vector3.up * 10.0F);
    }
    private void Update()
    {
        if (dontAnimate == true)
        {
            transform.position = end;
            if (connection != null) connection.ForceCreate();
            return;
        }

        lerp = Mathf.Clamp01(lerp + Time.deltaTime * 1.5F);
        transform.position = Vector3.Lerp(start, end, lerp);

        if (connection != null)
        {
            if (lerp == 1.0F && !connection.created)
            {
                connection.StartCreation();
            }
        }
    }
}
