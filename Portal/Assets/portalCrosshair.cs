using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class portalCrosshair : MonoBehaviour
{
    Image blueCrosshair;
    Image redCrosshair;

    [SerializeField]
    Sprite blueFill;
    [SerializeField]
    Sprite blueEmpty;
    [SerializeField]
    Sprite redFill;
    [SerializeField]
    Sprite redEmpty;

    [SerializeField]
    teleport portal1;
    [SerializeField]
    teleport portal2;

    // Start is called before the first frame update
    void Start()
    {
        blueCrosshair = transform.Find("blue").GetComponent<Image>();
        redCrosshair = transform.Find("red").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (portal1.placed)
            blueCrosshair.sprite = blueFill;
        else
            blueCrosshair.sprite = blueEmpty;

        if (portal2.placed)
            redCrosshair.sprite = redFill;
        else
            redCrosshair.sprite = redEmpty;
    }
}
