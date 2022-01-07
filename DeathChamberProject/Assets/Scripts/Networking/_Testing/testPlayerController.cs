using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class testPlayerController : MonoBehaviour
{
    public Camera cam;
    public float camFov;
    public Transform camObj;
    public Transform bulletTransform;
    public float _xRot;
    public float _yRot;
    public float turnSpeed;
    public float moveSpeed;
    public float sprintSpeed;
    private Rigidbody rb;
    public PositionState ServerPositionState;
    public PositionState predictedState;
    public PositionState extrapolatedPos;
    public List<PositionState> ClientPositionStates = new List<PositionState>();
    private int tick = 1;
    public float interpolationSpeed;
    public Player player;
    public float interactDistance;
    public float collideHeight;
    public float colliderWidth;

    private void Start()
    {
        camFov = OptionsManager.instance.fov;
        cam = GetComponentInChildren<Camera>();
        cam.fieldOfView = camFov;
        rb = GetComponent<Rigidbody>();
        predictedState = new PositionState(transform.position, transform.rotation, tick);

        if(MenuOptions.instance.sens != 0)
        {
            turnSpeed = OptionsManager.instance.sens;
        }
        
    }

    private void FixedUpdate()
    {
        SendInputToServer(); 
        
        tick += 1;
    }

    private void Update()
    {
        
        

        if (player.pManager.paused)
        {
            
        }
        else if(player.inventoryOpen)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                player.CloseInventory();
            }
        }
        else if(player.pManager.GetComponent<ChatWindow>().chatWindowOpen)
        {

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                player.OpenInventory();
            }

            _xRot -= Input.GetAxisRaw("Mouse Y") * (turnSpeed/10);
            _yRot += Input.GetAxisRaw("Mouse X") * (turnSpeed/10);

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                player.ChangeSelectedItem(0);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                player.ChangeSelectedItem(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                player.ChangeSelectedItem(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                player.ChangeSelectedItem(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                player.ChangeSelectedItem(4);
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                DropItem();
            }

            if (InteractRaycast())
            {
                player.InteractCanvas.gameObject.SetActive(true);
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Interact();
                    Debug.Log("item interacted with");
                }
            }
            else
            {
                player.InteractCanvas.gameObject.SetActive(false);
            }
        }

        

        _xRot = Mathf.Clamp(_xRot, -70f, 70f);
        //Debug.Log($"{_xRot}");
        camObj.localRotation = Quaternion.Euler(_xRot, cam.transform.localRotation.eulerAngles.y, cam.transform.localRotation.eulerAngles.z);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, _yRot, transform.localRotation.eulerAngles.z);
        transform.position = Vector3.Lerp(transform.position, predictedState.position, interpolationSpeed * 15 * Time.deltaTime);
    }



    private void DropItem()
    {
        player.DropItem(player.selectedItem);
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

        if (player.pManager.paused)
        {
            _inputs = new bool[6];
        }
        else if (player.pManager.GetComponent<ChatWindow>().chatWindowOpen)
        {
            _inputs = new bool[6];
        }

        //Debug.Log($"Sent Server ({_inputs[0]}, {_inputs[1]}, {_inputs[2]}, {_inputs[3]})");
        S_PlayerMovement(_inputs, tick);
        predictedState.rotation = transform.rotation;
        PositionState _newState = PredictMovement(_inputs, predictedState, tick);
        ClientPositionStates.Add(new PositionState(_newState.position, _newState.rotation, _newState.tick, _inputs));
        //Debug.Log($"{_preState.position}, {_preState.tick},  {_preState.inputs[0]}, {_preState.inputs[1]}, {_preState.inputs[2]}, {_preState.inputs[3]}");
        predictedState = _newState;
        //transform.position = predictedState.position;
    }
    private PositionState PredictMovement(bool[] _pInputs, int _tick)//unused
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

        Vector3 moveVector = transform.rotation * new Vector3(_xMovement, 0, _zMovement) * moveSpeed;
        rb.MovePosition(new Vector3(rb.position.x + moveVector.x, rb.position.y, rb.position.z + moveVector.z));
        return new PositionState(rb.position, rb.rotation, _tick, _pInputs);
    }

    private Vector3 PredictMovement(bool[] _pInputs, PositionState _pState)//serverpos
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

        Vector3 moveVector = _pState.rotation * new Vector3(_xMovement, 0, _zMovement) * moveSpeed;
        Vector3 newPos = new Vector3(_pState.position.x + moveVector.x, _pState.position.y, _pState.position.z + moveVector.z);
        return newPos;
    }

    private PositionState PredictMovement(bool[] _pInputs, PositionState _pState, int _tick)//clientsideprediction
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


          Vector3 moveVector = _pState.rotation * new Vector3(_xMovement, 0, _zMovement) * moveSpeed;
          Vector3 newPos = new Vector3(_pState.position.x + moveVector.x, _pState.position.y, _pState.position.z + moveVector.z);
        //rb.MovePosition(new Vector3(_pState.position.x + moveVector.x, rb.position.y, _pState.position.z + moveVector.z));
        return new PositionState(newPos, _pState.rotation, _tick, _pInputs);
        //return new PositionState(rb.position, rb.rotation, _tick, _pInputs);

    }

    public void OnServerState(PositionState _ServerState)
    {
        predictedState = _ServerState;
        foreach (PositionState posState in ClientPositionStates)
        {
            if(posState.tick >= _ServerState.tick)//<<<<-_---
            {
                PositionState newState = new PositionState(new Vector3(0, 0, 0), new Quaternion(), tick); // <<
                newState.position = PredictMovement(posState.inputs, predictedState); // <<
                predictedState.position = newState.position; // <<
            }
            
            //Debug.Log($"{predictedState.rotation.eulerAngles.x}, {predictedState.rotation.eulerAngles.y}, {predictedState.rotation.eulerAngles.z}");
        }

        var removeCount = ClientPositionStates.Count;
        for(int x = 0; x < removeCount; x++)
        {
            if (ClientPositionStates[x].tick <= _ServerState.tick)
            {
                ClientPositionStates.RemoveAt(0);
                break;
            }
        }
    }

    private bool InteractRaycast()
    {
        Vector3 rayVector = cam.transform.rotation * Vector3.forward * interactDistance;
        Ray ray = new Ray(cam.transform.position, rayVector);
        RaycastHit hit;

        Debug.DrawRay(cam.transform.position, rayVector);

        if (Physics.Raycast(ray, out hit, rayVector.magnitude))//PHYsics.raycast business
        {
            //Debug.DrawLine(cam.transform.position, hit.point, Color.red);

            if (hit.collider.gameObject.GetComponent<ItemPickup>() || hit.collider.gameObject.GetComponentInParent<ItemPickup>())
            {
                //Debug.Log("caninteractwithitem");
                return true;
                
            }
            else if (hit.collider.gameObject.GetComponent<Interactable>() || hit.collider.gameObject.GetComponentInParent<Interactable>())
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }
        else
        {
            return false;
        }
    }

    private void Interact()
    {
        S_Interact();
    }

    #region Messages
    private static void S_PlayerMovement(bool[] _inputs, int _tick)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ClientToServerId.playeMovement);

        message.Add(_inputs.Length);
        foreach (bool _input in _inputs)
        {
            message.Add(_input);
        }

        message.Add(testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.transform.rotation);
        message.Add(testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<testPlayerController>().cam.transform.rotation);

        message.Add(_tick);

        NetworkManager.instance.Client.Send(message);
    }

    private static void S_Interact()
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ClientToServerId.interact);
        NetworkManager.instance.Client.Send(message);
    }

    [MessageHandler((ushort)ServerToClientId.playerPosition)]
    private static void R_PlayerPosition(Message message)
    {
        int _id = message.GetInt();
        Vector3 _position = message.GetVector3();
        Quaternion _rotation = message.GetQuaternion();
        int _tick = message.GetInt();

        bool[] _inputs = new bool[message.GetInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = message.GetBool();
        }

        if (!testPlayerManager.list[(ushort)_id].playerObj)
        {
            return;
        }

        if (_id == NetworkManager.instance.Client.Id)
        {
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<testPlayerController>().OnServerState(new PositionState(_position, _rotation, _tick));
        }
        else
        {
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().DesiredPos = _position;
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().DesiredRot = _rotation;
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().PlayerMoved(_inputs);
        }
    }

    [MessageHandler((ushort)ServerToClientId.playerRotation)]
    private static void R_PlayerRotation(Message message)
    {
        int _id = message.GetInt();
        Quaternion _camRotation = message.GetQuaternion();

        if (_id != NetworkManager.instance.Client.Id)
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().camTransform.rotation = _camRotation;
    }
    #endregion
}




public class PositionState
{
    public int tick;
    public bool[] inputs;
    public Vector3 position;
    public Quaternion rotation;

    public PositionState(Vector3 _position, Quaternion _rotation, int _tick)
    {
        position = _position;
        rotation = _rotation;
        tick = _tick;
    }
    public PositionState(Vector3 _position, Quaternion _rotation, int _tick, bool[] _inputs)
    {
        position = _position;
        rotation = _rotation;
        tick = _tick;
        inputs = _inputs;
    }
}
