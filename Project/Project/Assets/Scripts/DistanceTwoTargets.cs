using UnityEngine;
using System.Collections;

public class DistanceTwoTargets : MonoBehaviour {

    public float distance;
    public int[] attachedTargets = new int[2];
    public Transform[] obj = new Transform[2];
    public bool twoDire = false;

    void Start()
    {
        distance = transform.localScale.z;
        attachedTargets[0] = -1;
        attachedTargets[1] = -1;
        obj[0] = transform.GetChild(0);
        obj[1] = transform.GetChild(1);
        if (obj[0].CompareTag("pass") && obj[1].CompareTag("pass")) {
            twoDire = true;
        }
    }

    void Update()
    {
        attachedTargets[0] = obj[0].GetComponent<PassDetect>().targetNum;
        attachedTargets[1] = obj[1].GetComponent<PassDetect>().targetNum;
    }
}
