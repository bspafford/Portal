using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class removeSimulation : MonoBehaviour
{
    [SerializeField]
    GameObject[] walls = new GameObject[4];

    float timer = 1;
    float time = 0;
    bool timerGoing = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            timerGoing = true;
        }
        else if (Input.GetKeyDown("i"))
        {
            setThreshold(0);
        }

        if (timerGoing)
        {
            time += Time.deltaTime;

            float x = time / timer;
            float y = x * x;
            setThreshold(y);

            if (time >= timer)
            {
                timerGoing = false;
                time = 0;
            }
        }
    }

    void setThreshold(float y)
    {
        for (int i = 0; i < walls.Length; i++)
            walls[i].GetComponent<MeshRenderer>().material.SetFloat("_Threshold", y);
    }
    
}
