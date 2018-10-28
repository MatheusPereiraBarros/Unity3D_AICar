using UnityEngine;
using System.Collections;

public class CarControl : MonoBehaviour {

    public WheelCollider wheelFL;
    public WheelCollider wheelFR;
    public WheelCollider wheelRR;
    public WheelCollider wheelRL;
    public Transform wheelTransformFL;
    public Transform wheelTransformFR;
    public Transform wheelTransformRL;
    public Transform wheelTransformRR;
    public GameObject backLight;
    public Material brakeBackLight;
    public Material offBackLight;
    private float currentSpeed;
    private float maxTorque = 800f;
    private float maxSteerAngle = 30f;
    private float minSteerAngle = 10f;
    private float brakeTorque = 300f;
    private float topSpeed = 33f;
    private float topGasSpeed = 44f;
    private float topReverseSpeed = 10f;
    private float footBrakeTorque = 20000f;
    private float handBrakeTorque = 10000f;
    private float moveForward = 0;
    private float throttle = 0;
    private float FWSlipForwardStiffness;
    private float FWSlipSidewayStiffness;
    private float RWSlipForwardStiffness;
    private float RWSlipSidewayStiffness;
    private bool handbrake = false;
    private int[] engineGear = new int[5];
    public float gasCharge;
    public GameObject gasPadelEffect;
    public Transform LGasPedal;
    public Transform RGasPedal;
    public GameObject gasPedalSound;
    public bool gasFlag = false;
    private GameObject temp1;
    private GameObject temp2;
    private GameObject temp3;
    public bool autoMode;

    void Start () {

        GetComponent<Rigidbody>().centerOfMass = GetComponent<Rigidbody>().centerOfMass + new Vector3(0, -0.7f, 0);
        for(int i = 0; i < engineGear.Length; i++)
        {
            engineGear[i] = (int)((i + 1) * topSpeed / engineGear.Length) + 1;
        }
        FWSlipForwardStiffness = 0.8f;
        FWSlipSidewayStiffness = 0.7f;
        RWSlipForwardStiffness = 0.7f;
        RWSlipSidewayStiffness = 0.4f;
    }
	
	void FixedUpdate () {
        if (!autoMode)
        {
          ApplyDrive();
          Brake();
        }
        WheelUpdate();
        Suspension();
        LightUpdate();
        EngineSound();
        GasPedalSys();
    }

    void ApplyDrive()
    {
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
        throttle = Input.GetAxis("Vertical");
        if(Vector3.Angle(transform.forward, GetComponent<Rigidbody>().velocity) <= 80)//Move forward
        {
            moveForward = 1;
        }
        else if(Vector3.Angle(transform.forward, GetComponent<Rigidbody>().velocity) >= 100)//Back
        {
            moveForward = -1;
        }
        else
        {
            moveForward = 0;
        }
        //Apply torque to the wheel to give car a move power
        //Limit speed
        if (moveForward != -1)
        {
            if (currentSpeed < topSpeed)
            {
                wheelRR.motorTorque = maxTorque * throttle;
                wheelRL.motorTorque = maxTorque * throttle;
                wheelFR.motorTorque = maxTorque * throttle;
                wheelFL.motorTorque = maxTorque * throttle;
            }
            else
            {
                wheelRR.motorTorque = 0;
                wheelRL.motorTorque = 0;
                wheelFR.motorTorque = 0;
                wheelFL.motorTorque = 0;
            }
        }
        else
        {
            if (currentSpeed < topReverseSpeed)
            {
                wheelRR.motorTorque = maxTorque * throttle;
                wheelRL.motorTorque = maxTorque * throttle;
                wheelFR.motorTorque = maxTorque * throttle;
                wheelFL.motorTorque = maxTorque * throttle;
            }
            else
            {
                wheelRR.motorTorque = 0;
                wheelRL.motorTorque = 0;
                wheelFR.motorTorque = 0;
                wheelFL.motorTorque = 0;
            }
        }

        //Hard to steer in high speed
        float currentSteerAngle = Mathf.Lerp(maxSteerAngle, minSteerAngle, currentSpeed / topSpeed);

        wheelFL.steerAngle = currentSteerAngle * Input.GetAxis("Horizontal");//Apply steer to the wheel
        wheelFR.steerAngle = currentSteerAngle * Input.GetAxis("Horizontal");
        
    }

    void WheelUpdate() {
        Vector3 SteerWheel;
        //Calculate wheel graphic steer to fix collider steer
        SteerWheel = wheelTransformFL.localEulerAngles;
        SteerWheel.y = wheelFL.steerAngle;
        SteerWheel.z = 0;
        //Steer the two front wheels
        wheelTransformFL.localEulerAngles = SteerWheel;
        wheelTransformFR.localEulerAngles = SteerWheel;
        //Rotate all wheels
        wheelTransformFL.Rotate(Vector3.right * wheelFL.rpm / 60 * 360 * Time.deltaTime);// ω = rpm / 60 * 360;
        wheelTransformFR.Rotate(Vector3.right * wheelFR.rpm / 60 * 360 * Time.deltaTime);
        wheelTransformRL.Rotate(Vector3.right * wheelRL.rpm / 60 * 360 * Time.deltaTime);
        wheelTransformRR.Rotate(Vector3.right * wheelRR.rpm / 60 * 360 * Time.deltaTime);
    }

    void Suspension() {
        RaycastHit hit;
        Vector3 WheelPositionFL;
        Vector3 WheelPositionFR;
        Vector3 WheelPositionRL;
        Vector3 WheelPositionRR;
        //Inside suspension distance
        if (Physics.Raycast(wheelFL.transform.position, -wheelFL.transform.up, out hit, wheelFL.radius + wheelFL.suspensionDistance))
        {
            WheelPositionFL = hit.point + wheelFL.transform.up * wheelFL.radius;
        }
        //Outside suspension distance
        else {
            WheelPositionFL = wheelFL.transform.position - wheelFL.transform.up * wheelFL.suspensionDistance;
        }
        wheelTransformFL.position = WheelPositionFL;

        //FR
        if (Physics.Raycast(wheelFR.transform.position, -wheelFR.transform.up, out hit, wheelFR.radius + wheelFR.suspensionDistance))
        {
            WheelPositionFR = hit.point + wheelFR.transform.up * wheelFR.radius;
        }
        else
        {
            WheelPositionFR = wheelFR.transform.position - wheelFR.transform.up * wheelFR.suspensionDistance;
        }
        wheelTransformFR.position = WheelPositionFR;

        //RL
        if (Physics.Raycast(wheelRL.transform.position, -wheelRL.transform.up, out hit, wheelRL.radius + wheelRL.suspensionDistance))
        {
            WheelPositionRL = hit.point + wheelRL.transform.up * wheelRL.radius;
        }
        else
        {
            WheelPositionRL = wheelRL.transform.position - wheelRL.transform.up * wheelRL.suspensionDistance;
        }
        wheelTransformRL.position = WheelPositionRL;

        //RR
        if (Physics.Raycast(wheelRR.transform.position, -wheelRR.transform.up, out hit, wheelRR.radius + wheelRR.suspensionDistance))
        {
            WheelPositionRR = hit.point + wheelRR.transform.up * wheelRR.radius;
        }
        else
        {
            WheelPositionRR = wheelRR.transform.position - wheelRR.transform.up * wheelRR.suspensionDistance;
        }
        wheelTransformRR.position = WheelPositionRR;
    }

    void LightUpdate()
    {
        if ((throttle < 0 && moveForward == 1) || handbrake)//Brake
        {
            backLight.GetComponent<Renderer>().material = brakeBackLight;
        }
        else
        {
            backLight.GetComponent<Renderer>().material = offBackLight;
        }
    }

    void Brake()
    {
        if (Input.GetButton("Jump"))
        {
            handbrake = true;
        }
        else
        {
            handbrake = false;
        }

        if (!handbrake)
        {
            SetSlip(1, 1, 1, 1);
            //Apply footbrake
            if ((throttle < 0 && moveForward == 1) || (throttle > 0 && moveForward == -1))
            {
                wheelRR.brakeTorque = footBrakeTorque;
                wheelRL.brakeTorque = footBrakeTorque;
                wheelFR.brakeTorque = footBrakeTorque;
                wheelFL.brakeTorque = footBrakeTorque;

                //Add a force to make car easy to stop
                if(throttle < 0 && moveForward == 1)
                    GetComponent<Rigidbody>().AddForce(-transform.forward * 20000);
            }
            //Release up and down arrow 
            else if (throttle == 0)
            {
                wheelRR.brakeTorque = brakeTorque;
                wheelRL.brakeTorque = brakeTorque;
                wheelFR.brakeTorque = brakeTorque;
                wheelFL.brakeTorque = brakeTorque;
            }
            else
            {
                wheelRR.brakeTorque = 0;
                wheelRL.brakeTorque = 0;
                wheelFR.brakeTorque = 0;
                wheelFL.brakeTorque = 0;
            }
        }
        else//handbrake
        {
            wheelRR.brakeTorque = handBrakeTorque;
            wheelRL.brakeTorque = handBrakeTorque;
            SetSlip(FWSlipForwardStiffness, FWSlipSidewayStiffness, RWSlipForwardStiffness, RWSlipSidewayStiffness);
        }
        
    }

    void SetSlip(float FWSlipForwardStiffness, float FWSlipSidewayStiffness, float RWSlipForwardStiffness, float RWSlipSidewayStiffness)
    {
        WheelFrictionCurve RRfFriction = wheelRR.forwardFriction;
        WheelFrictionCurve RLfFriction = wheelRL.forwardFriction;
        WheelFrictionCurve RRsFriction = wheelRR.sidewaysFriction;
        WheelFrictionCurve RLsFriction = wheelRL.sidewaysFriction;
        WheelFrictionCurve FRfFriction = wheelFR.forwardFriction;
        WheelFrictionCurve FLfFriction = wheelFL.forwardFriction;
        WheelFrictionCurve FRsFriction = wheelFR.sidewaysFriction;
        WheelFrictionCurve FLsFriction = wheelFL.sidewaysFriction;
        RRfFriction.stiffness = RWSlipForwardStiffness;
        RLfFriction.stiffness = RWSlipForwardStiffness;
        RRsFriction.stiffness = RWSlipSidewayStiffness;
        RLsFriction.stiffness = RWSlipSidewayStiffness;
        FRfFriction.stiffness = FWSlipForwardStiffness;
        FLfFriction.stiffness = FWSlipForwardStiffness;
        FRsFriction.stiffness = FWSlipSidewayStiffness;
        FLsFriction.stiffness = FWSlipSidewayStiffness;
        wheelRR.forwardFriction = RRfFriction;
        wheelRL.forwardFriction = RLfFriction;
        wheelRR.sidewaysFriction = RRsFriction;
        wheelRL.sidewaysFriction = RLsFriction;
        wheelFR.forwardFriction = FRfFriction;
        wheelFL.forwardFriction = FLfFriction;
        wheelFR.sidewaysFriction = FRsFriction;
        wheelFL.sidewaysFriction = FLsFriction;
    }

    void EngineSound()
    {
        int i;
        int GearMin;
        int GearMax;
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

    void GasPedalSys()
    {
        if(gasCharge > 0)
        {
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if(!gasFlag)
                {
                    temp1 = (GameObject)Instantiate(gasPadelEffect, LGasPedal, false);
                    temp2 = (GameObject)Instantiate(gasPadelEffect, RGasPedal, false);
                    temp3 = (GameObject)Instantiate(gasPedalSound, transform, false);
                    gasFlag = true;
                }
                float GasTime = 30;
                gasCharge -= GasTime * Time.deltaTime;
                if (currentSpeed < topGasSpeed)
                    GetComponent<Rigidbody>().AddForce(transform.forward * 30000);
            }
            else
            {
                gasFlag = false;
                if (temp1) Destroy(temp1);
                if (temp2) Destroy(temp2);
                if (temp3) Destroy(temp3);
            }
        }
        if (gasCharge < 0)
        {
            gasFlag = false;
            if (temp1) Destroy(temp1);
            if (temp2) Destroy(temp2);
            if (temp3) Destroy(temp3);
        }
        if(currentSpeed > topSpeed)
            GetComponent<Rigidbody>().AddForce(transform.forward * -10000);
    }
}