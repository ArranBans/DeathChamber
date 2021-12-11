using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItem : MonoBehaviour, IButtonActivated
{
    public int ItemToSpawn;
    public Vector3 spawnPos;
    public Vector3 spawnRot;

    public void Activated()
    {
        testGameManager.instance.SpawnItem(ItemToSpawn, spawnPos, Quaternion.Euler(spawnRot));
    }
}
