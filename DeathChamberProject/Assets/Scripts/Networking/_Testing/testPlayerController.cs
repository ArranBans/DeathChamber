using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testPlayerController : MonoBehaviour
{
    private Camera cam;
    float _xRot;
    float _yRot;
    public float turnSpeed;
    public float moveSpeed;
    public float sprintSpeed;
    private Rigidbody rb;
    public PositionState ServerPositionState;
    public List<PositionState> ClientPositionStates = new List<PositionState>();
    private int tick = 0;

    private void Start()
    {
        cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        SendInputToServer();
        //rb.position = 
        ApplyServerMovement();//.position;
        //transform.position = ServerPositionStates[ServerPositionStates.Count-1].position;
        Debug.Log($"server tick: {ServerPositionState.tick}");
        Debug.Log($"local tick: {tick}");
        tick += 1;
    }
    private void Update()
    {
        _xRot -= Input.GetAxisRaw("Mouse Y") * turnSpeed;
        _yRot += Input.GetAxisRaw("Mouse X") * turnSpeed;

        _xRot = Mathf.Clamp(_xRot, -70f, 70f);
        //Debug.Log($"{_xRot}");
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
        ClientSend.PlayerMovement(_inputs, tick);
        ClientPositionStates.Add(PredictMovement(_inputs, tick));
        //foreach(PositionState x in ClientPositionStates)
        //{
            //Debug.Log($"{x.position}, {x.tick},  {x.inputs}");
        //}
    }
    private PositionState PredictMovement(bool[] _pInputs, int _tick)
    {
        float _xMovement = 0;
        float _zMovement = 0;

        #region InputHandling
        if (_pInputs[0])
        {
            _zMovement += 1;
        }
        if (_pInputs[1])
        {
            _xMovement -= 1;
        }
        if (_pInputs[2])
        {
            _zMovement -= 1;
        }
        if (_pInputs[3])
        {
            _xMovement += 1;
        }
        if (_pInputs[5])
        {
            _zMovement *= sprintSpeed;
        }
        #endregion

        Vector3 moveVector = transform.TransformDirection(new Vector3(_xMovement, 0, _zMovement)) * moveSpeed;
        rb.MovePosition(new Vector3(rb.position.x + moveVector.x, rb.position.y, rb.position.z + moveVector.z));
        return new PositionState(rb.position, _tick, _pInputs);
    }
    private Vector3 PredictMovement(bool[] _pInputs, Vector3 _position)
    {
        float _xMovement = 0;
        float _zMovement = 0;

        #region InputHandling
        if (_pInputs[0])
        {
            _zMovement += 1;
        }
        if (_pInputs[1])
        {
            _xMovement -= 1;
        }
        if (_pInputs[2])
        {
            _zMovement -= 1;
        }
        if (_pInputs[3])
        {
            _xMovement += 1;
        }
        if (_pInputs[5])
        {
            _zMovement *= sprintSpeed;
        }
        #endregion

        Vector3 moveVector = transform.TransformDirection(new Vector3(_xMovement, 0, _zMovement)) * moveSpeed;
        rb.MovePosition(new Vector3(_position.x + moveVector.x, rb.position.y, _position.z + moveVector.z));
        return rb.position;
    }

    public PositionState ApplyServerMovement() //BROOOOOOOOOKKKKKKEEEEEEEEEENNNNNNNNN
    {
        Vector3 _predictedState = ServerPositionState.position;
        //Debug.Log($"{_predictedState}");
        for (int x = ServerPositionState.tick; x < ClientPositionStates.Count; x++)
        {
            //Debug.Log($"{ClientPositionStates[x].position}, {ClientPositionStates[x].tick},  {ClientPositionStates[x].inputs}");
            _predictedState = PredictMovement(ClientPositionStates[x].inputs, _predictedState);//this line broken <------
        }
        for (int x = 0; x < ClientPositionStates.Count; x++)
        {
            if (ClientPositionStates[x].tick == tick)
            {
                ClientPositionStates.Remove(ClientPositionStates[x]);
            }
        }
        ClientPositionStates.Add(new PositionState(_predictedState, tick));
        return new PositionState(_predictedState, tick);
    }
}

public class PositionState
{
    public Vector3 position;
    public int tick;
    public bool[] inputs;
    public PositionState(Vector3 _position, int _tick)
    {
        position = _position;
        tick = _tick;
    }
    public PositionState(Vector3 _position, int _tick, bool[] _inputs)
    {
        position = _position;
        tick = _tick;
        inputs = _inputs;
    }
}
