using UnityEngine;
using System.Collections;

public class CarGUI : MonoBehaviour {

    public Texture2D speedometer;
    public Texture2D needle;
    public Texture2D gasPedal;
    public Texture2D gasPedalContainer;
    public float currentSpeed;
    public float gasCharge;

    void OnGUI()
    {
        gasCharge = GetComponent<CarControl>().gasCharge;
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
        GUI.DrawTexture(new Rect(Screen.width - 200, Screen.height - 200, 200, 200), speedometer);
        //Calculate to set gasPedal
        float GasRatio = gasCharge / 100;
        GUI.DrawTexture(new Rect(Screen.width - 220, Screen.height - 100 + 80 * (1 - GasRatio), 20, 80 * GasRatio), gasPedal);
        GUI.DrawTexture(new Rect(Screen.width - 220, Screen.height - 100, 20, 80), gasPedalContainer);
        //Calculate needle rotation
        float NeedleRotation = currentSpeed * 3600 / 1000;
        GUIUtility.RotateAroundPivot(NeedleRotation, new Vector2(Screen.width - 100, Screen.height - 100));
        GUI.DrawTexture(new Rect(Screen.width - 200, Screen.height - 200, 200, 200), needle);
    }
}
