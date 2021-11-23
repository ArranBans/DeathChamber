using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField]ItemSO gSO;
    public float id;

    public ItemSO Pickup()
    {
        return gSO;
    }
}
