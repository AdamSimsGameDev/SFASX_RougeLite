using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public static Environment instance;

    [SerializeField] private List<EnvironmentTile> AccessibleTiles;
    [SerializeField] private List<EnvironmentTile> InaccessibleTiles;
    [SerializeField] private float AccessiblePercentage;

    public Vector2Int Size;

    private EnvironmentTile[][] mMap;
    private List<EnvironmentTile> mAll;
    private List<EnvironmentTile> mToBeTested;
    private List<EnvironmentTile> mLastSolution;

    private readonly Vector3 NodeSize = Vector3.one * 9.0f; 
    private const float TileSize = 10.0f;
    private const float TileHeight = 2.5f;

    public EnvironmentTile StartTile { get; private set; }

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        instance = this;

        mAll = new List<EnvironmentTile>();
        mToBeTested = new List<EnvironmentTile>();
    }

    private void OnDrawGizmos()
    {
        // Draw the environment nodes and connections if we have them
        if (mMap != null)
        {
            for (int x = 0; x < Size.x; ++x)
            {
                for (int y = 0; y < Size.y; ++y)
                {
                    if (mMap[x][y].Connections != null)
                    {
                        for (int n = 0; n < mMap[x][y].Connections.Count; ++n)
                        {
                            Gizmos.color = Color.blue;
                            if (mMap[x][y].Connections[n] != null)
                            {
                                Gizmos.DrawLine(mMap[x][y].Position, mMap[x][y].Connections[n].Position);
                            }
                        }
                    }

                    // Use different colours to represent the state of the nodes
                    Color c = Color.white;
                    if (mMap[x][y].State != EnvironmentTile.TileState.None)
                    {
                        switch(mMap[x][y].State)
                        {
                            case EnvironmentTile.TileState.Player:
                                c = Color.cyan;
                                break;
                            case EnvironmentTile.TileState.Enemy:
                                c = Color.red;
                                break;
                            case EnvironmentTile.TileState.Obstacle:
                                c = Color.black;
                                break;
                            default:
                                c = Color.white;
                                break;
                        }
                    }
                    else
                    {
                        if(mLastSolution != null && mLastSolution.Contains( mMap[x][y] ))
                        {
                            c = Color.green;
                        }
                        else if (mMap[x][y].Visited)
                        {
                            c = Color.yellow;
                        }
                    }

                    Gizmos.color = c;
                    Gizmos.DrawWireCube(mMap[x][y].Position, NodeSize);
                }
            }
        }
    }

    public EnvironmentTile GetTile (int x, int y)
    {
        if (x < 0 || x > Size.x - 1 || y < 0 || y > Size.y - 1)
            return null;
        return mMap[x][y];
    }

    private void Generate()
    {
        // Setup the map of the environment tiles according to the specified width and height
        // Generate tiles from the list of accessible and inaccessible prefabs using a random
        // and the specified accessible percentage
        mMap = new EnvironmentTile[Size.x][];

        int halfWidth = Size.x / 2;
        int halfHeight = Size.y / 2;
        Vector3 position = new Vector3( -(halfWidth * TileSize) - (Size.x % 2 == 1 ? TileSize / 2 : 0.0F), 0.0f, -(halfHeight * TileSize) - (Size.y % 2 == 1 ? TileSize / 2 : 0.0F));
        bool start = false;

        for ( int x = 0; x < Size.x; ++x)
        {
            mMap[x] = new EnvironmentTile[Size.y];

            for ( int y = 0; y < Size.y; ++y)
            {
                start = halfWidth == x && halfHeight == y;

                bool isAccessible = start || Random.value < AccessiblePercentage;
                List<EnvironmentTile> tiles = isAccessible ? AccessibleTiles : InaccessibleTiles;
                EnvironmentTile prefab = tiles[Random.Range(0, tiles.Count)];
                EnvironmentTile tile = Instantiate(prefab, position, Quaternion.identity, transform);
                tile.Position = new Vector3( position.x + (TileSize / 2), TileHeight, position.z + (TileSize / 2));
                tile.GridPosition = new Vector2Int(x, y);
                tile.State = isAccessible ? EnvironmentTile.TileState.None : EnvironmentTile.TileState.Obstacle;
                tile.gameObject.name = string.Format("Tile({0},{1})", x, y);
                mMap[x][y] = tile;
                mAll.Add(tile);

                if(start)
                {
                    StartTile = tile;
                }

                position.z += TileSize;
                start = false;
            }

            position.x += TileSize;
            position.z = -(halfHeight * TileSize) - (Size.y % 2 == 1 ? TileSize / 2 : 0.0F);
        }
    }
    private void SetupConnections()
    {
        // Currently we are only setting up connections between adjacnt nodes
        for (int x = 0; x < Size.x; ++x)
        {
            for (int y = 0; y < Size.y; ++y)
            {
                EnvironmentTile tile = mMap[x][y];
                tile.Connections = new List<EnvironmentTile>(4);
                tile.Corners = new List<EnvironmentTile>(4);

                // create 4 null tiles as placeholders
                for (int i = 0; i < 4; i++)
                {
                    tile.Connections.Add(null);
                    tile.Corners.Add(null);
                }

                if (x > 0)
                {
                    // left
                    tile.Connections[3] = (mMap[x - 1][y]);
                }
                if (x < Size.x - 1)
                {
                    // right
                    tile.Connections[1] = (mMap[x + 1][y]);
                }

                if (y > 0)
                {
                    // down
                    tile.Connections[2] = (mMap[x][y - 1]);
                }
                if (y < Size.y - 1)
                {
                    // up
                    tile.Connections[0] = (mMap[x][y + 1]);
                }


                if (y > 0 && x < Size.x - 1)
                {
                    // bottom right
                    tile.Corners[2] = (mMap[x + 1][y - 1]);
                }
                if (y < Size.y - 1 && x < Size.x - 1)
                {
                    // top right
                    tile.Corners[1] = (mMap[x + 1][y + 1]);
                }
                if (y > 0 && x > 0)
                {
                    // bottom left
                    tile.Corners[3] = (mMap[x - 1][y - 1]);
                }
                if (y < Size.y - 1 && x > 0)
                {
                    // top left
                    tile.Corners[0] = (mMap[x - 1][y + 1]);
                }
            }
        }
    }
    private float Distance(EnvironmentTile a, EnvironmentTile b)
    {
        // Use the length of the connection between these two nodes to find the distance, this 
        // is used to calculate the local goal during the search for a path to a location
        float result = float.MaxValue;
        EnvironmentTile directConnection = a.Connections.Find(c => c == b);
        if (directConnection != null)
        {
            result = TileSize;
        }
        return result;
    }
    private float Heuristic(EnvironmentTile a, EnvironmentTile b)
    {
        // Use the locations of the node to estimate how close they are by line of sight
        // experiment here with better ways of estimating the distance. This is used  to
        // calculate the global goal and work out the best order to prossess nodes in
        return Vector3.Distance(a.Position, b.Position);
    }
    public void GenerateWorld()
    {
        Generate();
        SetupConnections();
    }
    public void CleanUpWorld()
    {
        if (mMap != null)
        {
            for (int x = 0; x < Size.x; ++x)
            {
                for (int y = 0; y < Size.y; ++y)
                {
                    Destroy(mMap[x][y].gameObject);
                }
            }
        }
    }

    public List<EnvironmentTile> GetAllTilesOfType(EnvironmentTile.TileState state)
    {
        List<EnvironmentTile> tiles = new List<EnvironmentTile>();

        foreach(EnvironmentTile t in mAll)
        {
            if (t.State == state)
            {
                tiles.Add(t);
            }
        }

        return tiles;
    }

    public List<EnvironmentTile> Solve(EnvironmentTile begin, EnvironmentTile destination)
    {
        List<EnvironmentTile> result = null;
        if (begin != null && destination != null)
        {
            // Nothing to solve if there is a direct connection between these two locations
            EnvironmentTile directConnection = begin.Connections.Find(c => c == destination);
            if (directConnection == null)
            {
                // Set all the state to its starting values
                mToBeTested.Clear();

                for( int count = 0; count < mAll.Count; ++count )
                {
                    mAll[count].Parent = null;
                    mAll[count].Global = float.MaxValue;
                    mAll[count].Local = float.MaxValue;
                    mAll[count].Visited = false;
                }

                // Setup the start node to be zero away from start and estimate distance to target
                EnvironmentTile currentNode = begin;
                currentNode.Local = 0.0f;
                currentNode.Global = Heuristic(begin, destination);

                // Maintain a list of nodes to be tested and begin with the start node, keep going
                // as long as we still have nodes to test and we haven't reached the destination
                mToBeTested.Add(currentNode);

                while (mToBeTested.Count > 0 && currentNode != destination)
                {
                    // Begin by sorting the list each time by the heuristic
                    mToBeTested.Sort((a, b) => (int)(a.Global - b.Global));

                    // Remove any tiles that have already been visited
                    mToBeTested.RemoveAll(n => n.Visited);

                    // Check that we still have locations to visit
                    if (mToBeTested.Count > 0)
                    {
                        // Mark this note visited and then process it
                        currentNode = mToBeTested[0];
                        currentNode.Visited = true;

                        // Check each neighbour, if it is accessible and hasn't already been 
                        // processed then add it to the list to be tested 
                        for (int count = 0; count < currentNode.Connections.Count; ++count)
                        {
                            EnvironmentTile neighbour = currentNode.Connections[count];

                            if (neighbour == null)
                                continue;

                            if (!neighbour.Visited && neighbour.State == EnvironmentTile.TileState.None)
                            {
                                mToBeTested.Add(neighbour);
                            }

                            // Calculate the local goal of this location from our current location and 
                            // test if it is lower than the local goal it currently holds, if so then
                            // we can update it to be owned by the current node instead 
                            float possibleLocalGoal = currentNode.Local + Distance(currentNode, neighbour);
                            if (possibleLocalGoal < neighbour.Local)
                            {
                                neighbour.Parent = currentNode;
                                neighbour.Local = possibleLocalGoal;
                                neighbour.Global = neighbour.Local + Heuristic(neighbour, destination);
                            }
                        }
                    }
                }

                // Build path if we found one, by checking if the destination was visited, if so then 
                // we have a solution, trace it back through the parents and return the reverse route
                if (destination.Visited)
                {
                    result = new List<EnvironmentTile>();
                    EnvironmentTile routeNode = destination;

                    while (routeNode.Parent != null)
                    {
                        result.Add(routeNode);
                        routeNode = routeNode.Parent;
                    }
                    result.Add(routeNode);
                    result.Reverse();

                    // Debug.LogFormat("Path Found: {0} steps {1} long", result.Count, destination.Local);
                }
                else
                {
                    Debug.LogWarning("Path Not Found");
                }
            }
            else
            {
                result = new List<EnvironmentTile>();
                result.Add(begin);
                result.Add(destination);
                // Debug.LogFormat("Direct Connection: {0} <-> {1} {2} long", begin, destination, TileSize);
            }
        }
        else
        {
            if (begin == null && destination == null)
            {
                Debug.LogWarning("Cannot find path for invalid nodes");
            }
            else if (begin == null)
            {
                Debug.LogWarning("Cannot find path for invalid start node");
            }
            else if (destination == null)
            {
                Debug.LogWarning("Cannot find path for invalid destination node");
            }
        }

        mLastSolution = result;

        return result;
    }
}
