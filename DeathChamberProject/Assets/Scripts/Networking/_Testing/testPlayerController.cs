using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPlayerController : MonoBehaviour
{
    private Camera cam;
    float _xRot;
    float _yRot;
    public float turnSpeed;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
    }

    private void FixedUpdate()
    {
        SendInputToServer();
    }
    private void Update()
    {
        _xRot -= Input.GetAxisRaw("Mouse Y") * turnSpeed;
        _yRot += Input.GetAxisRaw("Mouse X") * turnSpeed;

        _xRot = Mathf.Clamp(_xRot, -70f, 70f);
        Debug.Log($"{_xRot}");
        cam.transform.localRotation = Quaternion.Euler(_xRot, cam.transform.localRotation.eulerAngles.y, cam.transform.localRotation.eulerAngles.z);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, _yRot, transform.localRotation.eulerAngles.z);
    }

    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space),
            Input.GetKey(KeyCode.LeftShift)
        };

        //Debug.Log($"Sent Server ({_inputs[0]}, {_inputs[1]}, {_inputs[2]}, {_inputs[3]})");

        ClientSend.PlayerMovement(_inputs);
    }
}
