using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour {

    public Transform car;
    private Quaternion rotation;
    private float rotationDamping = 2f;//Speed of rotation
    private float damping = 5f;
    private float carAngle;
    private float carmeraAngle;
    private Vector3 velocity;
    private Vector3 currentPosition;

    void Start () {
        
	}
	
	void LateUpdate () {
        carmeraAngle = transform.eulerAngles.y;
        carmeraAngle = Mathf.LerpAngle(carmeraAngle, carAngle, rotationDamping * Time.deltaTime);//Smooth rotate the camera
        rotation = Quaternion.Euler(0, carmeraAngle, 0);//Calculate the rotation
        if (!car.GetComponent<CarControl>().gasFlag)
        {
            currentPosition.y = Mathf.Lerp(currentPosition.y, 2, damping * Time.deltaTime);
            currentPosition.z = Mathf.Lerp(currentPosition.z, -5, damping * Time.deltaTime);
        }
        else
        {
            currentPosition.y = Mathf.Lerp(currentPosition.y, 3, damping * Time.deltaTime);
            currentPosition.z = Mathf.Lerp(currentPosition.z, -7, damping * Time.deltaTime);
        }
        transform.position = car.position + rotation * currentPosition;
        transform.LookAt(car);//Camera look at the car
	}

    void FixedUpdate()
    {
        velocity = car.InverseTransformDirection(car.GetComponent<Rigidbody>().velocity);//Transform velocity from world to local
        if (velocity.z < -2)
        {
            carAngle = car.eulerAngles.y + 180;
        }
        else
        {
            carAngle = car.eulerAngles.y;
        }
    }
}
