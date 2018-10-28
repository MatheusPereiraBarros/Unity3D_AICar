using UnityEngine;
using System.Collections;

public class FindPath : MonoBehaviour {

    public static Transform[] targets;
    public float[,] distanceMatrix;
    private GameObject[] attaches;

    void Awake()
    {
        //Initiallize targets and distance between targets
        targets = new Transform[transform.childCount];
        distanceMatrix = new float[transform.childCount, transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                if (i == j)
                {
                    distanceMatrix[i, j] = 0;
                }
                else
                {
                    distanceMatrix[i, j] = Mathf.Infinity;//Infinite distance means cannot pass
                }
            }
        }
        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = transform.GetChild(i);
            targets[i].GetComponent<TargetNum>().num = i;
        }
        
    }
    
    void SetDistance()
    {
        //Set distance between targets
        attaches = GameObject.FindGameObjectsWithTag("attach");
        for(int i = 0; i < attaches.Length; i++)
        {
            DistanceTwoTargets thisAttach = attaches[i].GetComponent<DistanceTwoTargets>();
            if (thisAttach.attachedTargets[0] != -1 && thisAttach.attachedTargets[1] != -1)
            {
                if (thisAttach.twoDire)
                {
                    distanceMatrix[thisAttach.attachedTargets[0], thisAttach.attachedTargets[1]] = thisAttach.distance;
                    distanceMatrix[thisAttach.attachedTargets[1], thisAttach.attachedTargets[0]] = thisAttach.distance;
                }
                else
                {
                    distanceMatrix[thisAttach.attachedTargets[0], thisAttach.attachedTargets[1]] = thisAttach.distance;
                    distanceMatrix[thisAttach.attachedTargets[1], thisAttach.attachedTargets[0]] = Mathf.Infinity;
                }
            }
        }
    }

    public int[] Dijkstra(int start)
    {
        //Set distance matrix
        SetDistance();
        int elements = transform.childCount;
        ArrayList determined = new ArrayList(elements);//Save the determined index
        ArrayList unDetermined = new ArrayList(elements);//Save the unDetermined index
        float[] distance = new float[elements];//Save the min distance
        int[] prePoint = new int[elements];//Save the previous shorstest point

        determined.Add(start);

        for (int i = 0; i < elements; i++)
        {
            if (i != start)
            {
                unDetermined.Add(i);
            }
        }

        for (int i = 0; i< elements; i++)
        {
            distance[i] = distanceMatrix[start, i];
            prePoint[i] = -1;
        }

        while (unDetermined.Count > 0)
        {
            int min_index = (int)unDetermined[0];
            foreach(int r in unDetermined)
            {
                if (distance[r] < distance[min_index])
                    min_index = r;
            }
            determined.Add(min_index);
            unDetermined.Remove(min_index);
            if(prePoint[min_index] == -1)
            {
                prePoint[min_index] = start;
            }
            foreach(int r in unDetermined)
            {
                if(distance[r] > distance[min_index] + distanceMatrix[min_index, r])
                {
                    distance[r] = distance[min_index] + distanceMatrix[min_index, r];
                    prePoint[r] = min_index;
                }
            }
        }
        return prePoint;
    }
}
