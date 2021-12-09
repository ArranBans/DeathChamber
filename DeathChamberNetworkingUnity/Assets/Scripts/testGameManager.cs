using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGameManager : MonoBehaviour
{
    public static testGameManager instance;
    public Vector3 spawnPoint;
    public List<ItemPickup> items = new List<ItemPickup>();
    float timeToNextSpawn = 0;
    public int maxItems;
    public float itemSpawnInterval;
    float timeToNextUpdate;
    public float updateFrequency = 15;
    float updateTime;
    



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
        foreach(ItemPickup _item in items)
        {
            if(_item != null)
            {
                _item.id = items.IndexOf(_item);
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
        }
    }

    public void SpawnItem(int id, Vector3 _location, Quaternion _rotation)
    {
        GameObject ItemGo = Instantiate(Database.instance.GetItem(id).empty, _location, _rotation);
        ItemGo.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.pickup);
        ItemPickup iPickup = ItemGo.GetComponent<ItemPickup>();
        iPickup.id = items.Count;
        items.Add(iPickup);
        ServerSend.SpawnItem(iPickup.id, iPickup.iSO.id, _location, _rotation);
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
        Player p = NetworkManager.instance.InstantiatePlayer();
        p.Initialise(_id, Server.clients[_id].pName, spawnPoint);
        p.SetHealth(p.maxHealth);
        Server.clients[_id].player = p;
        ServerSend.Respawn(_id);
        p.ChangeSelectedItem(0);
        ServerSend.ChangeSelectedItem(_id, 0);
    }



}
