using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class drawVisible : MonoBehaviour
{
    private Mesh triangleMesh;
    private Mesh throughPortalMesh;

    private float viewDistance = 5;
    private float fov = 30;


    [SerializeField]
    Material camViewMat;
    [SerializeField]
    Material throughPortal1Mat;

    [SerializeField]
    GameObject portal1;
    [SerializeField]
    GameObject portal2;

    [SerializeField]
    bool isPortalCamera;

    private void OnDrawGizmos()
    {
        // if (triangleMesh == null)
        CreateTriangleMesh();

        // Set material (only necessary if you want color)
        Material mat = new Material(camViewMat);
        mat.SetPass(0);
        Graphics.DrawMeshNow(triangleMesh, Matrix4x4.identity);

        mat = new Material(throughPortal1Mat);
        mat.SetPass(0);
        Graphics.DrawMeshNow(throughPortalMesh, Matrix4x4.identity);

        if (isPortalCamera)
        {
            Transform character = GameObject.Find("character").transform;
            Vector3 localPos = portal2.transform.InverseTransformPoint(character.position);
            localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
            transform.position = portal1.transform.TransformPoint(localPos);

            Vector3 localDir = portal2.transform.InverseTransformDirection(character.forward);
            localDir = new Vector3(-localDir.x, localDir.y, -localDir.z);
            transform.rotation = Quaternion.LookRotation(portal1.transform.TransformDirection(localDir), Vector3.up);
        }
    }

    void Update()
    {

    }

    void CreateTriangleMesh()
    {
        float opposite = Mathf.Tan(fov / 2) * viewDistance;

        triangleMesh = new Mesh();

        Vector3[] vertices = new Vector3[3];
        Vector3 pos = transform.position - new Vector3(0, 0.01f, 0);
        vertices[0] = pos;

        Vector3 worldPos1 = transform.TransformPoint(new Vector3(opposite, 0, viewDistance));
        Vector3 worldPos2 = transform.TransformPoint(new Vector3(-opposite, 0, viewDistance));
        worldPos1.y = 0.01f;
        worldPos2.y = 0.01f;

        vertices[1] = worldPos1;
        vertices[2] = worldPos2;

        Vector3 portalPos1 = portal1.transform.position + (portal1.transform.right / 2);
        Vector3 portalPos2 = portal1.transform.position - (portal1.transform.right / 2);

        Vector3 intersection1;
        Vector3 intersection2;
        findIntersection(transform.position, portalPos1, worldPos1, worldPos2, out intersection1);
        findIntersection(transform.position, portalPos2, worldPos1, worldPos2, out intersection2);

        float y1 = 1f;

        portalPos1.y = y1;
        portalPos2.y = y1;
        intersection1.y = y1;
        intersection2.y = y1;
        // Debug.DrawLine(transform.position, intersection1);
        // Debug.DrawLine(transform.position, intersection2);

        int[] triangles = new int[] { 0, 1, 2 };

        triangleMesh.vertices = vertices;
        triangleMesh.triangles = triangles;

        // camera pos
        // portal pos 1
        // then see where it hits the end?

        throughPortalMesh = new Mesh();

        vertices = new Vector3[4];
        vertices[0] = portalPos2;
        vertices[1] = portalPos1;
        vertices[2] = intersection1;
        vertices[3] = intersection2;

        triangles = new int[] { 2, 1, 0,
                                0, 3, 2 };

        throughPortalMesh.vertices = vertices;
        throughPortalMesh.triangles = triangles;
    }
    
    public static bool findIntersection(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, out Vector3 intersection)
    {
        intersection = Vector3.zero;

        float x1 = p1.x, z1 = p1.z;
        float x2 = p2.x, z2 = p2.z;
        float x3 = p3.x, z3 = p3.z;
        float x4 = p4.x, z4 = p4.z;

        float denom = (x1 - x2) * (z3 - z4) - (z1 - z2) * (x3 - x4);

        if (Mathf.Approximately(denom, 0f))
        {
            return false;
        }

        float px = ((x1 * z2 - z1 * x2) * (x3 - x4) - (x1 - x2) * (x3 * z4 - z3 * x4)) / denom;
        float pz = ((x1 * z2 - z1 * x2) * (z3 - z4) - (z1 - z2) * (x3 * z4 - z3 * x4)) / denom;

        intersection = new Vector3(px, 0f, pz);
        return true;
    }
}
