using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTree
{
    private int maxObjectsInNode;
    private List<GameObject> objects;
    private Rec bounds;
    private QuadTree[] children;

    public QuadTree(int maxObjectsInNode, Rec bounds)
    {
        this.maxObjectsInNode = maxObjectsInNode;
        objects = new List<GameObject>();
        this.bounds = bounds;
        children = new QuadTree[4];
        Debug.DrawLine(new Vector3(bounds.x, 0, bounds.y), new Vector3(bounds.x, 0, bounds.y + bounds.height));
        Debug.DrawLine(new Vector3(bounds.x, 0, bounds.y), new Vector3(bounds.x + bounds.width, 0, bounds.y));
        Debug.DrawLine(new Vector3(bounds.x + bounds.width, 0, bounds.y), new Vector3(bounds.x + bounds.width, 0, bounds.y + bounds.height));
        Debug.DrawLine(new Vector3(bounds.x, 0, bounds.y + bounds.height), new Vector3(bounds.x + bounds.width, 0, bounds.y + bounds.height));
    }

    public void delete()
    {
        objects.Clear();

        for (int i = 0; i < 4; i++)
        {
            if (children[i] != null)
            {
                children[i].delete();
                children[i] = null;
            }
        }
    }

    public void split()
    {
        float x = bounds.x;
        float y = bounds.y;
        float width = bounds.width / 2f;
        float height = bounds.height / 2f;

        children[0] = new QuadTree(maxObjectsInNode, new Rec(x, y, width, height));
        children[1] = new QuadTree(maxObjectsInNode, new Rec(x + width, y, width, height));
        children[2] = new QuadTree(maxObjectsInNode, new Rec(x, y + height, width, height));
        children[3] = new QuadTree(maxObjectsInNode, new Rec(x + width, y + height, width, height));
    }

    public int getIndex(Rec bounds)
    {
        int index = -1;
        float verticalMid = this.bounds.x + (this.bounds.width / 2);
        float horizontalMid = this.bounds.y + (this.bounds.height / 2);
        
        bool topQ = (bounds.y < horizontalMid && bounds.y + bounds.height < horizontalMid);
        bool bottomQ = (bounds.y > horizontalMid);

        if(bounds.x < verticalMid && bounds.x + bounds.width < verticalMid)
        {
            if(topQ)
            {
                index = 0;
            }
            else if(bottomQ)
            {
                index = 2;
            }
        }
        else if(bounds.x > verticalMid)
        {
            if(topQ)
            {
                index = 1;
            }
            else if(bottomQ)
            {
                index = 3;
            }
        }
        return index;
    }

    public void insert(GameObject obj)
    {
        Rec bounds = getBounds(obj);

        if(children[0] != null)
        {
            int index = getIndex(bounds);

            if(index != -1)
            {
                children[index].insert(obj);

                return;
            }
        }

        objects.Add(obj);

        if (objects.Count > maxObjectsInNode)
        {
            if (children[0] == null)
            {
                split();
            }

            int i = 0;
            while (i < objects.Count)
            {
                int index = getIndex(getBounds(objects[i]));
                if(index != -1)
                {
                    children[index].insert(objects[i]);
                    objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }

    public QuadTree[] getChildren()
    {
        return children;
    }

    public List<GameObject> get(List<GameObject> objects, Rec bounds)
    {
        return retrieve(objects, bounds);
    }

    public List<GameObject> retrieve(List<GameObject> objects, Rec bounds)
    {
        int index = getIndex(bounds);

        if(index != -1 && children[0] != null)
        {
            children[index].retrieve(objects, bounds);
        }
        objects.AddRange(this.objects);
        return objects;
    }

    public static Rec getBounds(GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        Rec rect = new Rec(bounds.center.x, bounds.center.z, bounds.size.x, bounds.size.z);
        return rect;
    }
}

public class Rec
{
    public float x;
    public float y;
    public float width;
    public float height;

    public Rec(float x, float y, float width, float height)
    {
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
    }
}