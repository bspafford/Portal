using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class trackCharacter : MonoBehaviour
{
    public Transform portal1;
    public Transform characterCameraTransform;
    public Transform portal2;

    public void Setup(Transform portal1, Transform characterCameraTransform, Transform portal2)
    {
        this.portal1 = portal1;
        this.characterCameraTransform = characterCameraTransform;
        this.portal2 = portal2;
    }

    // Update is called once per frame
    void Update()
    {
        if (!portal1 || !portal2 || !characterCameraTransform)
            return;

        // position        
        float angle = Mathf.DeltaAngle(portal1.rotation.eulerAngles.y, portal2.rotation.eulerAngles.y);
        Vector3 locDiff = characterCameraTransform.position - portal1.position;
        Vector3 rotatedLoc = Quaternion.AngleAxis(angle, Vector3.up) * locDiff;
        transform.position = portal2.position + new Vector3(-rotatedLoc.x, rotatedLoc.y, -rotatedLoc.z);

        // rotation
        Quaternion rotDiff = portal2.rotation * Quaternion.Inverse(portal1.rotation) * Quaternion.Euler(0, 180, 0);
        transform.rotation = rotDiff * characterCameraTransform.rotation;
    }
}
