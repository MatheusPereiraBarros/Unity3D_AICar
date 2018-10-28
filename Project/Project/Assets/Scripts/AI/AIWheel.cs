using UnityEngine;
using System.Collections;

public class AIWheel : MonoBehaviour {

    public WheelCollider thisWheelCollider;
    
	void Update () {
        Vector3 SteerWheel;
        SteerWheel = transform.localEulerAngles;
        SteerWheel.y = thisWheelCollider.steerAngle;
        SteerWheel.z = 0;
        transform.localEulerAngles = SteerWheel;
        transform.Rotate(Vector3.right * thisWheelCollider.rpm / 60 * 360 * Time.deltaTime);
	}
}
