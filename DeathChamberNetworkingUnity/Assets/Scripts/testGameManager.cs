using System.Collections;
using RiptideNetworking;
using System.Collections.Generic;
using UnityEngine;

public class testGameManager : MonoBehaviour
{
    public static testGameManager instance;
    public List<Transform> spawnPoints = new List<Transform>();
    public Vector3 spawnPoint;
    public List<ItemPickup> items = new List<ItemPickup>();
    public List<ItemInfo> startingItems = new List<ItemInfo>();
    public List<EnemyTest> enemies = new List<EnemyTest>();
    float timeToNextSpawn = 0;
    public int maxItems;
    public float itemSpawnInterval;
    float timeToNextUpdate;
    public float updateFrequency = 15;
    float updateTime;
    public ushort mapId;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void Start()
    {
        foreach (ItemInfo _item in startingItems)
        {
            if (_item != null)
            {
                int _aux1 = 0;
                int _aux2 = 0;

                if (_item.iSO.itemType == ItemSO.ItemType.gun)
                {
                    _aux1 = ((GunSO)_item.iSO).magAmmo;
                    _aux2 = ((GunSO)_item.iSO).maxAmmo;
                }
                _item.ChangeState(ItemInfo.ItemState.pickup, _aux1, _aux2);
                ItemPickup itemP = _item.GetComponent<ItemPickup>();
                itemP.id = items.Count;
                items.Add(itemP);
            }

        }
        
        foreach (EnemyTest _enemy in enemies)
        {
            if(_enemy != null)
            {
                _enemy.id = enemies.IndexOf(_enemy);
            }
            
        }

        updateTime = 1 / updateFrequency;
    }

    private void FixedUpdate()
    {

        if(Time.time >= timeToNextSpawn && items.Count <= maxItems)
        {
            
            int r = Mathf.FloorToInt(Random.Range(1, Database.instance.itemDatabase.database.Count));

            SpawnItem(Database.instance.GetItem(r).id, new Vector3(0, 10, 0), Quaternion.identity);
            Debug.Log($"Spawning {r}");
            timeToNextSpawn = Time.time + itemSpawnInterval;
        }

        if (Time.time >= timeToNextUpdate)
        {
            foreach (ItemPickup _item in items)
            {
                if (_item != null)
                {
                    S_ItemPosition(_item.id, _item.gameObject.transform.position, _item.gameObject.transform.rotation);
                }
            }
            timeToNextUpdate = Time.time + updateTime;
           // Debug.Log(timeToNextUpdate);
        }
    }

    public void SpawnItem(int id, Vector3 _location, Quaternion _rotation)
    {
        GameObject ItemGo = Instantiate(Database.instance.GetItem(id).empty, _location, _rotation);

        int _aux1 = 0;
        int _aux2 = 0;

        if (Database.instance.GetItem(id).itemType == ItemSO.ItemType.gun)
        {
            _aux1 = ((GunSO)Database.instance.GetItem(id)).magAmmo;
            _aux2 = ((GunSO)Database.instance.GetItem(id)).maxAmmo;
        }
        ItemGo.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.pickup, _aux1, _aux2);
        ItemPickup iPickup = ItemGo.GetComponent<ItemPickup>();
        iPickup.id = items.Count;
        items.Add(iPickup);
            
        S_SpawnItem(iPickup.id, iPickup.iSO.id, _location, _rotation, _aux1, 0);
    }

    public void SpawnEnemy(int enemyType, Vector3 _location, Quaternion _rotation)
    {
        GameObject EnemyGo = Instantiate(Database.instance.GetEnemy(enemyType).obj, _location, _rotation);
        EnemyTest enemy = EnemyGo.GetComponent<EnemyTest>();
        enemy.id = enemies.Count;
        enemies.Add(enemy);

        S_SpawnEnemy(enemy.id, enemy.eSO.id, _location, _rotation);
    }

    public void SpawnItemTest()
    {
        SpawnItem(1, new Vector3(0, 10, 0), Quaternion.identity);
    }

    public void Deploy(ushort _id)
    {
        int _spawnPI = Mathf.FloorToInt(Random.Range(0, spawnPoints.Count));
        PlayerManager.list[_id].player = NetworkManager.instance.InstantiatePlayer(PlayerManager.list[_id], spawnPoints[_spawnPI].position);
        PlayerManager.list[_id].player.Initialise();
        PlayerManager.list[_id].player.pManager = PlayerManager.list[_id];
        S_Deploy(_id, PlayerManager.list[_id].player.transform.position, PlayerManager.list[_id].player.transform.rotation);
    }

    #region Messages
    public void S_Deploy(ushort _id, Vector3 pos, Quaternion rot)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.serverDeploy);
        message.Add(_id);
        message.Add(pos);
        message.Add(rot);

        NetworkManager.instance.Server.SendToAll(message);
    }

    public void S_SpawnItem(int _itemId, int _databaseId, Vector3 _pos, Quaternion _rot, int _aux1, int _aux2) // Spawn Item 2
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.spawnItems);
        message.Add(_itemId);
        message.Add(_databaseId);
        message.Add(_pos);
        message.Add(_rot);
        message.Add(_aux1);
        message.Add(_aux2);

        NetworkManager.instance.Server.SendToAll(message);
    }
    public void S_SpawnItem(int _id, int _itemId, int _databaseId, Vector3 _pos, Quaternion _rot, int _aux1, int _aux2)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.spawnItems);
        message.Add(_itemId);
        message.Add(_databaseId);
        message.Add(_pos);
        message.Add(_rot);
        message.Add(_aux1);
        message.Add(_aux2);

        NetworkManager.instance.Server.Send(message, (ushort)_id);
    }

    public void S_SpawnEnemy(int _enemyId, int _enemyType, Vector3 _pos, Quaternion _rot) // Spawn Enemy 2
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.spawnEnemy);
        message.Add(_enemyId);
        message.Add(_enemyType);
        message.Add(_pos);
        message.Add(_rot);

        NetworkManager.instance.Server.SendToAll(message);
    }
    public void S_SpawnEnemy(int _id, int _enemyId, int _enemyType, Vector3 _pos, Quaternion _rot)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.spawnEnemy);
        message.Add(_enemyId);
        message.Add(_enemyType);
        message.Add(_pos);
        message.Add(_rot);

        NetworkManager.instance.Server.Send(message, (ushort)_id);
    }

    public void S_ItemPosition(int _itemId, Vector3 _pos, Quaternion _rot)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.itemPosition);
        message.Add(_itemId);
        message.Add(_pos);
        message.Add(_rot);

        NetworkManager.instance.Server.SendToAll(message);
    }
    public void S_EnemyPosition(int _enemyId, Vector3 _pos, Quaternion _rot)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ServerToClientId.enemyPosition);
        message.Add(_enemyId);
        message.Add(_pos);
        message.Add(_rot);

        NetworkManager.instance.Server.SendToAll(message);
    }
    public void S_RemoveItem(int _itemId)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.removeItemPickup);
        message.Add(_itemId);

        NetworkManager.instance.Server.SendToAll(message);
    }
    [MessageHandler((ushort)ClientToServerId.deploy)]
    private static void R_Deploy(ushort _fromClient, Message message)
    {
        if(PlayerManager.list[_fromClient].player == null)
        {
            instance.Deploy(_fromClient);
        }
        
    }
    #endregion
}


