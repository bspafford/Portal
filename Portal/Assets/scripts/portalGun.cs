using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Analytics;

public class portalGun : MonoBehaviour
{
    public GameObject portalObject;
    public Material portal1Color;
    public Material portal2Color;

    public GameObject portal1;
    public GameObject portal2;

    [SerializeField]
    private Material portalMat;

    GameObject box;
    public bool boxThroughPortal = false;

    public GameObject tempObject;

    // Update is called once per frame
    void Update()
    {
        bool leftMouseButton = Input.GetMouseButtonDown(0);
        bool rightMouseButton = Input.GetMouseButtonDown(1);

        if (leftMouseButton || rightMouseButton)
        {
            RaycastHit hit;
            Vector3 startPoint = transform.position;
            Vector3 rayDir = transform.forward;
            float rayDist = 100;
            int layerMask = ~LayerMask.GetMask("portal");
            if (Physics.Raycast(startPoint, rayDir, out hit, rayDist, layerMask))
            {
                if (hit.transform.tag == "portalable")
                {
                    if (leftMouseButton)
                        spawnPortal(hit, ref portal1);
                    else if (rightMouseButton)
                        spawnPortal(hit, ref portal2);
                }
            }
        }


        if (Input.GetKeyDown("e"))
        {
            RaycastHit hit;
            Vector3 startPoint = transform.position;
            Vector3 rayDir = transform.forward;
            float rayDist = 100;
            int layerMask = ~LayerMask.GetMask("portal");
            if (Physics.Raycast(startPoint, rayDir, out hit, rayDist, layerMask))
            {
                // tempObject.SetActive(false);
                if (!box && hit.transform.tag == "box") // pick up box
                {
                    box = hit.transform.gameObject;
                }
                else if (box) // set down box
                {
                    Rigidbody boxRb = box.GetComponent<Rigidbody>();
                    boxRb.useGravity = true;
                    box = null;
                }
                else if (hit.transform.tag == "portalBackground")
                {
                    GameObject portalA;
                    GameObject portalB;
                    getPortals(hit, out portalA, out portalB);

                    float percent = hit.distance / rayDist;

                    // get position relative to portal
                    Vector3 localPos = portalA.transform.InverseTransformPoint(hit.point);
                    localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
                    Vector3 hitPoint = portalB.transform.TransformPoint(localPos);

                    // get direction relative to portal
                    Vector3 localDir = portalA.transform.InverseTransformDirection(rayDir);
                    localDir = new Vector3(-localDir.x, localDir.y, -localDir.z);
                    Vector3 newDir = portalB.transform.TransformDirection(localDir);

                    // through portal raycast
                    RaycastHit portalHit;
                    int portalLayerMask = ~LayerMask.GetMask("portalBackground");
                    Debug.DrawRay(hitPoint, newDir, Color.red);
                    if (Physics.Raycast(hitPoint, newDir, out portalHit, rayDist - hit.distance, portalLayerMask))
                    {
                        if (portalHit.transform.tag == "box" && !box)
                        {
                            box = portalHit.transform.gameObject;
                        }
                    }

                    // Debug.DrawRay(hitPoint, newDir, Color.red, 100);
                }
            }
        }

        if (box)
        {
            Rigidbody boxRb = box.GetComponent<Rigidbody>();

            RaycastHit hit;
            Vector3 startPoint = transform.position;
            Vector3 rayDir = transform.forward;
            float rayDist = 5;

            // problem is when im no longer looking through the portal it will just swap back, but that shouldn't happen.
            // maybe it should just drop it?
            // 

            bool prevBoxThroughPortal = boxThroughPortal;

            int boxLayerMask = LayerMask.GetMask("portalBackground");
            boxThroughPortal = Physics.Raycast(startPoint, rayDir, out hit, rayDist, boxLayerMask);

            Debug.DrawLine(startPoint, transform.forward * rayDist + startPoint, Color.red);

            tempObject.SetActive(false);

            Vector3 goToLoc;
            if (!boxThroughPortal)
            {
                goToLoc = transform.position + transform.forward * rayDist;
            }
            else
            {
                tempObject.SetActive(true);
                tempObject.transform.position = hit.point;

                GameObject portalA;
                GameObject portalB;
                getPortals(hit, out portalA, out portalB);
                if (!portalA || !portalB) return;

                // rotate box
                if (!prevBoxThroughPortal && boxThroughPortal)
                {
                    Vector3 lookDirFromPlayer = box.transform.forward;
                    Quaternion portalRotDiff = portalB.transform.rotation * Quaternion.Inverse(portalA.transform.rotation);
                    Vector3 newCamForward = portalRotDiff * lookDirFromPlayer;
                    newCamForward = new Vector3(newCamForward.x, -newCamForward.y, newCamForward.z);
                    Quaternion newRot = Quaternion.LookRotation(-newCamForward, Vector3.up);
                    box.transform.rotation = newRot;

                    Rigidbody rb = box.GetComponent<Rigidbody>();
                    Vector3 localSpin = portalA.transform.InverseTransformDirection(rb.angularVelocity);
                    localSpin = new Vector3(-localSpin.x, localSpin.y, -localSpin.z);
                    rb.angularVelocity = portalB.transform.TransformDirection(localSpin);
                }

                // position
                Vector3 localPos = portalA.transform.InverseTransformPoint(transform.position + transform.forward * rayDist);
                localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
                Vector3 newWorldPos = portalB.transform.TransformPoint(localPos);

                goToLoc = newWorldPos;
                


                float percent = hit.distance / rayDist;

                // get position relative to portal
                Vector3 localPos1 = portalA.transform.InverseTransformPoint(hit.point);
                localPos1 = new Vector3(-localPos1.x, localPos1.y, -localPos1.z);
                Vector3 hitPoint = portalB.transform.TransformPoint(localPos1);

                // get direction relative to portal
                Vector3 localDir = portalA.transform.InverseTransformDirection(rayDir);
                localDir = new Vector3(-localDir.x, localDir.y, -localDir.z);
                Vector3 newDir = portalB.transform.TransformDirection(localDir);

                Debug.DrawRay(hitPoint, newDir * (rayDist - hit.distance), Color.red);
            }


            boxRb.velocity = Vector3.zero;
            boxRb.useGravity = false;
            // boxRb.AddForce(goToDir, ForceMode.Force);
            box.transform.position = goToLoc;
        }
    }

    void getPortals(RaycastHit hit, out GameObject portalA, out GameObject portalB)
    {
        if (hit.transform.parent && hit.transform.parent.gameObject == portal1)
        {
            portalA = portal1;
            portalB = portal2;
        }
        else if (hit.transform.parent && hit.transform.parent.gameObject == portal2)
        {
            portalA = portal2;
            portalB = portal1;
        }
        else
        {
            portalA = null;
            portalB = null;
            return;
        }
    }

    void spawnPortal(RaycastHit hit, ref GameObject portal)
    {
        Vector3 position = adjustPortalLoc(hit);

        Quaternion rotation = Quaternion.LookRotation(hit.normal);
        rotation *= Quaternion.Euler(0f, 180f, 0f);


        if (portal)
        {
            portal.transform.rotation = rotation;
            portal.transform.position = position;
        }
        else
            portal = Instantiate(portalObject, position, rotation);

        // portal.GetComponent<Renderer>().material = portalMat;
        portal.GetComponent<teleport>().setSinglePortal();
        teleport _teleport = portal.GetComponent<teleport>();
        _teleport.wall = hit.transform.gameObject;
        _teleport.setPlaced(true);

        if (portal1.GetComponent<teleport>().placed && portal2.GetComponent<teleport>().placed) // both portals now exist
            setupPortals();
    }

    void setupPortals()
    {
        portal1.GetComponent<teleport>().Setup(portal2.transform, portal1Color);
        portal2.GetComponent<teleport>().Setup(portal1.transform, portal2Color);
        portal1.GetComponent<Renderer>().enabled = true;
        portal2.GetComponent<Renderer>().enabled = true;
    }

    // send out a certain amount of line casts and see if they intercept an object
    Vector3 adjustPortalLoc(RaycastHit wallHit)
    {
        Vector3 portalLoc = wallHit.point + wallHit.normal * 0.01f;

        Vector3 offset = Vector3.zero;

        // get relative forward and right from normal vector
        Vector3 arbitrary = Mathf.Abs(Vector3.Dot(wallHit.normal, Vector3.up)) > 0.99f ? Vector3.forward : Vector3.up;
        Vector3 right = Vector3.Normalize(Vector3.Cross(arbitrary, wallHit.normal));
        Vector3 forward = Vector3.Normalize(Vector3.Cross(wallHit.normal, right));

        List<Vector3> dirs = new List<Vector3>
        {
            forward,
            right,
            -forward,
            -right
        };

        List<float> distances = new List<float> { 1.1f, 2.1f, 1.1f, 2.1f };

        int lineCastNum = 4;
        for (int i = 0; i < lineCastNum; i++)
        {
            Vector3 currNormal = wallHit.transform.TransformDirection(dirs[i]);
            float currDist = distances[i];

            RaycastHit hit;
            Debug.DrawRay(portalLoc, dirs[i], Color.red);
            if (Physics.Raycast(portalLoc, dirs[i], out hit, distances[i]))
            {
                if (hit.distance >= 2)
                    continue; // in a good spot
                offset -= currNormal * (2 - hit.distance); // needs to move
            }
        }

        return portalLoc + offset;
    }
}
