using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class openPortal : MonoBehaviour
{
    Renderer meshRenderer;
    Material distortionMat;

    float time = 0;
    float timer = 0.5f;
    bool timerGoing = false;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<Renderer>();
        meshRenderer.enabled = false;

        distortionMat = transform.parent.Find("background").GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            timerGoing = true;
            meshRenderer.enabled = true;
            transform.localScale = Vector3.one * Time.deltaTime;
        }
        else if (Input.GetKeyDown("i"))
        {
            timerGoing = false;
            time = 0;
            meshRenderer.enabled = false;
            distortionMat.SetFloat("_Radius", 0);
        }

        if (timerGoing)
        {
            time += Time.deltaTime;

            float percent = time / timer;

            distortionMat.SetFloat("_Radius", percent * 3);
            transform.localScale = Vector3.one * percent * percent;

            if (time >= timer)
            {
                timerGoing = false;
                time = 0;
            }
        }
    }
}
