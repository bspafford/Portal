using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class followCharacter : MonoBehaviour
{
    [SerializeField]
    Transform character;
    [SerializeField]
    Transform portal1;
    [SerializeField]
    Transform portal2;

    void Update()
    {
        Vector3 localPos = portal1.InverseTransformPoint(character.position);
        localPos = new Vector3(-localPos.x, localPos.y, -localPos.z);
        transform.position = portal2.TransformPoint(localPos);

        Vector3 localDir = portal1.InverseTransformDirection(character.forward);
        localDir = new Vector3(-localDir.x, localDir.y, -localDir.z);
        transform.rotation = Quaternion.LookRotation(portal2.TransformDirection(localDir), Vector3.up);
    }
}
