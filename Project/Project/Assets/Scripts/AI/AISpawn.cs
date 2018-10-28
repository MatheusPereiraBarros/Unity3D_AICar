using UnityEngine;
using System.Collections;

public class AISpawn : MonoBehaviour {

    public GameObject AICarPrefab;

    private Transform[] spawns;
    private int spawnIndex;
    public float spawnTime = 0.5f;
    private float i = 0f;

    void Start()
    {
        spawns = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            spawns[i] = transform.GetChild(i);
        }
    }

    void Update()
    {
        Spawn();
    }

    void Spawn()
    {
        if (i < spawnTime)
        {
            i += Time.deltaTime;
        }
        else
        {
            i = 0;
            spawnIndex = Random.Range(0, spawns.Length);
            if (spawns[spawnIndex].GetComponent<CheckSpawn>().state == -1)
            {
                i = 1;
            }
            else if (spawns[spawnIndex].GetComponent<CheckSpawn>().state == 0)
            {
                Instantiate(AICarPrefab, spawns[spawnIndex].position, spawns[spawnIndex].rotation);
                spawns[spawnIndex].GetComponent<CheckSpawn>().state = -1;
            }
        }
    }
}
