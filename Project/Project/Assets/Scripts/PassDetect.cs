using UnityEngine;
using System.Collections;

public class PassDetect : MonoBehaviour {

    public int targetNum;

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("target")) {
            targetNum = other.GetComponent<TargetNum>().num;
        }
    }
}
