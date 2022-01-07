using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
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
        Server.clients[_fromclient].SetupPlayerManger(_username);
        ServerSend.SendMap(_fromclient, testGameManager.instance.mapId);
    }

    public static void MapLoaded(int _fromclient, Packet _packet)
    {
        Server.clients[_fromclient].loaded = true;
        Server.clients[_fromclient].SendIntoGame();
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

        PlayerManager.list[_fromClient].player.SetInput(_inputs, _rotation, _camRotation, _tick);
    }

    public static void UDPTest(int _fromClient, Packet _packet)
    {
        string _msg = _packet.ReadString();

        Debug.Log($"Client {_fromClient} says: {_msg}!!!");
    }

    public static void Interact(int _fromClient, Packet _packet)
    {
        ItemPickup _item;
        GameObject _object;
        if(PlayerManager.list[_fromClient].player.InteractRaycast(out _object))
        {

           //Debug.Log($"client {_fromClient} interacted with {_item}");
            if(_object.GetComponent(typeof(IInteractable)))
            {
                IInteractable i = (IInteractable)_object.GetComponent(typeof(IInteractable));
                i.Interacted();
            }
            else if(_object.GetComponent<ItemPickup>())
            {

                _item = _object.GetComponent<ItemPickup>();
                if (PlayerManager.list[_fromClient].player.AddItemToInventory(_item.iSO.id))
                {
                    int _aux1 = 0;
                    int _aux2 = 0;
                    if (Database.instance.GetItem(_item.iSO.id).itemType == ItemSO.ItemType.gun)
                    {
                        _aux1 = ((GunSO)Database.instance.GetItem(_item.iSO.id)).magAmmo;
                        _aux2 = ((GunSO)Database.instance.GetItem(_item.iSO.id)).maxAmmo;
                    }

                    ServerSend.AddItemToInventory(_fromClient, _item.iSO.id, _aux1, _aux2);
                    ServerSend.RemoveItem(_item.id);
                    testGameManager.instance.items[testGameManager.instance.items.IndexOf(_item)] = null;
                    UnityEngine.Object.Destroy(_item.gameObject);
                }
            }
            
            
        }
    }

    public static void DropItem(int _fromClient, Packet _packet)
    {
        int _index = _packet.ReadInt();
        int _id = PlayerManager.list[_fromClient].player.inventory[_index].itemSO.id;


        PlayerManager.list[_fromClient].player.RemoveItemFromInventory(_index, false);
        testGameManager.instance.SpawnItem(_id, PlayerManager.list[_fromClient].player.dropTransform.position, PlayerManager.list[_fromClient].player.transform.rotation);
    }

    public static void ChangeSelectedItem(int _fromClient, Packet _packet)
    {
        int _index = _packet.ReadInt();
        //Debug.Log(_index);
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
                ServerSend.ChangeSelectedItem(_fromClient, PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO.id, _aux1, _aux2);
            }
            else
            {
                ServerSend.ChangeSelectedItem(_fromClient, PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO.id,0,0);
            }
        } 
        else
        {
            ServerSend.ChangeSelectedItem(_fromClient, 0,0,0);
        }


    }

    public static void FireWeapon(int _fromClient, Packet _packet)
    {
        bool _aiming = _packet.ReadBool();
        float _gunXRot = _packet.ReadFloat();

        if(_aiming)
        {
            PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].transform.localPosition = ((GunSO)PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO).aimPos;
        }
        else
        {
            PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].transform.localPosition = ((GunSO)PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO).hipPos;
        }

        GameObject bullet = (GameObject)GameObject.Instantiate(Resources.Load($"Projectiles/{PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO.ItemName}_Projectile"), PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].transform.TransformPoint(((GunSO)PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO).bulletSpawnPoint), Quaternion.Euler(_gunXRot, PlayerManager.list[_fromClient].player.camTransform.rotation.eulerAngles.y, PlayerManager.list[_fromClient].player.camTransform.rotation.eulerAngles.z));
        ServerSend.FireWeapon(_fromClient, PlayerManager.list[_fromClient].player.inventory[PlayerManager.list[_fromClient].player.selectedItem].itemSO.ItemName);
    }

    public static void Deploy(int _fromClient, Packet _packet)
    {
        if(!PlayerManager.list[_fromClient].player)
        {
            testGameManager.instance.Deploy(_fromClient);
        }
    }

    public static void ConsumableUse(int _fromClient, Packet _packet)
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

        ServerSend.ChangeSelectedItem(_fromClient, newItem, _aux1, _aux2);
    }

    public static void Command(int _fromClient, Packet _packet)
    {
        int _commandType = _packet.ReadInt();
        int _index = _packet.ReadInt();

        if (_commandType == 0)
        {
            if(PlayerManager.list[_fromClient].player)
            {
                if(PlayerManager.list[_fromClient].player.AddItemToInventory(_index))
                {
                    int _aux1 = 0;
                    int _aux2 = 0;

                    if (Database.instance.GetItem(_index).itemType == ItemSO.ItemType.gun)
                    {
                        _aux1 = ((GunSO)Database.instance.GetItem(_index)).magAmmo;
                        _aux2 = ((GunSO)Database.instance.GetItem(_index)).maxAmmo;
                    }

                    ServerSend.AddItemToInventory(_fromClient, _index, _aux1, _aux2);
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
                PlayerManager.list[_fromClient].player.transform.position = Server.clients[_index].playerManager.player.transform.position;

            }
        }
    }
}
*/