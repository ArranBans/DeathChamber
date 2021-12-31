using System.Collections;
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
    public int mapId;
    



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

                if(_item.iSO.itemType == ItemSO.ItemType.gun)
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
                    ServerSend.ItemPosition(_item.id, _item.gameObject.transform.position, _item.gameObject.transform.rotation);
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
            
        ServerSend.SpawnItem(iPickup.id, iPickup.iSO.id, _location, _rotation, _aux1, 0);
    }

    public void SpawnEnemy(int enemyType, Vector3 _location, Quaternion _rotation)
    {
        GameObject EnemyGo = Instantiate(Database.instance.GetEnemy(enemyType).obj, _location, _rotation);
        EnemyTest enemy = EnemyGo.GetComponent<EnemyTest>();
        enemy.id = enemies.Count;
        enemies.Add(enemy);

        ServerSend.SpawnEnemy(enemy.id, enemy.eSO.id, _location, _rotation);
    }

    public void SpawnItemTest()
    {
        SpawnItem(1, new Vector3(0, 10, 0), Quaternion.identity);
    }

    public void Respawner(int _id)
    {
        StartCoroutine(Server.clients[_id].Respawn(_id));
    }

    public void Respawn(int _id)
    {
        //Player p = NetworkManager.instance.InstantiatePlayerManager();
        //p.SetHealth(p.maxHealth);
        //Server.clients[_id].player = p;
        ServerSend.Respawn(_id);
        //p.ChangeSelectedItem(0);
        ServerSend.ChangeSelectedItem(_id, 0,0,0);
    }

    public void Deploy(int _id)
    {
        int _spawnPI = Mathf.FloorToInt(Random.Range(0, spawnPoints.Count));
        Server.clients[_id].playerManager.player = NetworkManager.instance.InstantiatePlayer(Server.clients[_id].playerManager, spawnPoints[_spawnPI].position);
        Server.clients[_id].playerManager.player.Initialise();
        Server.clients[_id].playerManager.player.pManager = Server.clients[_id].playerManager;
        ServerSend.Deploy(_id, Server.clients[_id].playerManager.player.transform.position, Server.clients[_id].playerManager.player.transform.rotation);
    }

}
