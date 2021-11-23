using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGameManager : MonoBehaviour
{
    public static testGameManager instance;
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
            _item.id = items.IndexOf(_item);
        }
    }

    private void FixedUpdate()
    {
        foreach(ItemPickup _item in items)
        {
            ServerSend.ItemPosition(_item.id, _item.gameObject.transform.position, _item.gameObject.transform.rotation);
        }
    }
}
