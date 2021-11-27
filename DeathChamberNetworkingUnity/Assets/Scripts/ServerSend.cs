using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ServerSend : MonoBehaviour
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }



    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }

    }

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].tcp.SendData(_packet);
            }

        }

    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }

    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }

        }

    }

    #region Packets
    public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.username);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);

            SendTCPData(_toClient, _packet);
            Debug.Log($"Told player {_toClient}, {_player.username} to spawn in player {_player.id}");
        }
    }

    public static void PlayerPosition(Player _player, int _tick)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_tick);

            SendUDPDataToAll(_packet);
        }
    }
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.id);
            _packet.Write(_player.transform.rotation);

            SendUDPDataToAll(_player.id, _packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            _packet.Write(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnItem(int _id ,int _itemId, string _name, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnItems))
        {
            _packet.Write(_itemId);
            _packet.Write(_name);
            _packet.Write(_pos);
            _packet.Write(_rot);

            SendTCPData(_id, _packet);
        }
    }

    public static void SpawnItem(int _itemId, string _name, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnItems))
        {
            _packet.Write(_itemId);
            _packet.Write(_name);
            _packet.Write(_pos);
            _packet.Write(_rot);

            SendTCPDataToAll(_packet);
        }
    }

    public static void RemoveItem(int _itemId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.removeItemPickup))
        {
            _packet.Write(_itemId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ItemPosition(int _itemId, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemPosition))
        {
            _packet.Write(_itemId);
            _packet.Write(_pos);
            _packet.Write(_rot);

            SendTCPDataToAll(_packet);
        }
    }

    public static void AddItemToInventory(int _id, string _item, int _itemId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.addItemToInventory))
        {
            _packet.Write(_item);
            _packet.Write(_itemId);

            SendTCPData(_id, _packet);
        }
    }

    public static void RemoveItemFromInventory(int _id, int _index)
    {
        using (Packet _packet = new Packet((int)ServerPackets.removeItemFromInventory))
        {

        }
    }
    #endregion
}
