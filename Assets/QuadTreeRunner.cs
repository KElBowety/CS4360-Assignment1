using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadTreeRunner : MonoBehaviour
{
    private QuadTree tree;
    [SerializeField] int maxObjectsInNode;
    [SerializeField] GameObject[] objects;

    void Start()
    {
        tree = new QuadTree(maxObjectsInNode, new Rec(-100, -100, 200, 200));
    }

    // Update is called once per frame
    void Update()
    {
        tree.delete();
        
        for(int i = 0; i < objects.Length; i++)
        {
            tree.insert(objects[i]);
        }
        
        List<GameObject> possibleObjects = new List<GameObject>();

        for(int i = 0; i < objects.Length; i++)
        {
            possibleObjects.Clear();
            possibleObjects = tree.get(possibleObjects, QuadTree.getBounds(objects[i]));

            if(possibleObjects.Count > 3)
            {
                objects[i].GetComponent<BoxCollider>().enabled = true;
            }
            else
            {
                objects[i].GetComponent<BoxCollider>().enabled = false;
            }
        }
    }
}
