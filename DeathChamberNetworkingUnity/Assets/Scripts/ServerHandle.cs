using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromclient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();
        string _username = _packet.ReadString();
        Debug.Log($"{Server.clients[_fromclient].tcp.socket.Client.RemoteEndPoint} connected successfuly and is now player {_fromclient}.");
        if (_fromclient != _clientIdCheck)
        {
            Debug.Log($"Player \"{_username}\" (ID: {_fromclient}) has assumed the wrong client ID ({_clientIdCheck})!!!");
        }
        Server.clients[_fromclient].pName = _username;
        Server.clients[_fromclient].SendIntoGame(_username);
    }

    public static void PlayerMovement(int _fromClient, Packet _packet)
    {
        bool[] _inputs = new bool[_packet.ReadInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = _packet.ReadBool();
        }
        Quaternion _rotation = _packet.ReadQuaternion();
        Quaternion _camRotation = _packet.ReadQuaternion();
        int _tick = _packet.ReadInt();

        Server.clients[_fromClient].player.SetInput(_inputs, _rotation, _camRotation, _tick);
    }

    public static void UDPTest(int _fromClient, Packet _packet)
    {
        string _msg = _packet.ReadString();

        Debug.Log($"Client {_fromClient} says: {_msg}!!!");
    }

    public static void Interact(int _fromClient, Packet _packet)
    {
        ItemPickup _item;
        if(Server.clients[_fromClient].player.InteractRaycast(out _item))
        {

           //Debug.Log($"client {_fromClient} interacted with {_item}");
            if(Server.clients[_fromClient].player.AddItemToInventory(_item.iSO))
            {
                ServerSend.AddItemToInventory(_fromClient, _item.iSO.id);
                ServerSend.RemoveItem(_item.id);
                testGameManager.instance.items[testGameManager.instance.items.IndexOf(_item)] = null;
                UnityEngine.Object.Destroy(_item.gameObject);
            }
            
        }
    }

    public static void DropItem(int _fromClient, Packet _packet)
    {
        int _index = _packet.ReadInt();
        int _id = Server.clients[_fromClient].player.inventory[_index].itemSO.id;


        Server.clients[_fromClient].player.RemoveItemFromInventory(_index);
        ServerSend.RemoveItemFromInventory(_fromClient, _index);
        testGameManager.instance.SpawnItem(_id, Server.clients[_fromClient].player.dropTransform.position, Server.clients[_fromClient].player.transform.rotation);
    }

    public static void ChangeSelectedItem(int _fromClient, Packet _packet)
    {
        int _index = _packet.ReadInt();
        Debug.Log(_index);
        Server.clients[_fromClient].player.ChangeSelectedItem(_index);

        if (_index + 1 <= Server.clients[_fromClient].player.inventory.Count)
        {
            if (Server.clients[_fromClient].player.inventory[_index] != null)
            {
                Debug.Log($"Player {_fromClient} changed item to {_index}");
                ServerSend.ChangeSelectedItem(_fromClient, Server.clients[_fromClient].player.inventory[Server.clients[_fromClient].player.selectedItem].itemSO.id);
            }
            else
            {
                ServerSend.ChangeSelectedItem(_fromClient, Server.clients[_fromClient].player.inventory[Server.clients[_fromClient].player.selectedItem].itemSO.id);
            }
        } 
        else
        {
            ServerSend.ChangeSelectedItem(_fromClient, 0);
        }


    }

    public static void FireWeapon(int _fromClient, Packet _packet)
    {
        GameObject bullet = (GameObject)GameObject.Instantiate(Resources.Load($"Projectiles/{Server.clients[_fromClient].player.inventory[Server.clients[_fromClient].player.selectedItem].itemSO.ItemName}_Projectile"), Server.clients[_fromClient].player.bulletTransform.position, Server.clients[_fromClient].player.bulletTransform.rotation);
        ServerSend.FireWeapon(_fromClient, Server.clients[_fromClient].player.inventory[Server.clients[_fromClient].player.selectedItem].itemSO.ItemName);
    }
}
