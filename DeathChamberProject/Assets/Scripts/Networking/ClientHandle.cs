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
        int _tick = _packet.ReadInt();

        testGameManager.players[_id].GetComponent<testPlayerController>().ServerPositionStates.Add(new PositionState(_position, _tick));
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
}
