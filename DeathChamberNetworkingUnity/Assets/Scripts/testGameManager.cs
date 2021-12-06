using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGameManager : MonoBehaviour
{
    public static testGameManager instance;
    public Vector3 spawnPoint;
    public List<ItemPickup> items = new List<ItemPickup>();
    

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
    }

    public void SpawnItem(string _name, Vector3 _location, Quaternion _rotation)
    {
        GameObject ItemGo = Instantiate((GameObject)Resources.Load($"ItemPickups/{_name}_Pickup"), _location, _rotation);
        ItemPickup iPickup = ItemGo.GetComponent<ItemPickup>();
        iPickup.id = items.Count;
        items.Add(iPickup);
        ServerSend.SpawnItem(iPickup.id, iPickup.gSO.ItemName, _location, _rotation);
    }

    public void SpawnItemTest()
    {
        SpawnItem("Akm", new Vector3(0, 10, 0), Quaternion.identity);
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
