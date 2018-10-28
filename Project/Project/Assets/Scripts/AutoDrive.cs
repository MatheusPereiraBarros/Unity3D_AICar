using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AutoDrive : MonoBehaviour {

    public int startPoint = -1;
    public int endPoint = -1;
    public bool autoMode = false;
    public Vector3 destination;
    public GameObject finder;
    public Transform[] route;
    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRL;
    public WheelCollider wheelRR;
    public int currentTarget = 0;
    private float maxSteer = 40f;
    private float topSpeed = 20f;
    private float closeSpeed = 10f;
    private float limitSpeed;
    private float maxTorque = 800f;
    private float brakeTorque = 20000f;
    private float decelerationTorque = 25000f;
    private float reachTargetDis = 5f;
    private float closeTargetDis = 30f;
    private bool startFlag = true;
    private bool sensorFlag = false;
    public GameObject backLight;
    public Material brakeBackLight;
    public Material offBackLight;
    public Transform frontLeftSensor;
    public Transform frontRightSensor;
    public Transform middleSensor;
    public Transform bodyLeftSensor;
    public Transform bodyRightSensor;
    private float straightSensorLength = 8f;
    private float straightMiddleSensorLength = 8f;
    private float decelerationDis = 8f;
    private float stopDis = 6f;
    private float sideSensorLenght = 1f;
    public int[] prePoint;
    private float angledSensorLength = 8f;
    private float sensorAngle = 15f;
    public List<GameObject> objList;//Objects that point to this car
    public GameObject pointToObj;//The object that sensor point to

    private int[] engineGear = new int[5];

    void Start()
    {
        objList = new List<GameObject>();
        for (int i = 0; i < engineGear.Length; i++)
        {
            engineGear[i] = (int)((i + 1) * topSpeed / engineGear.Length) + 1;
        }
    }

    void Update()
    {
        if (autoMode)
        {
            if (startFlag)
            {
                GetDestination();
                GetStart();
                startFlag = false;
            }
            HighLightPath();
            if (!sensorFlag)
            {
                ApplyDrive();
                Brake();
            }
            Sensor();
            LightUpdate();
            EngineSound();
        }
    }

    void GetDestination()
    {
        endPoint = GetNearestTarget(destination);
    }

    int GetNearestTarget(Vector3 position)
    {
        int nearestPoint;
        float nearestDistance;
        nearestPoint = 0;
        nearestDistance = (FindPath.targets[nearestPoint].position - position).magnitude;
        for (int i = 0; i < FindPath.targets.Length; i++)
        {
           if(nearestDistance > (FindPath.targets[i].position - position).magnitude)
            {
                nearestPoint = i;
                nearestDistance = (FindPath.targets[i].position - position).magnitude;
            }
        }
        return nearestPoint;
    }

    void GetStart()
    {
        startPoint = GetNearestTarget(transform.position);
    }

    Transform[] GetPath()
    {
        int start = startPoint;
        int end = endPoint;
        int temp = end;
        int[] path;
        Transform[] pathTargets;
        int pathNodes = 0;
        prePoint = finder.GetComponent<FindPath>().Dijkstra(start);
        for(int i = 0; i < finder.transform.childCount; i++)
        {
            if (prePoint[temp] != -1)
            {
                temp = prePoint[temp];
            }
            else
            {
                pathNodes = i + 1;
                break;
            }
        }
        temp = end;
        path = new int[pathNodes];
        for(int i = pathNodes - 2; i >= 0; i--)
        {
            if (prePoint[temp] != -1)
            {
                temp = prePoint[temp];
                path[i] = temp;
            }
        }
        path[pathNodes - 1] = end;
        path[0] = start;

        pathTargets = new Transform[pathNodes];
        for(int i = 0; i < pathTargets.Length; i++)
        {
            pathTargets[i] = FindPath.targets[path[i]];
        }
        return pathTargets;
    }
    
    void HighLightPath()
    {
        if (autoMode)
        {
            for(int i = 0; i < FindPath.targets.Length; i++)
            {
                FindPath.targets[i].GetComponent<Renderer>().material.color = Color.white;
            }
            route = GetPath();
            for (int i = currentTarget; i < route.Length; i++)
            {
                route[i].GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    void ApplyDrive()
    {
        Vector3 distanceVector = transform.InverseTransformPoint(route[currentTarget].position.x, transform.position.y, route[currentTarget].position.z);
        float currentSteer = maxSteer * (distanceVector.x / distanceVector.magnitude);
        wheelFL.steerAngle = currentSteer;
        wheelFR.steerAngle = currentSteer;
        if (distanceVector.magnitude < reachTargetDis)
        {
            currentTarget++;
            if(currentTarget > route.Length - 1)
            {
                Reset();
            }
        }

        if(distanceVector.magnitude > closeTargetDis)
        {
            limitSpeed = topSpeed;
        }
        else
        {
            limitSpeed = closeSpeed;
        }

        if (GetComponent<Rigidbody>().velocity.magnitude < limitSpeed)
        {
            wheelFL.motorTorque = maxTorque;
            wheelFR.motorTorque = maxTorque;
            wheelRL.motorTorque = maxTorque;
            wheelRR.motorTorque = maxTorque;
        }
        else
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
            wheelRL.motorTorque = 0;
            wheelRR.motorTorque = 0;
        }
    }

    void Brake()
    {
        if (Input.GetAxis("Vertical") < 0 || Input.GetButton("Jump") || GetComponent<Rigidbody>().velocity.magnitude > limitSpeed)
        {
            wheelFL.motorTorque = 0;
            wheelFR.motorTorque = 0;
            wheelRL.motorTorque = 0;
            wheelRR.motorTorque = 0;
            wheelFL.brakeTorque = brakeTorque;
            wheelFR.brakeTorque = brakeTorque;
            wheelRL.brakeTorque = brakeTorque;
            wheelRR.brakeTorque = brakeTorque;
            if (Vector3.Angle(transform.forward, GetComponent<Rigidbody>().velocity) <= 80)
            {
                GetComponent<Rigidbody>().AddForce(-transform.forward * 10000);
            }
        }
        else
        {
            wheelFL.brakeTorque = 0;
            wheelFR.brakeTorque = 0;
            wheelRL.brakeTorque = 0;
            wheelRR.brakeTorque = 0;
        }
    }

    void LightUpdate()
    {
        if(wheelFL.brakeTorque > 0)
        {
            backLight.GetComponent<Renderer>().material = brakeBackLight;
        }
        else
        {
            backLight.GetComponent<Renderer>().material = offBackLight;
        }
    }

    void Sensor()
    {
        sensorFlag = false;
        RaycastHit hit;
        // Middle sensor
        if (Physics.Raycast(middleSensor.position, middleSensor.forward, out hit, straightMiddleSensorLength))
        {
            if (hit.transform.tag != "spawn" && hit.transform.tag != "ground" && hit.transform.tag != "road")
            {
                Debug.DrawLine(middleSensor.position, hit.point, Color.blue);
                if (hit.transform.tag == "AICar")
                {
                    pointToObj = hit.transform.gameObject;
                    hit.transform.GetComponent<AICar>().Add(gameObject);
                }
                else
                {
                    pointToObj = null;
                }
                sensorFlag = true;
                if ((hit.point - middleSensor.position).magnitude < decelerationDis && (hit.point - middleSensor.position).magnitude > stopDis)
                    Deceleration();
                else if ((hit.point - middleSensor.position).magnitude < stopDis)
                    Stop();
            }
        }
        else
        {
            pointToObj = null;
        }

        // Front left sensor straight
        if (Physics.Raycast(frontLeftSensor.position, frontLeftSensor.forward, out hit, straightSensorLength))
        {
            if (hit.transform.tag != "spawn" && hit.transform.tag != "ground" && hit.transform.tag != "road")
            {
                Debug.DrawLine(frontLeftSensor.position, hit.point, Color.blue);
                sensorFlag = true;
                if ((hit.point - frontLeftSensor.position).magnitude < decelerationDis && (hit.point - frontLeftSensor.position).magnitude > stopDis)
                    Deceleration();
                else if ((hit.point - frontLeftSensor.position).magnitude < stopDis)
                    Stop();
            }
        }

        // Front left sensor angled
        Quaternion rotationLeft = Quaternion.AngleAxis(sensorAngle, -frontLeftSensor.up);
        if (Physics.Raycast(frontLeftSensor.position, rotationLeft * frontLeftSensor.forward, out hit, angledSensorLength))
        {
            if (hit.transform.tag != "spawn" && hit.transform.tag != "ground" && hit.transform.tag != "road")
            {
                Debug.DrawLine(frontLeftSensor.position, hit.point, Color.blue);
                sensorFlag = true;
                Deceleration();
            }
        }

            // Front right sensor straight
            if (Physics.Raycast(frontRightSensor.position, frontRightSensor.forward, out hit, straightSensorLength))
        {
            if (hit.transform.tag != "spawn" && hit.transform.tag != "ground" && hit.transform.tag != "road")
            {
                Debug.DrawLine(frontRightSensor.position, hit.point, Color.blue);
                sensorFlag = true;
                if ((hit.point - frontRightSensor.position).magnitude < decelerationDis && (hit.point - frontRightSensor.position).magnitude > stopDis)
                    Deceleration();
                else if ((hit.point - frontRightSensor.position).magnitude < stopDis)
                    Stop();
            }
        }

        // Front right sensor angled
        Quaternion rotationRight = Quaternion.AngleAxis(sensorAngle, frontRightSensor.up);
        if (Physics.Raycast(frontRightSensor.position, rotationRight * frontRightSensor.forward, out hit, angledSensorLength))
        {
            if (hit.transform.tag != "spawn" && hit.transform.tag != "ground" && hit.transform.tag != "road")
            {
                Debug.DrawLine(frontRightSensor.position, hit.point, Color.blue);
                sensorFlag = true;
                Deceleration();
            }
        }

        // Body left sensor
        if (Physics.Raycast(bodyLeftSensor.position, -bodyLeftSensor.right, out hit, sideSensorLenght))
        {
            Debug.DrawLine(bodyLeftSensor.position, hit.point, Color.blue);
        }

        // Body right sensor
        if (Physics.Raycast(bodyRightSensor.position, bodyRightSensor.right, out hit, sideSensorLenght))
        {
            Debug.DrawLine(bodyRightSensor.position, hit.point, Color.blue);
        }
        
        // Traffic light detect sensor
        Quaternion rotationUp = Quaternion.AngleAxis(45, -frontRightSensor.right);
        if (Physics.Raycast(middleSensor.position, rotationUp * middleSensor.forward, out hit, straightMiddleSensorLength))
        {
            Debug.DrawLine(middleSensor.position, hit.point, Color.blue);
            if (hit.transform.tag == "RHC" || hit.transform.tag == "RHL")
            {
                if (!hit.transform.GetComponent<TrafficLight>().canPass && Vector3.Angle(hit.transform.forward, transform.forward) < 90)
                {
                    Stop();
                }
            }
        }
    }

    void Deceleration()
    {
        wheelFL.motorTorque = 0;
        wheelFR.motorTorque = 0;
        wheelRL.motorTorque = 0;
        wheelRR.motorTorque = 0;
        wheelFL.brakeTorque = decelerationTorque;
        wheelFR.brakeTorque = decelerationTorque;
        wheelRL.brakeTorque = decelerationTorque;
        wheelRR.brakeTorque = decelerationTorque;
    }

    void Stop()
    {
        wheelFL.motorTorque = 0;
        wheelFR.motorTorque = 0;
        wheelRL.motorTorque = 0;
        wheelRR.motorTorque = 0;
        wheelFL.brakeTorque = brakeTorque;
        wheelFR.brakeTorque = brakeTorque;
        wheelRL.brakeTorque = brakeTorque;
        wheelRR.brakeTorque = brakeTorque;
        if (Vector3.Angle(transform.forward, GetComponent<Rigidbody>().velocity) <= 80)
        {
            GetComponent<Rigidbody>().AddForce(-transform.forward * 40000);
        }
    }

    void Clear()
    {
        foreach (GameObject obj in objList.ToArray())
        {
            if (obj != null && obj.GetComponent<AICar>().pointToObj != gameObject)
            {
                objList.Remove(obj);
            }
        }
    }

    public void Add(GameObject obj)
    {
        if (!objList.Contains(obj))
        {
            objList.Add(obj);
        }
    }

    public void Reset()
    {
        currentTarget = 0;
        autoMode = false;
        GetComponent<CarControl>().autoMode = false;
        startPoint = -1;
        endPoint = -1;
        startFlag = true;
    }

    void EngineSound()
    {

        int i;
        int GearMin;
        int GearMax;
        float currentSpeed;

        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
        //Calculate the gear number
        if (currentSpeed < topSpeed)
        {
            for (i = 0; i < engineGear.Length; i++)
            {
                if (currentSpeed < engineGear[i])
                    break;
            }

            if (i == 0)
            {
                GearMin = 0;
                GearMax = engineGear[i];
            }
            else
            {
                GearMin = engineGear[i - 1];
                GearMax = engineGear[i];
            }

            GetComponent<AudioSource>().pitch = (currentSpeed - GearMin) / (GearMax - GearMin) * 0.3f + 0.2f;
        }
    }
}
