using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    Vector3 cameraDistance;

    private void FixedUpdate()
    {
        transform.position = target.position + cameraDistance;
    }
}