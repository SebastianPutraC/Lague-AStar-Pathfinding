using System;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currenItemCount;

    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }
    public void Add(T item)
    {
        item.HeapIndex = currenItemCount;
        items[currenItemCount] = item;
        SortUp(item);
        currenItemCount++;
    }

    //Pop and get the highest item on the tree
    public T RemoveFirst()
    {
        T firstItem = items[0];
        currenItemCount--;
        items[0] = items[currenItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return firstItem;
    }

    //Just call the private SortUpItem public for visibility
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    //Not build in function like list or array
    public int Count
    {
        get
        {
            return currenItemCount;
        }
    }

    //Check for higher f cost (lower priority)
    private void SortDown(T item)
    {
        while (true)
        {
            //get left child from a parent node = 2n + 1
            int childIndexLeft = item.HeapIndex * 2 + 1;

            //get right child from a parent node = 2n + 2
            int childIndexRight = item.HeapIndex * 2 + 2;

            int swapIndex = 0;
            if (childIndexLeft < currenItemCount)
            {
                //default, the child will be on the left
                swapIndex = childIndexLeft;

                //Check if this child could be on the right
                if (childIndexRight < currenItemCount)
                {
                    // < 0 means lower priority (meaning higher f cost than childIndexRight)
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                // After checking left or right, put it there if Higher F cost
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                }
                else 
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    //Check for lower f cost (higher priority)
    private void SortUp(T item)
    {
        //get parent of a tree node = (n - 1) /2
        int parentIndex = (item.HeapIndex - 1) / 2;

        //Keep repeating this until 
        while (true)
        {
            T parentItem = items[parentIndex];
            // > 0 means that item is greater priority (meaning lower f cost than parent)
            if (item.CompareTo(parentItem) > 0)
            {
                Swap(item, parentItem);
            }
            else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int tempIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tempIndex;
    }


    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    //Testing less garbage for heap (Contains and Clear)
    /*public bool Contains(T item)
    {
        if (item.HeapIndex < currenItemCount)
        {
            return Equals(items[item.HeapIndex], item);
        }
        else
        {
            return false;
        }
    }
    public void Clear()
    {
        currenItemCount = 0;
    }*/
}

//Icomparable is used to check if the class/object has a comparable value with each other
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
