using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPlayerController : MonoBehaviour
{
    private void FixedUpdate()
    {
        SendInputToServer();
    }

    private void SendInputToServer()
    {
        bool[] _inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.D)
        };

        Debug.Log($"Sent Server ({_inputs[0]}, {_inputs[1]}, {_inputs[2]}, {_inputs[3]})");

        ClientSend.PlayerMovement(_inputs);
    }
}
