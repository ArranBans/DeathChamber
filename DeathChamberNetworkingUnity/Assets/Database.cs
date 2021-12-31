using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour
{
    public static Database instance;
    public ItemDatabase itemDatabase;
    public EnemyDatabase enemyDatabase;

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

    public ItemSO GetItem(int id)
    {
        return itemDatabase.database[id];
    }

    public EnemySO GetEnemy(int id)
    {
        return enemyDatabase.database[id];
    }
}
