using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestController : MonoBehaviour
{
    [Header("Inventory")]
    public int inventorySize;
    //public List<Item> inventory = new List<Item>();
    [HideInInspector]public float moveSpeed;
    [HideInInspector]public float jumpForce;
    [HideInInspector] public float sprintSpeed;
    private int tick = 0;

    private Rigidbody rb;
    bool[] pInputs;

    private void Start()
    {
        for (int x = 0; x < inventorySize; x++)
        {
            //Item _newItem = new Item();
            //inventory.Add(_newItem);
        }

        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        float _xMovement = 0;
        float _zMovement = 0;

        #region InputHandling
        if (pInputs[0])
        {
            _zMovement += 1;
        }
        if (pInputs[1])
        {
            _xMovement -= 1;
        }
        if (pInputs[2])
        {
            _zMovement -= 1;
        }
        if (pInputs[3])
        {
            _xMovement += 1;
        }
        if (pInputs[5])
        {
            _zMovement *= sprintSpeed;
        }
        #endregion

        Vector3 moveVector = transform.TransformDirection(new Vector3(_xMovement, 0, _zMovement)) * moveSpeed;
        rb.MovePosition(new Vector3(rb.position.x + moveVector.x, rb.position.y, rb.position.z + moveVector.z));
    }
       
    public void GetInputs(bool[] _inputs, float _moveSpeed, float _jumpForce, float _sprintSpeed, int _tick)
    {
        pInputs = _inputs;
        moveSpeed = _moveSpeed;
        jumpForce = _jumpForce;
        sprintSpeed = _sprintSpeed;
        tick = _tick;
    }

}
