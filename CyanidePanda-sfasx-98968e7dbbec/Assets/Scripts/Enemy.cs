using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float SingleNodeMoveTime = 0.5f;

    public EnvironmentTile currentPosition { get; set; }

    public void Damage(float damage)
    {
        Debug.Log("Enemy took " + damage + " damage!");
    }
}
