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

        if(testGameManager.players[_id].GetComponent<testPlayerController>() != null)
        {
            testGameManager.players[_id].GetComponent<testPlayerController>().OnServerState(new PositionState(_position, _rotation, _tick));
        }
        else
        {
            testGameManager.players[_id].GetComponent<NetPlayerController>().DesiredPos = _position;
            testGameManager.players[_id].GetComponent<NetPlayerController>().DesiredRot = _rotation;
        }
        
        
        //Debug.Log($"Packet ID: {_id} should hold position {_position}");
        //testGameManager.players[_id].transform.position = _position;
    }
    public static void PlayerRotation(Packet _packet)
    {
        int _id = _packet.ReadInt();
        Quaternion _rotation = _packet.ReadQuaternion();
        Quaternion _camRotation = _packet.ReadQuaternion();

        testGameManager.players[_id].transform.rotation = _rotation;
        testGameManager.players[_id].GetComponent<NetPlayerController>().camTransform.rotation = _camRotation;
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
        string _name = _packet.ReadString();
        Vector3 _pos = _packet.ReadVector3();
        Quaternion _rot = _packet.ReadQuaternion();

        Debug.Log($"Spawning: {_name}");
        GameObject _item = (GameObject)Instantiate(Resources.Load($"Item Pickups/{_name}_Pickup"), _pos, _rot);
        _item.GetComponent<ItemPickup>().id = _id;
        testGameManager.itemPickups.Add(_item.GetComponent<ItemPickup>());
        Debug.Log($"Spawned: {_name}");
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
                    _item.gameObject.transform.position = _pos;
                    _item.gameObject.transform.rotation = _rot;
                }
            }
            
        }
    }

    public static void AddItemToInventory(Packet _packet)
    {
        string _ItemName = _packet.ReadString();
        int _PickupId = _packet.ReadInt();

        GameObject itemGO = (GameObject)Instantiate(Resources.Load($"Item Viewmodels/{_ItemName}_Item"), testGameManager.players[Client.instance.myId].GetComponent<testPlayerController>().cam.transform);

        testGameManager.players[Client.instance.myId].GetComponent<Player>().AddItemToInventory(itemGO.GetComponent<Item>());
        testGameManager.players[Client.instance.myId].GetComponent<Player>().ChangeSelectedItem(testGameManager.players[Client.instance.myId].GetComponent<Player>().inventory.IndexOf(itemGO.GetComponent<Item>()));


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
        testGameManager.players[Client.instance.myId].GetComponent<Player>().RemoveItemFromInventory(_index);
    }

    public static void ChangeSelectedItem(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _name = _packet.ReadString();

        Debug.Log($"{_id} changing item to {_name}");

        if(testGameManager.players[_id].GetComponent<NetPlayerController>().selectedItem != null)
        {
            Destroy(testGameManager.players[_id].GetComponent<NetPlayerController>().selectedItem);
        }
        
        if(_name != null)
        {
            testGameManager.players[_id].GetComponent<NetPlayerController>().selectedItem = (GameObject)Instantiate(Resources.Load($"Item Charmodels/{_name}_Charmodel"), testGameManager.players[_id].GetComponent<NetPlayerController>().camTransform);
        }
        
        
    }

    public static void FireWeapon(Packet _packet)
    {
        int _id = _packet.ReadInt();
        string _name = _packet.ReadString();

        
        Instantiate(Resources.Load($"Projectiles/{_name}_Projectile"), testGameManager.players[_id].GetComponent<NetPlayerController>().camTransform.position, testGameManager.players[_id].GetComponent<NetPlayerController>().camTransform.rotation);
    }

    public static void ChangeHealth(Packet _packet)
    {
        float _value = _packet.ReadFloat();


        testGameManager.players[Client.instance.myId].GetComponent<Player>().health = _value;
    }

    public static void Die(Packet _packet)
    {
        int _id = _packet.ReadInt();

        Debug.Log($"Player {_id} has died");
        //Destroy(testGameManager.players[_id].gameObject);
        //testGameManager.players.Remove(_id);
    }

}
