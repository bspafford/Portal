using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camCircle : MonoBehaviour
{
    [SerializeField]
    Vector3 centerLoc;
    [SerializeField]
    float radius = 5;
    [SerializeField]
    float camHeight;

    private float time = 0;
    [SerializeField]
    private float timer = 5;
    private bool timerGoing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k"))
            timerGoing = true;

        if (timerGoing)
        {
            time += Time.deltaTime;
            if (time >= timer)
            {
                timerGoing = false;
                time = 0;
            }
        }

        float angle = time / timer * 2 * Mathf.PI;
        float x = Mathf.Cos(angle) * radius;
        float y = Mathf.Sin(angle) * radius;

        transform.position = centerLoc + new Vector3(x, camHeight, y);
        transform.LookAt(centerLoc);
    }
}
