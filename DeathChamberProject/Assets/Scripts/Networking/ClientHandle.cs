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

        testGameManager.players[_id].transform.rotation = _rotation;
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
        GameObject _item = (GameObject)Instantiate(Resources.Load($"Item Pickups/{_name}"), _pos, _rot);
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
            if(_item.id == _id)
            {
                _item.gameObject.transform.position = _pos;
                _item.gameObject.transform.rotation = _rot;
            }
        }
    }

    public static void AddItemToInventory(Packet _packet)
    {
        //player.AddItemToInventory
        Debug.Log("Adding item to inventory");
    }

    public static void RemoveItemFromInventory(Packet _packet)
    {
        //player.RemoveItemFromInventory
    }
}
