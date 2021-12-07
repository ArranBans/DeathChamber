using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemSO iSO;
    public int id;

    public ItemSO Pickup()
    {
        return iSO;
    }
}
