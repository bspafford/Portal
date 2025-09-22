using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pairPortals : MonoBehaviour
{
    private GameObject portal1;
    private GameObject portal2;

    // Start is called before the first frame update
    void Start()
    {
        portal1 = transform.GetChild(0).gameObject;
        portal2 = transform.GetChild(1).gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
