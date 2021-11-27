using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
    public string username;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpspeed = 5;
    public float sprintSpeed = 1.5f;
    private bool[] inputs;
    public PlayerTestController ptController;
    public Transform camTransform;
    private int tick = 0;
    [Header("Inventory")]
    public List<Item> inventory = new List<Item>();
    public int inventorySize = 5;
    public List<Item> hotbar = new List<Item>();
    public int hotbarSize = 3;
    public int selectedItem = 0;
    public float interactDistance;

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

    public void SetInput(bool[] _inputs, Quaternion _rotation, Quaternion _camRotation, int _tick)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
        camTransform.rotation = _camRotation;
        tick = _tick;
    }

    public bool AddItemToInventory(ItemSO _item)
    {
        if (inventory.Count < inventorySize)
        {
            
            GameObject _invItemObj = Instantiate((GameObject)Resources.Load("Items/"+_item.ViewModelName), camTransform);
            _invItemObj.transform.localPosition = Vector3.zero;
            Item _invItem = _invItemObj.GetComponent<Item>();
            inventory.Add(_invItem);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveItemFromInventory(int _i)
    {
            inventory.RemoveAt(_i);
    }

    public bool InteractRaycast(out ItemPickup _item)
    {
        Vector3 rayVector = camTransform.rotation * Vector3.forward * interactDistance;
        Ray ray = new Ray(camTransform.position, rayVector);
        RaycastHit hit;

        Debug.DrawRay(camTransform.position, rayVector);

        if (Physics.Raycast(ray, out hit, rayVector.magnitude))//PHYsics.raycast business
        {
            //Debug.DrawLine(cam.transform.position, hit.point, Color.red);

            if (hit.collider.gameObject.GetComponent<ItemPickup>() || hit.collider.gameObject.GetComponentInParent<ItemPickup>())
            {
                Debug.Log($"client {id} interacted with {hit.collider.name}");
                _item = hit.collider.gameObject.GetComponent<ItemPickup>();
                return true;   
            }
            else
            {
                Debug.Log($"client {id} interacted with non item: {hit.collider.name}");
                _item = null;
                return false;
            }

        }
        else
        {
            Debug.Log($"client {id} interacted with nothing");
            _item = null;
            return false;
        }
        
            
        
    }
}
