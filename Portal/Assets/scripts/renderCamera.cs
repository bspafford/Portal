using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using RenderPipeline = UnityEngine.Rendering.RenderPipelineManager;

public class renderCamera : MonoBehaviour
{
    [SerializeField]
    private GameObject[] portals = new GameObject[2];

    [SerializeField]
    private Camera portalCamera;

    [SerializeField]
    private int iterations = 10;

    [SerializeField]
    bool obliqueCull = true;

    private RenderTexture tempTexture1;
    private RenderTexture tempTexture2;

    private Camera mainCamera;

    public Camera overlayCam;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();

        tempTexture1 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
        tempTexture2 = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32);
    }

    private void Start()
    {
        if (portals[0])
            portals[0].GetComponent<Renderer>().material.mainTexture = tempTexture1;
        if (portals[1])
            portals[1].GetComponent<Renderer>().material.mainTexture = tempTexture2;
    }

    private void OnEnable()
    {
        RenderPipeline.beginCameraRendering += UpdateCamera;
    }

    private void OnDisable()
    {
        RenderPipeline.beginCameraRendering -= UpdateCamera;
    }

    void UpdateCamera(ScriptableRenderContext SRC, Camera camera)
    {
        if (camera == overlayCam)
            return;

        if (!portals[0] || !portals[1])
        {
            return;
        }

        if (portals[0].GetComponent<Renderer>().isVisible)
        {
            portalCamera.targetTexture = tempTexture1;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[0], portals[1], i, SRC);
            }
        }

        if (portals[1].GetComponent<Renderer>().isVisible)
        {
            portalCamera.targetTexture = tempTexture2;
            for (int i = iterations - 1; i >= 0; --i)
            {
                RenderCamera(portals[1], portals[0], i, SRC);
            }
        }
    }

    private void RenderCamera(GameObject inPortal, GameObject outPortal, int iterationID, ScriptableRenderContext SRC)
    {
        Transform inTransform = inPortal.transform;
        Transform outTransform = outPortal.transform;

        Transform cameraTransform = portalCamera.transform;
        cameraTransform.position = transform.position;
        cameraTransform.rotation = transform.rotation;

        for (int i = 0; i <= iterationID; ++i)
        {
            // position
            Vector3 relativePos = inTransform.InverseTransformPoint(cameraTransform.position);
            relativePos = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativePos;
            if (float.IsNaN(relativePos.x) || float.IsNaN(relativePos.y) || float.IsNaN(relativePos.z))
                return;
            if (float.IsNaN(outTransform.TransformPoint(relativePos).x) || float.IsNaN(outTransform.TransformPoint(relativePos).y) || float.IsNaN(outTransform.TransformPoint(relativePos).z))
                return;
            cameraTransform.position = outTransform.TransformPoint(relativePos);

            // rotate
            Quaternion relativeRot = Quaternion.Inverse(inTransform.rotation) * cameraTransform.rotation;
            relativeRot = Quaternion.Euler(0.0f, 180.0f, 0.0f) * relativeRot;
            cameraTransform.rotation = outTransform.rotation * relativeRot;
        }

        if (obliqueCull)
        {
            Vector4 clipPlaneCameraSpace = CameraSpacePlane(portalCamera, outTransform.position, -outTransform.forward);
            portalCamera.projectionMatrix = mainCamera.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }

        UniversalRenderPipeline.RenderSingleCamera(SRC, portalCamera);
    }
    
    Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign = 1.0f)
    {
        Vector3 offsetPos = pos + normal * 0.0f; // Push slightly forward to avoid clipping
        Matrix4x4 m = cam.worldToCameraMatrix;
        Vector3 cPos = m.MultiplyPoint(offsetPos);
        Vector3 cNormal = m.MultiplyVector(normal).normalized * sideSign;

        return new Vector4(cNormal.x, cNormal.y, cNormal.z, -Vector3.Dot(cPos, cNormal));
    }
}
