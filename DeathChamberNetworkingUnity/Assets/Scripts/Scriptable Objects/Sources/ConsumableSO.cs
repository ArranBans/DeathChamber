using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "ScriptableObjects/Consumable")]
public class ConsumableSO : ItemSO
{
    public float value;
    public float useTime;
    public float beginUseTime;
    public Vector3 idlePos;
    public Vector3 usePos;
    public Vector3 idleRot;
    public Vector3 useRot;

    public enum ConsumableType
    {
        health,
        speedBoost
    }
}
