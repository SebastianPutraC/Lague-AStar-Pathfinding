using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    //Player stuff is used to check for the node position in the map
    public Transform player;

    public LayerMask unwalkableMask;

    //Size of the world
    public Vector2 gridWorldSize;

    //Radius that create distance between other nodes
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX;
    int gridSizeY; // Y is pretty much Z 

    //Showing path visual on the map
    public List<Node> path;

    private void Start()
    {
        //Nodes area
        nodeDiameter = nodeRadius * 2;

        //How many nodes can be fit in the world by x and y range
        //Diameter is 0.5 (0.25 *2) so there's 60 (0 - 59) 
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        //Transform.position = 0, 0, 0 (Middle of the world).  
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 //Left Edge of the world
           - Vector3.forward * gridWorldSize.y / 2; //Bottom left of the world

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                //From the worldBottomLeft, go right and up at increment of the x and y value. 
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) 
                    + Vector3.forward * (y * nodeDiameter + nodeRadius);

                //Check for unwalkable obstacle
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                //The world point is now registered on the grid, added the x and y so that the node know which node on grid
                grid[x, y] = new Node(walkable, worldPoint, x, y);
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        //Search by 3x3 area around the center node (As parameter)
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) //this is the center node, dont return this
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                //Grid is inside the already set grid area
                if (checkX >= 0 && checkX < gridSizeX
                    && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    //Calculating where the exact node based on the world position
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        //float percentX = (worldPosition.x + gridWorldSize.x / 2) / gridWorldSize.x;
        //float percentY = (worldPosition.z + gridWorldSize.y / 2) / gridWorldSize.y;
        //This might fix an issue if the map is not at 0,0,0
        float percentX = (worldPosition.x - transform.position.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPosition.z - transform.position.z + gridWorldSize.y / 2) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        //Debug.Log(" percent X : " + percentX + " percent Y : " + percentY );
        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        //Debug.Log("X : " + x + " Y : " + y);
        return grid[x, y];
    }

    private void OnDrawGizmos()
    {
        //Visualizing the gizmos for adjusting to fit the world size
        //Use y on Vector3 z value
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            Node playerNode = NodeFromWorldPoint(player.position);
            foreach(Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (playerNode == n)
                {
                    Gizmos.color = Color.cyan;
                }
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                //The 0.1f is for gaps between the cubes
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }
}
