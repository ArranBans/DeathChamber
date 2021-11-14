using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;

    public float moveSpeed = 5f;
    public float jumpspeed = 5;
    public float sprintSpeed = 1.5f;
    private bool[] inputs;
    public PlayerTestController ptController;
    private int tick = 0;

    public void Initialise(int _id, string _username, Vector3 _spawnPos)
    {
        id = _id;
        username = _username;

        inputs = new bool[6];
    }

    public void FixedUpdate()
    {

        ptController.GetInputs(inputs, moveSpeed, jumpspeed, sprintSpeed, tick);
        MovePlayer();
        //Debug.Log($"{inputs[0]},{inputs[1]}, {inputs[2]}, {inputs[3]}, ");
    }

    private void MovePlayer()
    {
        ServerSend.PlayerPosition(this, tick);
        ServerSend.PlayerRotation(this);
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation, int _tick)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
        tick = _tick;
    }
}
