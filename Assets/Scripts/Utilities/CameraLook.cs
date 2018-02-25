using UnityEngine;
using UnityEngine.EventSystems;

public class CameraLook : MonoBehaviour
{
    public Transform cameraRig;
    public float distance = 5.0f;
    public float xSpeed = 3.0f;
    public float ySpeed = 2.0f;
    public float zSpeed = 2.0f;
    public float zRange = 8f;

    private float x;
    private float y;
    private float z;

    private float yMin = -30.0f;
    private float yMax = 80.0f;
    private float zMin, zMax;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        z = -distance;
        zMin = -distance - zRange / 2;
        zMax = -distance + zRange / 2;
        RotatePosition();
    }

    void LateUpdate()
    {
        if (cameraRig == null)
        {
            return;
        }
        if (Input.GetMouseButton(0)&&!EventSystem.current.IsPointerOverGameObject())
        {
            x += Input.GetAxis("Mouse X") * xSpeed;
            y -= Input.GetAxis("Mouse Y") * ySpeed;
        }
        
        z += Input.GetAxis("Mouse ScrollWheel") * zSpeed;
        y = Mathf.Clamp(y, yMin, yMax);
        z = Mathf.Clamp(z, zMin, zMax);
        RotatePosition();
    }

    void RotatePosition()
    {
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        Vector3 position = rotation * new Vector3(0.0f, 0.0f, z) + cameraRig.position;

        transform.position = position;
        transform.rotation = rotation;
    }


}
