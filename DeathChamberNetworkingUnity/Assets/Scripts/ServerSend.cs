using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
public class ServerSend : MonoBehaviour
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        message.AddLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }



    private static void SendTCPDataToAll(Packet _packet)
    {
        message.AddLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }

    }

    private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
    {
        message.AddLength();
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
        message.AddLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendUDPDataToAll(Packet _packet)
    {
        message.AddLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }

    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet)
    {
        message.AddLength();
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
            message.Add(_msg);
            message.Add(_toClient);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void SendMap(int _toClient, int _mapId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.sendMap))
        {
            message.Add(_mapId);
            SendTCPData(_toClient, _packet);
        }
    }

    public static void SpawnPlayer(int _toClient, PlayerManager _playerManager)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnPlayer))
        {
            message.Add(_playerManager.id);
            message.Add(_playerManager.username);

            SendTCPData(_toClient, _packet);
            Debug.Log($"Told player {_toClient}, {_playerManager.username} to spawn in player {_playerManager.id}");
        }
    }

    public static void PlayerPosition(Player _player, int _tick, bool[] _inputs)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerPosition))
        {
            message.Add(_player.pManager.id);
            message.Add(_player.transform.position);
            message.Add(_player.transform.rotation);
            message.Add(_tick);

            message.Add(_inputs.Length);
            foreach (bool _input in _inputs)
            {
                message.Add(_input);
            }

            SendUDPDataToAll( _packet);

            
        }
    }
    public static void PlayerRotation(Player _player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerRotation))
        {
            message.Add(_player.pManager.id);
            message.Add(_player.transform.rotation);
            message.Add(_player.camTransform.rotation);

            SendUDPDataToAll(_player.pManager.id, _packet);
        }
    }

    public static void PlayerDisconnected(int _playerId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.playerDisconnected))
        {
            message.Add(_playerId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnItem(int _id ,int _itemId, int _databaseId, Vector3 _pos, Quaternion _rot, int _aux1, int _aux2)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnItems))
        {
            message.Add(_itemId);
            message.Add(_databaseId);
            message.Add(_pos);
            message.Add(_rot);
            message.Add(_aux1);
            message.Add(_aux2);

            SendTCPData(_id, _packet);
        }
    }

    public static void SpawnItem(int _itemId, int _databaseId, Vector3 _pos, Quaternion _rot, int _aux1, int _aux2)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnItems))
        {
            message.Add(_itemId);
            message.Add(_databaseId);
            message.Add(_pos);
            message.Add(_rot);
            message.Add(_aux1);
            message.Add(_aux2);

            SendTCPDataToAll(_packet);
        }
    }

    public static void RemoveItem(int _itemId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.removeItemPickup))
        {
            message.Add(_itemId);

            SendTCPDataToAll(_packet);
        }
    }

    public static void ItemPosition(int _itemId, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.itemPosition))
        {
            message.Add(_itemId);
            message.Add(_pos);
            message.Add(_rot);

            SendUDPDataToAll(_packet);
        }
    }

    public static void AddItemToInventory(int _id, int _itemId,int _aux1, int _aux2)
    {
        using (Packet _packet = new Packet((int)ServerPackets.addItemToInventory))
        {
            message.Add(_itemId);
            message.Add(_aux1);
            message.Add(_aux2);

            SendTCPData(_id, _packet);
        }
    }

    public static void RemoveItemFromInventory(int _id, int _index, bool _clear)
    {
        using (Packet _packet = new Packet((int)ServerPackets.removeItemFromInventory))
        {
            message.Add(_index);
            message.Add(_clear);

            SendTCPData(_id, _packet);
        }
    }

    public static void ChangeSelectedItem(int _id, int _itemId, int _aux1, int _aux2)
    {
        using (Packet _packet = new Packet((int)ServerPackets.changeSelectedItem))
        {
            message.Add(_id);
            message.Add(_itemId);
            message.Add(_aux1);
            message.Add(_aux2);

            SendTCPDataToAll(_id, _packet);
        }
    }

    public static void FireWeapon(int _id, string _name)
    {
        using (Packet _packet = new Packet((int)ServerPackets.fireWeapon))
        {
            message.Add(_id);
            message.Add(_name);
            message.Add(PlayerManager.list[_id].player.inventory[PlayerManager.list[_id].player.selectedItem].itemSO.id);

            SendUDPDataToAll(_id, _packet);
        }
    }

    public static void ChangeHealth(int _id, float _value)
    {
        using (Packet _packet = new Packet((int)ServerPackets.changeHealth))
        {
            message.Add(_value);

            SendTCPData(_id, _packet);
        }
    }

    public static void Die(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.die))
        {
            message.Add(_id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void Respawn(int _id)
    {
        using (Packet _packet = new Packet((int)ServerPackets.respawn))
        {
            message.Add(_id);

            SendTCPDataToAll(_packet);
        }
    }

    public static void Deploy(int _id, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.serverDeploy))
        {
            message.Add(_id);
            message.Add(_pos);
            message.Add(_rot);

            SendTCPDataToAll(_packet);
        }
    }

    public static void SpawnEnemy(int _enemyId, int _enemyType, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            message.Add(_enemyId);
            message.Add(_enemyType);
            message.Add(_pos);
            message.Add(_rot);
            Debug.Log($"Told players to spawn in Enemy: {_enemyId}");

            SendTCPDataToAll(_packet);
        }
    }
    public static void SpawnEnemy(int _id, int _enemyId, int _enemyType, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnEnemy))
        {
            message.Add(_enemyId);
            message.Add(_enemyType);
            message.Add(_pos);
            message.Add(_rot);
            Debug.Log($"Told player {_id}, {PlayerManager.list[_id].username} to spawn in Enemy: {_enemyId}");

            SendTCPData(_id, _packet);
        }
    }
    public static void EnemyPosition(int _enemyId, Vector3 _pos, Quaternion _rot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyPosition))
        {
            message.Add(_enemyId);
            message.Add(_pos);
            message.Add(_rot);

            SendUDPDataToAll(_packet);
        }
    }

    public static void EnemyFire(int _enemyId, Quaternion _fireRot)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyFire))
        {
            message.Add(_enemyId);
            message.Add(_fireRot);

            SendUDPDataToAll(_packet);
        }
    }

    public static void EnemyDie(int _enemyId)
    {
        using (Packet _packet = new Packet((int)ServerPackets.enemyDie))
        {
            message.Add(_enemyId);

            SendTCPDataToAll(_packet);
        }
    }

    #endregion
}
*/