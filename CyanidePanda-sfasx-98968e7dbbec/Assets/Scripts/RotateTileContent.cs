using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTileContent : MonoBehaviour
{
    [Range(1, 360)] public int increment = 90;

    private void Start()
    {
        int r = Random.Range(0, 360 / increment);
        transform.Rotate(transform.up, increment * r);
    }

}
