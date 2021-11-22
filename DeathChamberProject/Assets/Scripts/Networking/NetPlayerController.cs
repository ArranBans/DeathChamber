using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerController : MonoBehaviour
{
    public Vector3 DesiredPos;
    public Quaternion DesiredRot;
    public float interpolationRate;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, DesiredPos, interpolationRate * 15 * Time.deltaTime);
        transform.rotation = DesiredRot;
    }
}
