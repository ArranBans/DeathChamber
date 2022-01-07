using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientHandle : MonoBehaviour
{
    /*
    public static void Welcome(Packet _packet)
    {
        string msg = message.GetString();
        int _myId = message.GetInt();

        Debug.Log($"Message from server: {msg}");
        NetworkManager.instance.Client.Id = _myId;

        //          ClientSend.WelcomeReceived();// Also Calls welcomeRecieved
        NetworkUiManager.instance.ConnectedToServer();

        Client.instance.udp.Connect(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        //          ClientSend.UDPTest("Hello Server!");
    }

    public static void SendMap(Packet _packet)
    {
        int _mapId = message.GetInt();
        Client.instance.LoadMap(_mapId);
    }

    public static void SpawnPlayer(Packet _packet)
    {
        Debug.Log($"SpawnPlayerReceived");
        int _id = message.GetInt();
        string _username = message.GetString();

        testGameManager.instance.SpawnPlayerManager(_id, _username);

        if (_id != NetworkManager.instance.Client.Id)
        {
            ////          ClientSend.ChangeSelectedItem(testPlayerManager.list[NetworkManager.instance.Client.Id].GetComponent<Player>().selectedItem);
        }

        Debug.Log($"Spawning Player {_id}, {_username}");
    }

    public static void PlayerPosition(Packet _packet)
    {
        int _id = message.GetInt();
        Vector3 _position = message.GetVector3();
        Quaternion _rotation = message.GetQuaternion();
        int _tick = message.GetInt();

        bool[] _inputs = new bool[message.GetInt()];
        for (int i = 0; i < _inputs.Length; i++)
        {
            _inputs[i] = message.GetBool();
        }

        if(!testPlayerManager.list[(ushort)_id].playerObj)
        {
            return;
        }

        if (testPlayerManager.list[(ushort)_id].playerObj.GetComponent<testPlayerController>() != null)
        {
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<testPlayerController>().OnServerState(new PositionState(_position, _rotation, _tick));
        }
        else
        {
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().DesiredPos = _position;
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().DesiredRot = _rotation;
            //Debug.Log($"{_id}, Input: {_inputs[0]}, {_inputs[1]}, {_inputs[2]}, {_inputs[3]}");
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().PlayerMoved(_inputs);
        }
        
        
        //Debug.Log($"Packet ID: {_id} should hold position {_position}");
        //testPlayerManager.list[(ushort)_id].transform.position = _position;
    }
    public static void PlayerRotation(Packet _packet)
    {
        int _id = message.GetInt();
        Quaternion _rotation = message.GetQuaternion();
        Quaternion _camRotation = message.GetQuaternion();

        testPlayerManager.list[(ushort)_id].playerObj.transform.rotation = _rotation;
        testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().camTransform.rotation = _camRotation;
    }

    public static void PlayerDisconnected(Packet _packet)
    {
        int _id = message.GetInt();

        Destroy(testPlayerManager.list[(ushort)_id].gameObject);
        testGameManager.players.Remove(_id);
    }

    public static void SpawnItems(Packet _packet)
    {
        int _id = message.GetInt();
        int _databaseId = message.GetInt();
        Vector3 _pos = message.GetVector3();
        Quaternion _rot = message.GetQuaternion();
        int _aux1 = message.GetInt();
        int _aux2 = message.GetInt();

        Debug.Log($"Spawning: {_databaseId}");
        GameObject _item = Instantiate(Database.instance.GetItem(_databaseId).empty, _pos, _rot);
        _item.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.pickup, _aux1, _aux2);
        _item.GetComponent<ItemPickup>().id = _id;
        testGameManager.itemPickups.Add(_item.GetComponent<ItemPickup>());
        //Debug.Log($"Spawned: {_databaseId}");
    }

    public static void ItemPosition(Packet _packet)
    {
        int _id = message.GetInt();
        Vector3 _pos = message.GetVector3();
        Quaternion _rot = message.GetQuaternion();

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
        int _ItemId = message.GetInt();
        int _aux1 = message.GetInt();
        int _aux2 = message.GetInt();

        GameObject itemGO = (GameObject)Instantiate(Database.instance.GetItem(_ItemId).empty, testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<testPlayerController>().cam.transform);
        itemGO.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.item, _aux1, _aux2);
        testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().AddItemToInventory(itemGO.GetComponent<Item>());
        testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().ChangeSelectedItem(testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().inventory.IndexOf(itemGO.GetComponent<Item>()));


        //Debug.Log("Adding item to inventory");
    }

    public static void RemoveItemPickup(Packet _packet)
    {
        int _id = message.GetInt();

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
        int _index = message.GetInt();
        bool _clear = message.GetBool();
        if(_clear)
        {
            foreach(Item i in testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().inventory)
            {
                Destroy(i);
            }

            testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().inventory = new List<Item>();
            return;
        }
        testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().RemoveItemFromInventory(_index);
    }

    public static void ChangeSelectedItem(Packet _packet)
    {
        int _id = message.GetInt();
        int _itemId = message.GetInt();
        int _aux1 = message.GetInt();
        int _aux2 = message.GetInt();

        Debug.Log($"{_id} changing item to {_itemId}");

        if(testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem != null)
        {
            Destroy(testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem);
        }
        
        if(_itemId != 0)
        {
            NetPlayerController netPlayer = testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>();
            netPlayer.selectedItem = (GameObject)Instantiate(Database.instance.GetItem(_itemId).empty, testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().camTransform);
            netPlayer.selectedItemInfo = testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem.GetComponent<ItemInfo>();
            netPlayer.selectedItemInfo.ChangeState(ItemInfo.ItemState.charModel, _aux1, _aux2);   
        }
        
        
    }

    public static void FireWeapon(Packet _packet)
    {
        int _id = message.GetInt();
        string _name = message.GetString();
        int _weaponId = message.GetInt();
        
        Instantiate(Resources.Load($"Projectiles/{_name}_Projectile"), testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().camTransform.position, testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().camTransform.rotation);
        if (testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem)
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem.GetComponent<GunInfo>().fireAudio.PlayOneShot(testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem.GetComponent<GunInfo>().fireAudio.clip);
        testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem.GetComponent<GunInfo>().muzzleFlash.Play();
    }

    public static void ChangeHealth(Packet _packet)
    {
        float _value = message.GetFloat();

        Player p = testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>();
        if (p.health > _value)
        {
            p.TakeDamage();
        }
        p.health = _value;
        p.healthSlider.value = _value;

        
    }

    public static void Die(Packet _packet)
    {
        int _id = message.GetInt();

        Debug.Log($"Player {_id} has died");
        
        if(_id == NetworkManager.instance.Client.Id)
        {
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<Player>().Die();
        }
        else
        {
            Destroy(testPlayerManager.list[(ushort)_id].playerObj);
        }
    }

    public static void Respawn(Packet _packet)
    {
        int _id = message.GetInt();

        Debug.Log($"Player {_id} has respawned");

        if (_id == NetworkManager.instance.Client.Id)
        {
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<Player>().Respawn();
        }
        else
        {
            testPlayerManager.list[(ushort)_id].playerObj.SetActive(true);
        }
    }

    public static void Deploy(Packet _packet)
    {
        int _id = message.GetInt();
        Vector3 _pos = message.GetVector3();
        Quaternion _rot = message.GetQuaternion();

        Debug.Log($"Player {_id} has Deployed!!!");

        if(!testPlayerManager.list[(ushort)_id].playerObj)
        {
            testGameManager.instance.SpawnPlayer(_id, _pos, _rot);
        }
        else
        {
            Debug.Log($"Player {_id} is already deployed!!!");
        }
        

    }

    public static void SpawnEnemy(Packet _packet)
    {
        int _enemyId = message.GetInt();
        int _enemyType = message.GetInt();
        Vector3 _pos = message.GetVector3();
        Quaternion _rot = message.GetQuaternion();

        Debug.Log($"Spawning Enemy: {_enemyType}");
        GameObject _enemy = Instantiate(Database.instance.GetEnemy(_enemyType).obj, _pos, _rot);
        _enemy.GetComponent<EnemyTest>().id = _enemyId;
        testGameManager.enemies.Add(_enemy.GetComponent<EnemyTest>());
    }

    public static void EnemyPosition(Packet _packet)
    {
        int _id = message.GetInt();
        Vector3 _pos = message.GetVector3();
        Quaternion _rot = message.GetQuaternion();

        foreach (EnemyTest _enemy in testGameManager.enemies)
        {
            if (_enemy != null)
            {
                if (_enemy.id == _id)
                {
                    _enemy.UpdateEnemyState(_pos, _rot);
                }
            }

        }
    }

    public static void EnemyFire(Packet _packet)
    {
        int _id = message.GetInt();
        Quaternion _fireRot = message.GetQuaternion();

        foreach (EnemyTest e in testGameManager.enemies)
        {
            if (e.id == _id)
            {
                GameObject projectile = Instantiate(e.eSO.projectile, e.attackPoint.position, _fireRot);
                break;
            }
            else
            {
                continue;
            }

        }

        
    }

    public static void EnemyDie(Packet _packet)
    {
        int _id = message.GetInt();
        Debug.Log($"Enemy: {_id} has died!");
        foreach(EnemyTest e in testGameManager.enemies)
        {
            if(e.id == _id)
            {
                Destroy(e.gameObject);
                break;
            }
            else
            {
                continue;
            }
            
        }
        
    }
    */
}
