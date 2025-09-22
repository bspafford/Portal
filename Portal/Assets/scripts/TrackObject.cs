using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackObject : MonoBehaviour
{
    GameObject objectFollowing;
    Transform portal1;
    Transform portal2;

    public void Setup(GameObject objectFollowing, Transform portal1, Transform portal2)
    {
        this.objectFollowing = objectFollowing;
        this.portal1 = portal1;
        this.portal2 = portal2;
    }

    public void setPortals(Transform portalA, Transform portalB)
    {
        print("swapping");
        portal1 = portalA;
        portal2 = portalB;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.DeltaAngle(portal1.transform.rotation.eulerAngles.y, portal2.rotation.eulerAngles.y);
        Vector3 locDiff = objectFollowing.transform.position - portal1.transform.position;
        Vector3 rotatedLoc = Quaternion.AngleAxis(angle, Vector3.up) * locDiff;

        // position
        Vector3 newPos = portal2.position + new Vector3(-rotatedLoc.x, rotatedLoc.y, -rotatedLoc.z);
        Rigidbody rb = objectFollowing.transform.GetComponent<Rigidbody>();
        // rb.velocity = Quaternion.AngleAxis(angle, Vector3.up) * -rb.velocity; // rotates velocity

        // rotation
        Quaternion relativeRotation = Quaternion.Inverse(portal1.rotation) * objectFollowing.transform.rotation;
        Quaternion newWorldRotation = portal2.rotation * Quaternion.Euler(0, 180, 0) * relativeRotation;
        transform.rotation = newWorldRotation;

        transform.position = newPos;
        // transform.rotation = newRot;

        objectFollowing.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
