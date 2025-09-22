using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class portalTextureSetup : MonoBehaviour
{
    public Camera cam1;
    public Material camMat1;
    public Camera cam2;
    public Material camMat2;

    // Start is called before the first frame update
    void Start()
    {
        SetupMaterials();
    }

    public void Setup(Camera cam1, Material camMat1, Camera cam2, Material camMat2)
    {
        this.cam1 = cam1;
        this.camMat1 = camMat1;
        this.cam2 = cam2;
        this.camMat2 = camMat2;
        SetupMaterials();
    }

    void SetupMaterials()
    {
        if (!cam1 || !cam2 || !camMat1 || !camMat2)
            return;

        if (cam1.targetTexture != null)
        {
            cam1.targetTexture.Release();
        }
        cam1.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camMat1.mainTexture = cam1.targetTexture;

        if (cam2.targetTexture != null)
        {
            cam2.targetTexture.Release();
        }
        cam2.targetTexture = new RenderTexture(Screen.width, Screen.height, 24);
        camMat2.mainTexture = cam2.targetTexture;
    }
}
