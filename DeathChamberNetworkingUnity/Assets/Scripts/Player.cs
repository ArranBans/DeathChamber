using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{/*
    public PlayerManager pManager;
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpspeed = 5;
    public float sprintSpeed = 1.5f;
    public bool[] inputs;
    public PlayerTestController ptController;
    public Transform camTransform;
    public Transform bulletTransform;
    public Transform dropTransform;
    private int tick = 0;
    [Header("Inventory")]
    public List<Item> inventory = new List<Item>();
    public int inventorySize = 5;
    public List<Item> hotbar = new List<Item>();
    public int hotbarSize = 3;
    public int selectedItem = 0;
    public float interactDistance;
    [Header("Health")]
    public float maxHealth;
    public float health;
    public GameObject capsule;
    public Transform enemyTarget;

    public void Initialise()
    {
        inputs = new bool[6];
        health = maxHealth;
    }

    public void FixedUpdate()
    {

        ptController.GetInputs(inputs, moveSpeed, jumpspeed, sprintSpeed, tick);
        MovePlayer();
        //Debug.Log($"{inputs[0]},{inputs[1]}, {inputs[2]}, {inputs[3]}, ");
    }

    private void MovePlayer()
    {
        ServerSend.PlayerPosition(this, tick, inputs);
        ServerSend.PlayerRotation(this);
        //Debug.Log($"{pManager.id} moved and rotated");
    }

    public void SetInput(bool[] _inputs, Quaternion _rotation, Quaternion _camRotation, int _tick)
    {
        inputs = _inputs;
        transform.rotation = _rotation;
        camTransform.rotation = _camRotation;
        tick = _tick;
    }

    public bool AddItemToInventory(int _item)
    {
        if (inventory.Count < inventorySize)
        {
            
            GameObject _invItemObj = Instantiate(Database.instance.GetItem(_item).empty, camTransform);
            int _aux1 = 0;
            int _aux2 = 0;
            if (Database.instance.GetItem(_item).itemType == ItemSO.ItemType.gun)
            {
                _aux1 = ((GunSO)Database.instance.GetItem(_item)).magAmmo;
                _aux2 = ((GunSO)Database.instance.GetItem(_item)).maxAmmo;
            }

            _invItemObj.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.item,_aux1,_aux2);
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

    public void ChangeSelectedItem(int _index)
    {
        selectedItem = _index;

        foreach (Item i in inventory)
        {
            if (inventory.IndexOf(i) == selectedItem)
            {
                i.gameObject.SetActive(true);
            }
            else
            {
                i.gameObject.SetActive(false);
            }
        }

    }

    public void RemoveItemFromInventory(int _i, bool _clear)
    {
        if(_clear)
        {
            foreach(Item i in inventory)
            {
                Destroy(i);
            }
            inventory = new List<Item>();
            return;
        }

        Debug.Log($"Item: {_i} removed from inventory of {pManager.id}");
        Item _item = inventory[_i];
        Destroy(inventory[_i].gameObject);
        //Destroy(inventory[_i]);
        inventory.RemoveAt(_i);
        ServerSend.RemoveItemFromInventory(pManager.id, _i, _clear);
    }
    public void SetHealth(float _value)
    {
        health = Mathf.Clamp(_value, -1, maxHealth);
        ServerSend.ChangeHealth(pManager.id, _value);

        if(health <= 0)
        {
            //Destroy(Server.clients[id].player.gameObject);
            
            foreach (Item i in inventory)
            {
                if(i != null)
                {
                    testGameManager.instance.SpawnItem(i.itemSO.id, dropTransform.position, transform.rotation);
                }       
            }

            ServerSend.Die(pManager.id);
            Debug.Log($"player {pManager.id} has died");
            //testGameManager.instance.Respawner(pManager.id);
            Destroy(gameObject);
        }
    }

    public bool InteractRaycast(out GameObject _object)
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
                Debug.Log($"client {pManager.id} interacted with pickup {hit.collider.name}");
                _object = hit.collider.gameObject;
                return true;   
            }
            else if(hit.collider.gameObject.GetComponent(typeof(IInteractable)))
            {
                Debug.Log($"client {pManager.id} interacted with interactable {hit.collider.name}");
                _object = hit.collider.gameObject;
                return true;
            }
            else if(hit.collider.gameObject.GetComponentInParent(typeof(IInteractable)))
            {
                Debug.Log($"client {pManager.id} interacted with interactable {hit.collider.name}");
                _object = hit.collider.gameObject;
                return true;
            }
            else
            {
                Debug.Log($"client {pManager.id} interacted with non item: {hit.collider.name}");
                _object = null;
                return false;
            }

        }
        else
        {
            Debug.Log($"client {pManager.id} interacted with nothing");
            _object = null;
            return false;
        }
        
            
        
    }*/
}
