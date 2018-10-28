using UnityEngine;
using System.Collections;

public class CheckSpawn : MonoBehaviour {

    public int state = 0;

    void OnTriggerExit(Collider other)
    {
        if(true)
        {
            state = 0;
        }
    }
}
