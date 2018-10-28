using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonControl : MonoBehaviour {

    public GameObject navigatorCamera;
    public GameObject carCamera;
    public GameObject car;
    public GameObject autoDriveButton;
    Button[] buttons;
    Text selectTargetText;

    void Start()
    {
        SwitchToCar();
        buttons = GetComponentsInChildren<Button>();
        StartCoroutine(WaitToDisableAutoFunction(0.1f));
        selectTargetText = GameObject.Find("SelectTargetText").GetComponent<Text>();
        selectTargetText.enabled = false;
        InvisibleButton(GameObject.Find("ConfirmButton").GetComponent<Button>());
        InvisibleButton(GameObject.Find("QuitButton").GetComponent<Button>());
    }

	public void ClickAutoButton()
    {
        SetButton();
        SwitchToNavigator();
        autoDriveButton.GetComponent<AutoButtonFunction>().enabled = true;
        InvisibleButton(GameObject.Find("AutoDriveButton").GetComponent<Button>());
        selectTargetText.enabled = true;
        GameObject.Find("AutoDriveButton").GetComponent<AutoButtonFunction>();
    }

    public void ClickConfirmButton()
    {
        SetButton();
        SwitchToCar();
        InvisibleButton(GameObject.Find("ConfirmButton").GetComponent<Button>());
        InvisibleButton(GameObject.Find("QuitButton").GetComponent<Button>());
        selectTargetText.enabled = false;
        ConfirmButton();
        StartCoroutine(WaitToDisableAutoFunction(1f));
    }

    public void ClickQuitButton()
    {
        SetButton();
        SwitchToCar();
        InvisibleButton(GameObject.Find("QuitButton").GetComponent<Button>());
        InvisibleButton(GameObject.Find("ConfirmButton").GetComponent<Button>());
        selectTargetText.enabled = false;
        QuitButton();
        StartCoroutine(WaitToDisableAutoFunction(1f));
    }

    void InvisibleButton(Button thisButton)
    {
        thisButton.interactable = false;
        thisButton.GetComponentInChildren<Text>().enabled = false;
    }

    void SetButton()
    {
        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = true;
            buttons[i].GetComponentInChildren<Text>().enabled = true;
        }
    }

    void SwitchToNavigator()
    {
        navigatorCamera.SetActive(true);
        carCamera.SetActive(false);
        car.GetComponent<CarControl>().enabled = false;
    }

    void SwitchToCar()
    {
        navigatorCamera.SetActive(false);
        carCamera.SetActive(true);
        car.GetComponent<CarControl>().enabled = true;
    }

    void ConfirmButton()
    {
        autoDriveButton.GetComponent<AutoButtonFunction>().confirmFlag = true;
    }

    void QuitButton()
    {

    }

    IEnumerator WaitToDisableAutoFunction(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        autoDriveButton.GetComponent<AutoButtonFunction>().enabled = false;
    }
}
