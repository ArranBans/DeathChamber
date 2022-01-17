using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
        S_PlayerPosition(this, tick, inputs);
        S_PlayerRotation(this);
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

    public bool AddItemToInventory(int _item, int _aux1, int _aux2)
    {
        if (inventory.Count < inventorySize)
        {

            GameObject _invItemObj = Instantiate(Database.instance.GetItem(_item).empty, camTransform);

            _invItemObj.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.item, _aux1, _aux2);
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

        Item _item = inventory[_i];
        Destroy(inventory[_i].gameObject);
        inventory.RemoveAt(_i);
        S_RemoveItemFromInventory(pManager.id, _i, _clear);
    }
    public void SetHealth(float _value)
    {
        health = Mathf.Clamp(_value, -1, maxHealth);
        S_ChangeHealth(pManager.id, _value);

        if(health <= 0)
        {
            //Destroy(Server.clients[id].player.gameObject);
            
            foreach (Item i in inventory)
            {
                if(i != null)
                {
                    int aux1 = 0, aux2 = 0;
                    if(i.itemSO.itemType == ItemSO.ItemType.gun)
                    {
                        aux1 = ((GunInfo)i.itemInfo).magAmmo;
                        aux2 = ((GunInfo)i.itemInfo).reserveAmmo;
                    }
                    testGameManager.instance.SpawnItem(i.itemSO.id, dropTransform.position, transform.rotation, aux1, aux2);
                }       
            }

            S_Die(pManager.id);
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
    }

    #region Messages
    public void S_PlayerPosition(Player _player, int _tick, bool[] _inputs)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.playerPosition);
        message.AddInt(_player.pManager.id);
        message.AddVector3(_player.transform.position);
        message.AddQuaternion(_player.transform.rotation);
        message.AddInt(_tick);

        message.AddInt(_inputs.Length);
        foreach (bool _input in _inputs)
        {
            message.AddBool(_input);
        }

        NetworkManager.instance.Server.SendToAll(message);
    }
    public void S_PlayerRotation(Player _player)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.playerRotation);
        message.AddInt(_player.pManager.id);
        message.AddQuaternion(_player.camTransform.rotation);

        NetworkManager.instance.Server.SendToAll(message);
    }
    //inventory
    public static void S_AddItemToInventory(int _id, int _itemId, int _aux1, int _aux2)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.addItemToInventory);
        message.AddInt(_itemId);
        message.AddInt(_aux1);
        message.AddInt(_aux2);

        NetworkManager.instance.Server.Send(message, (ushort)_id);
    }
    public void S_RemoveItemFromInventory(ushort _id, int _index, bool _clear)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.removeItemFromInventory);
        message.AddInt(_index);
        message.AddBool(_clear);

        NetworkManager.instance.Server.Send(message, _id);
    }

    public static void S_ChangeSelectedItem(int _id, int _item, int _aux1, int _aux2)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.changeSelectedItem);
        message.Add(_id);
        message.Add(_item);
        message.Add(_aux1);
        message.Add(_aux2);

        foreach (PlayerManager p in PlayerManager.list.Values)
        {
            if(p.id != _id)
             NetworkManager.instance.Server.Send(message, p.id);
        }
    }

    public static void S_FireWeapon(ushort _id, int _weaponId)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.fireWeapon);
        message.AddUShort(_id);
        message.AddInt(_weaponId);

        foreach (PlayerManager p in PlayerManager.list.Values)
        {
            if (p.id != _id)
                NetworkManager.instance.Server.Send(message, p.id);
        }
    }

    public static void S_ChangeHealth(ushort _id, float _value)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.changeHealth);
        message.AddFloat(_value);

        NetworkManager.instance.Server.Send(message, _id);
    }

    public static void S_Die(ushort _id)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.die);
        message.AddUShort(_id);

        NetworkManager.instance.Server.SendToAll(message);
    }

    [MessageHandler((ushort)ClientToServerId.playeMovement)]
    private static void R_PlayerMovement(ushort _fromClient, Message message)
    {
        bool[] _inputs = new bool[message.GetInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = message.GetBool();
        }
        Quaternion _rotation = message.GetQuaternion();
        Quaternion _camRotation = message.GetQuaternion();
        int _tick = message.GetInt();

        PlayerManager.list[_fromClient].player.SetInput(_inputs, _rotation, _camRotation, _tick);
    }

    [MessageHandler((ushort)ClientToServerId.interact)]
    private static void R_Interact(ushort _fromClient, Message message)
    {
        ItemPickup _item;
        GameObject _object;

        if (PlayerManager.list[_fromClient].player.InteractRaycast(out _object))
        {
            if (_object.GetComponent(typeof(IInteractable)))
            {
                IInteractable i = (IInteractable)_object.GetComponent(typeof(IInteractable));
                i.Interacted();
            }
            else if (_object.GetComponent<ItemPickup>())
            {

                _item = _object.GetComponent<ItemPickup>();

                int _aux1 = 0;
                int _aux2 = 0;
                if (Database.instance.GetItem(_item.iSO.id).itemType == ItemSO.ItemType.gun)
                {
                    _aux1 = ((GunInfo)_item.GetComponent<GunInfo>()).magAmmo;
                    _aux2 = ((GunInfo)_item.GetComponent<GunInfo>()).reserveAmmo;
                }

                if (PlayerManager.list[_fromClient].player.AddItemToInventory(_item.iSO.id, _aux1, _aux2))
                {

                    S_AddItemToInventory(_fromClient, _item.iSO.id, _aux1, _aux2);
                    testGameManager.instance.S_RemoveItem(_item.id);
                    testGameManager.instance.items[testGameManager.instance.items.IndexOf(_item)] = null;
                    Destroy(_item.gameObject);
                }
            }


        }
    }

    [MessageHandler((ushort)ClientToServerId.changeSelectedItem)]
    private static void R_ChangeSelectedItem(ushort _fromClient, Message message)
    {
        int _index = message.GetInt();

        PlayerManager.list[_fromClient].player.ChangeSelectedItem(_index);

        if (_index + 1 <= PlayerManager.list[_fromClient].player.inventory.Count)
        {



            if (PlayerManager.list[_fromClient].player.inventory[_index] != null)
            {
                int _aux1 = 0;
                int _aux2 = 0;
                if (Database.instance.GetItem(PlayerManager.list[_fromClient].player.inventory[_index].itemSO.id).itemType == ItemSO.ItemType.gun)
                {
                    _aux1 = ((GunSO)Database.instance.GetItem(PlayerManager.list[_fromClient].player.inventory[_index].itemSO.id)).magAmmo;
                    _aux2 = ((GunSO)Database.instance.GetItem(PlayerManager.list[_fromClient].player.inventory[_index].itemSO.id)).maxAmmo;
                }

                Debug.Log($"Player {_fromClient} changed item to {_index}");
                S_ChangeSelectedItem(_fromClient, PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO.id, _aux1, _aux2);
            }
            else
            {
                S_ChangeSelectedItem(_fromClient, PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO.id, 0, 0);
            }
        }
        else
        {
            S_ChangeSelectedItem(_fromClient, 0, 0, 0);
        }

    }

    [MessageHandler((ushort)ClientToServerId.dropItem)]
    private static void R_DropItem(ushort _fromClient, Message message)
    {
        int _index = message.GetInt();
        int _id = PlayerManager.list[_fromClient].player.inventory[_index].itemSO.id;

        int aux1 = ((GunInfo)PlayerManager.list[_fromClient].player.inventory[_index].itemInfo).magAmmo;
        int aux2 = ((GunInfo)PlayerManager.list[_fromClient].player.inventory[_index].itemInfo).reserveAmmo;

        PlayerManager.list[_fromClient].player.RemoveItemFromInventory(_index, false);
        testGameManager.instance.SpawnItem(_id, PlayerManager.list[_fromClient].player.dropTransform.position, PlayerManager.list[_fromClient].player.transform.rotation, aux1, aux2);
    }

    [MessageHandler((ushort)ClientToServerId.fireWeapon)]
    private static void R_FireWeapon(ushort _fromClient, Message message)
    {
        bool _aiming = message.GetBool();
        float _gunXRot = message.GetFloat();
        float _gunYRot = message.GetFloat();

        if (_aiming)
        {
            PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].transform.localPosition = ((GunSO)PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO).aimPos;
        }
        else
        {
            PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].transform.localPosition = ((GunSO)PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO).hipPos;
        }

        GameObject bullet = (GameObject)GameObject.Instantiate(Resources.Load($"Projectiles/{PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO.ItemName}_Projectile"), PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].transform.TransformPoint(((GunSO)PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO).bulletSpawnPoint), Quaternion.Euler(_gunXRot, _gunYRot, PlayerManager.list[_fromClient].player.camTransform.rotation.eulerAngles.z));
        S_FireWeapon(_fromClient, PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO.id);
        
        if(((GunInfo)PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemInfo).magAmmo > 0)
            ((GunInfo)PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemInfo).magAmmo -= 1;
    }

    [MessageHandler((ushort)ClientToServerId.consumable)]
    private static void R_Consumable(ushort _fromClient, Message message)
    {
        Consumable con = (Consumable)PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem];
        PlayerManager.list[_fromClient].player.SetHealth(PlayerManager.list[_fromClient].player.health + con.conSO.value);
        PlayerManager.list[_fromClient].player.RemoveItemFromInventory(PlayerManager.list[_fromClient].player.selectedItem, false);
        int newItem = PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO.id;

        int _aux1 = 0;
        int _aux2 = 0;

        if (Database.instance.GetItem(newItem).itemType == ItemSO.ItemType.gun)
        {
            _aux1 = ((GunSO)Database.instance.GetItem(newItem)).magAmmo;
            _aux2 = ((GunSO)Database.instance.GetItem(newItem)).maxAmmo;
        }

        S_ChangeSelectedItem(_fromClient, newItem, _aux1, _aux2);
    }

    [MessageHandler((ushort)ClientToServerId.command)]
    private static void R_Command(ushort _fromClient, Message message)
    {
        int _commandType = message.GetInt();
        int _index = message.GetInt();

        if (_commandType == 0)
        {
            if (PlayerManager.list[_fromClient].player)
            {
                if (PlayerManager.list[_fromClient].player.AddItemToInventory(_index))
                {
                    int _aux1 = 0;
                    int _aux2 = 0;

                    if (Database.instance.GetItem(_index).itemType == ItemSO.ItemType.gun)
                    {
                        _aux1 = ((GunSO)Database.instance.GetItem(_index)).magAmmo;
                        _aux2 = ((GunSO)Database.instance.GetItem(_index)).maxAmmo;
                    }

                    S_AddItemToInventory(_fromClient, _index, _aux1, _aux2);
                }

            }
        }
        if (_commandType == 1)
        {
            if (PlayerManager.list[_fromClient].player)
            {
                testGameManager.instance.SpawnItem(_index, PlayerManager.list[_fromClient].player.dropTransform.position, PlayerManager.list[_fromClient].player.transform.rotation);

            }
        }
        if (_commandType == 2)
        {
            if (PlayerManager.list[_fromClient].player)
            {
                PlayerManager.list[_fromClient].player.transform.position = PlayerManager.list[(ushort)_index].player.transform.position;

            }
        }
        if (_commandType == 3)
        {
            if (PlayerManager.list[_fromClient].player)
            {
                Ray ray = new Ray(PlayerManager.list[_fromClient].player.camTransform.position, PlayerManager.list[_fromClient].player.camTransform.forward);
                RaycastHit hitInfo;

                if(Physics.Raycast(ray, out hitInfo))
                {
                    Debug.Log($"Summoning Enemy {_index} at {hitInfo.point}");
                    testGameManager.instance.SpawnEnemy(_index, hitInfo.point, Quaternion.identity);
                }

            }
        }
    }
    #endregion
}
