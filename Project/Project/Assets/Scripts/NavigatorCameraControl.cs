using UnityEngine;
using System.Collections;

public class NavigatorCameraControl : MonoBehaviour {

    private float cameraSpeed = 250f;
    private float zoomSpeed = 5f;
    private float rotateSpeed = 2f;
    private Camera thisCamera;
    private Vector3 destination = new Vector3(0, 0, 0);
    public Texture2D mouseUpArrow;
    public Texture2D mouseRightArrow;
    public Texture2D mouseDownArrow;
    public Texture2D mouseLeftArrow;

    void Start()
    {
        thisCamera = GetComponent<Camera>();
    }

    void Update()
    {
        CameraMove();
    }

    void CameraMove()
    {

        Vector2 mouseOffset = new Vector2(0f, 0f);
        float moveOffset = 10f;
        if (Input.mousePosition.y >= Screen.height - moveOffset)
        {
            mouseOffset = new Vector2(0f,0f);
            Cursor.SetCursor(mouseUpArrow, mouseOffset, CursorMode.ForceSoftware);
            transform.Translate(Vector3.up * cameraSpeed * Time.deltaTime);
        }
        else if(Input.mousePosition.y <= 0 + moveOffset)
        {
            mouseOffset = new Vector2(0f, 30f);
            Cursor.SetCursor(mouseDownArrow, mouseOffset, CursorMode.ForceSoftware);
            transform.Translate(-Vector3.up * cameraSpeed * Time.deltaTime);
        }

        if(Input.mousePosition.x >= Screen.width - moveOffset)
        {
            mouseOffset = new Vector2(30f, 0f);
            Cursor.SetCursor(mouseRightArrow, mouseOffset, CursorMode.ForceSoftware);
            transform.Translate(Vector3.right * cameraSpeed * Time.deltaTime);
        }
        else if (Input.mousePosition.x <= 0 + moveOffset)
        {
            mouseOffset = new Vector2(0f, 0f);
            Cursor.SetCursor(mouseLeftArrow, mouseOffset, CursorMode.ForceSoftware);
            transform.Translate(-Vector3.right * cameraSpeed * Time.deltaTime);
        }

        if(Input.mouseScrollDelta.y <= 0)
        {
            transform.Translate(-Vector3.forward * cameraSpeed * zoomSpeed * Mathf.Abs(Input.mouseScrollDelta.y) * Time.deltaTime);
        }
        else if (Input.mouseScrollDelta.y >= 0)
        {
            transform.Translate(Vector3.forward * cameraSpeed * zoomSpeed * Mathf.Abs(Input.mouseScrollDelta.y) * Time.deltaTime);
        }
        
        if (Input.GetKey(KeyCode.Mouse1))
        {
            float rotation = Input.GetAxis("Mouse X");
            Quaternion cameraRotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + rotation * rotateSpeed);
            transform.rotation = cameraRotation;
        }

        if(Input.mousePosition.x > 0 + moveOffset && Input.mousePosition.x < Screen.width - moveOffset  && Input.mousePosition.y > 0 + moveOffset && Input.mousePosition.y < Screen.height - moveOffset)
        {
            mouseOffset = new Vector2(0f, 0f);
            Cursor.SetCursor(null, mouseOffset, CursorMode.ForceSoftware);
        }
    }

    public Vector3 GetPosition()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = thisCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                destination = hit.point;
            }
        }
        return destination;
    }
}
