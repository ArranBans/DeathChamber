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

    public static void SendMap(int _toClient, int _mapId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendMap))
        {
            _packet.Write(_mapId);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, PlayerManager _playerManager)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            _packet.Write(_playerManager.id);
            _packet.Write(_playerManager.username);

            SendTCPData(_toClient, _packet);
            Debug.Log($"Told player {_toClient}, {_playerManager.username} to spawn in player {_playerManager.id}");
        }
    }

    public static void PlayerPosition(Player _player, int _tick, bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            _packet.Write(_player.pManager.id);
            _packet.Write(_player.transform.position);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_tick);

            _packet.Write(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                _packet.Write(_input);
            }

            SendUDPDataToAll( _packet);

            
        }
    }
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            _packet.Write(_player.pManager.id);
            _packet.Write(_player.transform.rotation);
            _packet.Write(_player.camTransform.rotation);

            SendUDPDataToAll(_player.pManager.id, _packet);
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

    public static void SpawnItem(int _id ,int _itemId, int _databaseId, Vector3 _pos, Quaternion _rot, int _aux1, int _aux2)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnItems))
        {
            _packet.Write(_itemId);
            _packet.Write(_databaseId);
            _packet.Write(_pos);
            _packet.Write(_rot);
            _packet.Write(_aux1);
            _packet.Write(_aux2);

            SendTCPData(_id, _packet);
        }
    }

    public static void SpawnItem(int _itemId, int _databaseId, Vector3 _pos, Quaternion _rot, int _aux1, int _aux2)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnItems))
        {
            _packet.Write(_itemId);
            _packet.Write(_databaseId);
            _packet.Write(_pos);
            _packet.Write(_rot);
            _packet.Write(_aux1);
            _packet.Write(_aux2);

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

            SendUDPDataToAll(_packet);
        }
    }

    public static void AddItemToInventory(int _id, int _itemId,int _aux1, int _aux2)
    {
        using (Packet _packet = new Packet((int)ServerPackets.addItemToInventory))
        {
            _packet.Write(_itemId);
            _packet.Write(_aux1);
            _packet.Write(_aux2);

            SendTCPData(_id, _packet);
        }
    }

    public static void RemoveItemFromInventory(int _id, int _index, bool _clear)
    {
        using (Packet _packet = new Packet((int)ServerPackets.removeItemFromInventory))
        {
            _packet.Write(_index);
            _packet.Write(_clear);

            SendTCPData(_id, _packet);
        }
    }

    public static void ChangeSelectedItem(int _id, int _itemId, int _aux1, int _aux2)
    {
        using (Packet _packet = new Packet((int)ServerPackets.changeSelectedItem))
        {
            _packet.Write(_id);
            _packet.Write(_itemId);
            _packet.Write(_aux1);
            _packet.Write(_aux2);

            SendTCPDataToAll(_id, _packet);
        }
    }

    public static void FireWeapon(int _id, string _name)
    {
        using (Packet _packet = new Packet((int)ServerPackets.fireWeapon))
        {
            _packet.Write(_id);
            _packet.Write(_name);
            _packet.Write(Server.clients[_id].playerManager.player.inventory[Server.clients[_id].playerManager.player.selectedItem].itemSO.id);

            SendUDPDataToAll(_id, _packet);
        }
    }

    public static void ChangeHealth(int _id, float _value)
    {
        using (Packet _packet = new Packet((int)ServerPackets.changeHealth))
        {
            _packet.Write(_value);

            SendTCPData(_id, _packet);
        }
    }

    public static void Die(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.die))
        {
            _packet.Write(_id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void Respawn(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.respawn))
        {
            _packet.Write(_id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void Deploy(int _id, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.serverDeploy))
        {
            _packet.Write(_id);
            _packet.Write(_pos);
            _packet.Write(_rot);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnEnemy(int _enemyId, int _enemyType, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            _packet.Write(_enemyId);
            _packet.Write(_enemyType);
            _packet.Write(_pos);
            _packet.Write(_rot);
            Debug.Log($"Told players to spawn in Enemy: {_enemyId}");

            SendTCPDataToAll(_packet);
        }
    }
    public static void SpawnEnemy(int _id, int _enemyId, int _enemyType, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            _packet.Write(_enemyId);
            _packet.Write(_enemyType);
            _packet.Write(_pos);
            _packet.Write(_rot);
            Debug.Log($"Told player {_id}, {Server.clients[_id].playerManager.username} to spawn in Enemy: {_enemyId}");

            SendTCPData(_id, _packet);
        }
    }
    public static void EnemyPosition(int _enemyId, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyPosition))
        {
            _packet.Write(_enemyId);
            _packet.Write(_pos);
            _packet.Write(_rot);

            SendTCPDataToAll(_packet);
        }
    }

    #endregion
}
