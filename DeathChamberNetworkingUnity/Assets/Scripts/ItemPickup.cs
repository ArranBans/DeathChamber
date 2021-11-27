using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemSO gSO;
    public string prefabName;
    public int id;

    public ItemSO Pickup()
    {
        return gSO;
    }
}
