using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour
{
    public Transform otherPortal;
    private teleport otherPortalScript;

    public bool placed = false;

    Vector3 prevPos;
    GameObject spawnedObject;
    public GameObject wall; // the object the portal is on

    private Material backgroundMaterial;

    [SerializeField]
    Material tempReplicaMat;

    Vector4 getRandLocInCircle()
    {
        float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
        float x = 1 * Mathf.Cos(angle);
        float y = 2 * Mathf.Sin(angle);
        float z = UnityEngine.Random.Range(-5f, 5f);
        float w = UnityEngine.Random.Range(0.25f, 1f);
        return new Vector4(x, y, z, w);
    }

    void Awake()
    {
        gameObject.SetActive(placed);
        GetComponent<Renderer>().enabled = placed;
        otherPortalScript = otherPortal.GetComponent<teleport>();
    }

    public void setSinglePortal()
    {
        List<Vector4> points = new List<Vector4>();
        int pointsNum = 100;
        for (int i = 0; i < pointsNum; i++)
        {
            points.Add(getRandLocInCircle());
        }

        backgroundMaterial = transform.Find("background").GetComponent<Renderer>().material;
        backgroundMaterial.SetVectorArray("_Points", points);
        backgroundMaterial.SetInt("_NumPoints", pointsNum);
    }

    void Update()
    {
        if (backgroundMaterial)
        {
            backgroundMaterial.SetVector("_CamPos", Camera.main.transform.position);
            backgroundMaterial.SetMatrix("_CameraMatrix", Camera.main.cameraToWorldMatrix);
            backgroundMaterial.SetMatrix("_ViewMatrix", Camera.main.worldToCameraMatrix);
            backgroundMaterial.SetVector("_PortalPosition", transform.position);
            backgroundMaterial.SetVector("_PortalNormal", transform.forward);
        }
    }

    public void Setup(Transform otherPortal, Material portalColor)
    {
        this.otherPortal = otherPortal;

        transform.Find("portalFrame").GetComponent<MeshRenderer>().material = portalColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!otherPortalScript.placed)
            return;

        if (wall)
            Physics.IgnoreCollision(other, wall.GetComponent<Collider>());
        if (otherPortal.GetComponent<teleport>().wall)
            Physics.IgnoreCollision(other, otherPortal.GetComponent<teleport>().wall.GetComponent<Collider>());

        if (other.tag != "Player" && other.tag != "cantTP")
        {
            float angle = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, otherPortal.rotation.eulerAngles.y);
            Vector3 locDiff = other.transform.position - transform.position;
            Vector3 rotatedLoc = Quaternion.AngleAxis(angle, Vector3.up) * locDiff;

            // position
            Vector3 newPos = otherPortal.position + new Vector3(-rotatedLoc.x, rotatedLoc.y, -rotatedLoc.z);

            // rotation
            Quaternion portalRotDiff = otherPortal.rotation * Quaternion.Inverse(transform.rotation);
            Vector3 lookDirFromPlayer = other.transform.forward;
            Vector3 newCamForward = portalRotDiff * lookDirFromPlayer;
            newCamForward = new Vector3(newCamForward.x, -newCamForward.y, newCamForward.z);
            Quaternion newRot = Quaternion.LookRotation(-newCamForward, Vector3.up);
            
            // spawn object
            Vector3 newVelocity = other.GetComponent<Rigidbody>().velocity;
            spawnedObject = Instantiate(other.gameObject, newPos, newRot);
            // spawnedObject.GetComponent<Collider>().enabled = false;
            spawnedObject.tag = "cantTP";
            // spawnedObject.GetComponent<SphereCollider>().enabled = false;
            spawnedObject.GetComponent<Rigidbody>().velocity = newVelocity;
            TrackObject trackObject = spawnedObject.AddComponent<TrackObject>();
            trackObject.Setup(other.gameObject, transform, otherPortal);
            // spawnedObject.GetComponent<Renderer>().material = tempReplicaMat;
        }

        prevPos = other.bounds.center;
    }

    void OnTriggerStay(Collider other)
    {   
        if (!otherPortalScript.placed)
            return;

        if (other.tag == "cantTP")
            return;


        Vector3 portalCenter = GetComponent<Collider>().bounds.center;
        Vector3 portalNormal = transform.forward;

        Vector3 currentCenter = Vector3.zero;
        if (other.transform.parent && other.transform.parent.Find("Main Camera"))
            currentCenter = other.transform.parent.Find("Main Camera").position;
        else
            currentCenter = other.bounds.center;
            

        if (prevPos != -Vector3.zero)
        {
            float prevDot = Vector3.Dot(prevPos - portalCenter, portalNormal);
            float currDot = Vector3.Dot(currentCenter - portalCenter, portalNormal);

            bool crossed = prevDot < 0f && currDot >= 0f;

            Vector3 movement = currentCenter - prevPos;
            float approach = Vector3.Dot(movement.normalized, portalNormal);
            bool movingToward = approach > 0.5f;

            if (crossed && movingToward)
                teleportObject(other);
        }

        prevPos = currentCenter;
    }

    void teleportObject(Collider other)
    {
        if (other.tag != "cantTP")
        {
            float angle = Mathf.DeltaAngle(transform.rotation.eulerAngles.y, otherPortal.rotation.eulerAngles.y);

            if (other.transform.parent && other.transform.parent.Find("Main Camera")) // if character
            {
                Transform playerCamera = other.transform.parent.Find("Main Camera");
                Rigidbody rb = other.transform.parent.GetComponent<Rigidbody>();

                Transform characterTransform = other.transform.parent;

                // set location
                Vector3 localPos = transform.InverseTransformPoint(characterTransform.position);
                localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
                Vector3 newWorldPos = otherPortal.transform.TransformPoint(localPos);
                characterTransform.position = newWorldPos;

                // set rotation
                Vector3 localDir = transform.InverseTransformDirection(characterTransform.forward);
                localDir = new Vector3(-localDir.x, localDir.y, -localDir.z);
                Vector3 newWorldDir = otherPortal.transform.TransformDirection(localDir);
                characterTransform.rotation = Quaternion.LookRotation(newWorldDir, Vector3.up);

                // set velocity
                Vector3 localVelocity = transform.InverseTransformVector(rb.velocity);
                localVelocity = new Vector3(-localVelocity.x, localVelocity.y, -localVelocity.z);
                Vector3 newWorldVelocity = otherPortal.transform.TransformVector(localVelocity);
                rb.velocity = newWorldVelocity;

                // set angular velocity
                Vector3 localSpin = transform.transform.InverseTransformDirection(rb.angularVelocity);
                localSpin = new Vector3(-localSpin.x, localSpin.y, -localSpin.z);
                rb.angularVelocity = otherPortal.transform.TransformDirection(localSpin);
                rb.angularVelocity = Vector3.zero;
            }
            else // if not character
            {
                // swap places with spawned object
                Vector3 spawnedObjectPos = spawnedObject.transform.position;
                Quaternion spawnedObjectRot = spawnedObject.transform.rotation;

                spawnedObject.GetComponent<TrackObject>().setPortals(transform, otherPortal);

                other.transform.position = spawnedObjectPos;
                // other.transform.rotation = spawnedObjectRot;

                Quaternion deltaRotation = otherPortal.rotation * Quaternion.Euler(0, 180f, 0) * Quaternion.Inverse(transform.rotation);
                other.transform.rotation = deltaRotation * other.transform.rotation;

                Rigidbody rb = other.transform.GetComponent<Rigidbody>();
                rb.velocity = Quaternion.AngleAxis(angle, Vector3.up) * new Vector3(-rb.velocity.x, rb.velocity.y, -rb.velocity.z); // rotates velocity
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!otherPortalScript.placed)
            return;

        if (wall)
            Physics.IgnoreCollision(other, wall.GetComponent<Collider>(), false);
        if (otherPortal.GetComponent<teleport>().wall)
            Physics.IgnoreCollision(other, otherPortal.GetComponent<teleport>().wall.GetComponent<Collider>(), false);

        if (other.tag == "cantTP")
        {
            Destroy(other.gameObject);
        }
    }

    public void setPlaced(bool placed)
    {
        this.placed = placed;
        gameObject.SetActive(placed);
    }
}