using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnItem : ButtonActivated
{
    public int ItemToSpawn;
    public Vector3 spawnPos;
    public Quaternion spawnRot;

    public override void Activated()
    {
        testGameManager.instance.SpawnItem(ItemToSpawn, spawnPos, spawnRot);
    }
}
