using UnityEngine;

public class CamFollow : MonoBehaviour
{
    public Transform player; 
    public float cameraAngle = 45f; 
    public float smoothTime = 0.3f;
    public float turnThreshold = 0.6f;

    [SerializeField]
    float cameraHeight = 10f; 
    [SerializeField]
    float cameraDistance = 3f;

    public float rotationSpeed = 100f;
    public float mouseSensitivity = 3;
    public Vector2 camLimit;

    private float currentRotationAngle = 0f;
    private Vector3 velocity = Vector3.zero;
    private float pitch = 0f;
    float mouseX, mouseY;

    [HideInInspector]
    public float distanceHeight_Ratio;


    private void Start()
    {
        distanceHeight_Ratio = cameraDistance / cameraHeight;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        Vector3 offset = new Vector3(0, cameraHeight, -cameraDistance);

        mouseX += Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY += Input.GetAxis("Mouse Y") * mouseSensitivity;
        mouseY = Mathf.Clamp(mouseY, camLimit.x, camLimit.y);
        //transform.rotation = Quaternion.Euler(-mouseY, mouseX, 0);

        Quaternion rotation = Quaternion.Euler(-mouseY, mouseX, 0); //Quaternion.Euler(pitch, currentRotationAngle, 0);
        Vector3 desiredPosition = player.position + rotation * offset;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);
        transform.LookAt(player.position + Vector3.up * cameraHeight * 0.5f);
    }

    public void setHeight(float height)
    {
        cameraHeight = height;
        cameraDistance = cameraHeight * distanceHeight_Ratio;
        Debug.Log("Setting cam HEIGHT to = " + height + " AND cam DISTANCE to = " + cameraDistance);
    }

    public float getHeight()
    {
        return cameraHeight;
    }

}