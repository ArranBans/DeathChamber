using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string msg = _packet.ReadString();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: {msg}");
        Client.instance.myId = _myId;
        ClientSend.WelcomeReceived();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);

        ClientSend.UDPTest("Hello Server!");
    }

    public static void SpawnPlayer(Packet _packet)
    {
        Debug.Log($"SpawnPlayerReceived");
        int _id = _packet.ReadInt();
        string _username = _packet.ReadString();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();

        testGameManager.instance.SpawnPlayer(_id, _username, _position, _rotation);

        if (_id != Client.instance.myId)
        {
            //ClientSend.ChangeSelectedItem(testGameManager.players[Client.instance.myId].GetComponent<Player>().selectedItem);
        }

        Debug.Log($"Spawning Player {_id}, {_username}");
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _position = _packet.ReadVector3();
        Quaternion _rotation = _packet.ReadQuaternion();
        int _tick = _packet.ReadInt();

        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }

        if (testGameManager.players[_id].playerObj.GetComponent<testPlayerController>() != null)
        {
            testGameManager.players[_id].playerObj.GetComponent<testPlayerController>().OnServerState(new PositionState(_position, _rotation, _tick));
        }
        else
        {
            testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().DesiredPos = _position;
            testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().DesiredRot = _rotation;
            //Debug.Log($"{_id}, Input: {_inputs[0]}, {_inputs[1]}, {_inputs[2]}, {_inputs[3]}");
            testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().PlayerMoved(_inputs);
        }
        
        
        //Debug.Log($"Packet ID: {_id} should hold position {_position}");
        //testGameManager.players[_id].transform.position = _position;
    }
    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();
        Quaternion _camRotation = _packet.ReadQuaternion();

        testGameManager.players[_id].playerObj.transform.rotation = _rotation;
        testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().camTransform.rotation = _camRotation;
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Destroy(testGameManager.players[_id].gameObject);
        testGameManager.players.Remove(_id);
    }

    public static void SpawnItems(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _databaseId = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        Debug.Log($"Spawning: {_databaseId}");
        GameObject _item = (GameObject)Instantiate(Database.instance.GetItem(_databaseId).empty, _pos, _rot);
        _item.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.pickup);
        _item.GetComponent<ItemPickup>().id = _id;
        testGameManager.itemPickups.Add(_item.GetComponent<ItemPickup>());
        //Debug.Log($"Spawned: {_databaseId}");
    }

    public static void ItemPosition(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        foreach(ItemPickup _item in testGameManager.itemPickups)
        {
            if(_item != null)
            {
                if (_item.id == _id)
                {
                    _item.UpdateItemState(_pos, _rot);
                }
            }
            
        }
    }

    public static void AddItemToInventory(Packet _packet)
    {
        int _ItemId = _packet.ReadInt();

        GameObject itemGO = (GameObject)Instantiate(Database.instance.GetItem(_ItemId).empty, testGameManager.players[Client.instance.myId].playerObj.GetComponent<testPlayerController>().cam.transform);
        itemGO.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.item);
        testGameManager.players[Client.instance.myId].playerObj.GetComponent<Player>().AddItemToInventory(itemGO.GetComponent<Item>());
        testGameManager.players[Client.instance.myId].playerObj.GetComponent<Player>().ChangeSelectedItem(testGameManager.players[Client.instance.myId].playerObj.GetComponent<Player>().inventory.IndexOf(itemGO.GetComponent<Item>()));


        //Debug.Log("Adding item to inventory");
    }

    public static void RemoveItemPickup(Packet _packet)
    {
        int _id = _packet.ReadInt();

        foreach (ItemPickup _iPickup in testGameManager.itemPickups)
        {
            if (_iPickup != null)
            {
                if (_iPickup.id == _id)
                {
                    Destroy(_iPickup.gameObject);
                    testGameManager.itemPickups[testGameManager.itemPickups.IndexOf(_iPickup)] = null;
                }

            }
        }
    }

    public static void RemoveItemFromInventory(Packet _packet)
    {
        int _index = _packet.ReadInt();
        bool _clear = _packet.ReadBool();
        if(_clear)
        {
            foreach(Item i in testGameManager.players[Client.instance.myId].playerObj.GetComponent<Player>().inventory)
            {
                Destroy(i);
            }

            testGameManager.players[Client.instance.myId].playerObj.GetComponent<Player>().inventory = new List<Item>();
            return;
        }
        testGameManager.players[Client.instance.myId].playerObj.GetComponent<Player>().RemoveItemFromInventory(_index);
    }

    public static void ChangeSelectedItem(Packet _packet)
    {
        int _id = _packet.ReadInt();
        int _itemId = _packet.ReadInt();

        Debug.Log($"{_id} changing item to {_itemId}");

        if(testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().selectedItem != null)
        {
            Destroy(testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().selectedItem);
        }
        
        if(_itemId != 0)
        {
            testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().selectedItem = (GameObject)Instantiate(Database.instance.GetItem(_itemId).empty, testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().camTransform);
        }
        
        
    }

    public static void FireWeapon(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _name = _packet.ReadString();

        
        Instantiate(Resources.Load($"Projectiles/{_name}_Projectile"), testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().camTransform.position, testGameManager.players[_id].playerObj.GetComponent<NetPlayerController>().camTransform.rotation);
    }

    public static void ChangeHealth(Packet _packet)
    {
        float _value = _packet.ReadFloat();

        Player p = testGameManager.players[Client.instance.myId].playerObj.GetComponent<Player>();
        p.health = _value;
        p.healthSlider.value = _value;

    }

    public static void Die(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Debug.Log($"Player {_id} has died");
        
        if(_id == Client.instance.myId)
        {
            testGameManager.players[_id].playerObj.GetComponent<Player>().Die();
        }
        else
        {
            testGameManager.players[_id].playerObj.SetActive(false);
        }
    }

    public static void Respawn(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Debug.Log($"Player {_id} has respawned");

        if (_id == Client.instance.myId)
        {
            testGameManager.players[_id].playerObj.GetComponent<Player>().Respawn();
        }
        else
        {
            testGameManager.players[_id].playerObj.SetActive(true);
        }
    }

}
