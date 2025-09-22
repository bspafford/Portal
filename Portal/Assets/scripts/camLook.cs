using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class camLook : MonoBehaviour
{
    public float sensX = 0.5f;
    public float sensY = 0.5f;
    float xRotation;
    float yRotation;
    Transform meshTransform;

    [SerializeField]
    Transform spine;

    // Start is called before the first frame update
    void Start()
    {
        meshTransform = transform.parent.Find("characterMesh");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        // float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;
        // yRotation += mouseX;
        // xRotation -= mouseY;
        // xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        // meshTransform.rotation = Quaternion.Euler(0, yRotation, 0);
        float mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

        // Get current Euler angles
        Vector3 currentEuler = transform.rotation.eulerAngles;

        // Convert to signed angles to handle wraparound issues (0-360 → -180 to 180)
        float currentX = currentEuler.x;
        if (currentX > 180f) currentX -= 360f;

        float currentY = currentEuler.y;

        // Apply deltas
        currentX -= mouseY;
        currentY += mouseX;

        // Clamp pitch (x axis)
        currentX = Mathf.Clamp(currentX, -90f, 90f);

        // Apply rotation
        Quaternion newRotation = Quaternion.Euler(currentX, currentY, 0f);
        transform.rotation = newRotation;

        // Rotate mesh only on Y
        meshTransform.rotation = Quaternion.Euler(0f, currentY, 0f);

        spine.localRotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.x);
    }

    public void SetRotation(Vector3 rot)
    {
        // yRotation = rot;
    }
}
