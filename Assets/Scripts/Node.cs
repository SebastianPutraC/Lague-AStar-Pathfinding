using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    public int movementPenaly;

    public int gCost;
    public int hCost;
    public int fCost{
        get { return gCost + hCost; }
    }
    public Node parent;

    private int heapIndex;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penaly)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        this.movementPenaly = _penaly;
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if (compare == 0) // same f cost
        {
            compare = hCost.CompareTo(nodeToCompare.hCost); // tie breaker
        }

        //This want to return 1 if its lower instead of -1 so reverse it
        return -compare;
    }
}
