using UnityEngine;
using System.Collections;

public class AutoButtonFunction : MonoBehaviour {

    public GameObject car;
    public GameObject navigator;
    public GameObject selectEffect;
    public bool confirmFlag = false;
    private Vector3 selection;
    private Vector3 previousSelection;
    private GameObject temp;
    private GameObject previousTemp;

    void Start()
    {

    }

	void Update () {
        if (Input.GetKey(KeyCode.Mouse0) && !(Input.mousePosition.x < 200 && Input.mousePosition.y > Screen.height - 50))
        {
            selection = navigator.GetComponent<NavigatorCameraControl>().GetPosition();
            if (selection != previousSelection)
            {
                Quaternion rotation = Quaternion.Euler(-90, 0, 0);
                temp = (GameObject)Instantiate(selectEffect, selection, rotation);
                Destroy(previousTemp);
            }
            previousSelection = selection;
            previousTemp = temp;
        }

        if (confirmFlag)
        {
            car.GetComponent<AutoDrive>().Reset();
            car.GetComponent<AutoDrive>().destination = selection;
            car.GetComponent<AutoDrive>().autoMode = true;
            car.GetComponent<CarControl>().autoMode = true;
            confirmFlag = false;
        }
	}
}
