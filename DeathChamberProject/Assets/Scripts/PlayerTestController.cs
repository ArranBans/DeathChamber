using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestController : MonoBehaviour
{
    [Header("Inventory")]
    public int inventorySize;
    public List<Item> inventory = new List<Item>();
    [Header("Movement")]
    public float moveSpeed;
    public float turnSpeed;
    public float jumpForce;
    private Rigidbody rb;
    //[HideInInspector]
    public GameObject camobj;
    public Camera cam;
    public float _xRot;
    public float _yRot;
    public Quaternion camDRot;

    private void Start()
    {
        for(int x = 0; x < inventorySize; x++)
        {
            Item _newItem = new Item();
            inventory.Add(_newItem);
        }

        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float _xMovement = 0;
        float _zMovement = 0;

        #region InputHandling
        if (pInputs()[0])
        {
            _zMovement += 1;
        }
        if (pInputs()[1])
        {
            _xMovement -= 1;
        }
        if (pInputs()[2])
        {
            _zMovement -= 1;
        }
        if (pInputs()[3])
        {
            _xMovement += 1;
        }
        #endregion

        Vector3 moveVector = transform.TransformDirection(new Vector3(_xMovement, 0, _zMovement)) * moveSpeed;
        rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
    }
    private void Update()
    {


        _xRot -= Input.GetAxisRaw("Mouse Y") * turnSpeed;
        _yRot += Input.GetAxisRaw("Mouse X") * turnSpeed;
        
        _xRot = Mathf.Clamp(_xRot, -70f, 70f);
        //Debug.Log($"{_xRot}");

        camobj.transform.localRotation = Quaternion.Euler(_xRot, camobj.transform.localRotation.eulerAngles.y, camobj.transform.localRotation.eulerAngles.z);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, _yRot, transform.localRotation.eulerAngles.z);
    }

    private bool[] pInputs() // W, A, S, D, Space, Shift
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

        return _inputs;
    }
}
