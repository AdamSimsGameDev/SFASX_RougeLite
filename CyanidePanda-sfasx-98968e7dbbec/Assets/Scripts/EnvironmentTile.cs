using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentTile : MonoBehaviour
{
    public enum TileState { None, Obstacle, Player, Enemy }

    public List<EnvironmentTile> Connections { get; set; }
    public List<EnvironmentTile> Corners { get; set; }
    public EnvironmentTile Parent { get; set; }
    public Vector2Int GridPosition { get; set; }
    public Vector3 Position { get; set; }
    public float Global { get; set; }
    public float Local { get; set; }
    public bool Visited { get; set; }
    public TileState State { get; set; }
    public GameObject Occupier { get; set; }
}
