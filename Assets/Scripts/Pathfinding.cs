using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using UnityEngine.InputSystem;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker;
    public Transform target;

    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            FindPath(seeker.position, target.position);
        }
    }

    private void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        //Tools to compare which is faster, heap or list
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        // Setup
        //List<Node> openSet = new List<Node>();

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        //Get the lowest F cost
        while (openSet.Count > 0)
        {
            //For startNode with List
            /*Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost // lower fCost
                    || openSet[i].fCost == currentNode.fCost // if same, get lower hCost
                    && openSet[i].hCost < currentNode.hCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);*/

            //Using the Heap function
            Node currentNode = openSet.RemoveFirst();

            closedSet.Add(currentNode);

            //Reached the target
            if (currentNode == targetNode)
            {
                sw.Stop();
                UnityEngine.Debug.Log("Path Found : " + sw.ElapsedMilliseconds + " ms");

                //Retracing the path by checking on each nodes parent 
                RetracePath(startNode, targetNode);
                return;
            }

            foreach(Node neighbour in grid.GetNeighbours(currentNode))
            {
                //Check for obstacles or the node already on the closedSet so no need to add it
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                {
                    continue;
                }

                int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

                // Check if surrounding nodes gCost can be lowered or its not part of the openSet yet
                if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newMovementCostToNeighbour;
                    neighbour.hCost = GetDistance(neighbour, targetNode);

                    //This is used to retrace back so the unit can move here by knowing which most optimal node leads to which
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
    }

    private void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while(currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        grid.path = path;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int dstY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        else
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }
    }
}
