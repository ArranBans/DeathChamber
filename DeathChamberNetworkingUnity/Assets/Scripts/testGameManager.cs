using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGameManager : MonoBehaviour
{
    public static testGameManager instance;
    public Vector3 spawnPoint;
    public List<ItemPickup> items = new List<ItemPickup>();
    float timeToNextSpawn = 0;
    public float itemSpawnInterval;
    

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
    }

    private void FixedUpdate()
    {
        foreach(ItemPickup _item in items)
        {
            if (_item != null)
            {
                ServerSend.ItemPosition(_item.id, _item.gameObject.transform.position, _item.gameObject.transform.rotation);
            }
            
        }

        if(Time.time >= timeToNextSpawn)
        {
            
            int r = Mathf.FloorToInt(Random.Range(1, Database.instance.itemDatabase.database.Count));

            SpawnItem(Database.instance.GetItem(r).id, new Vector3(0, 10, 0), Quaternion.identity);
            Debug.Log($"Spawning {r}");
            timeToNextSpawn = Time.time + itemSpawnInterval;
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

    public IEnumerator Respawn(int id)
    {
        yield return new WaitForSeconds(2.5f);
        Server.clients[id].player.capsule.gameObject.SetActive(true);
        Server.clients[id].player.transform.position = spawnPoint;
        Server.clients[id].player.health = Server.clients[id].player.maxHealth;
        ServerSend.Respawn(id);
        Debug.Log($"player {id} has respawned");
    }
}
