using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    [Header("Static Info")]
    public ItemSO iSO;
    public enum ItemState
    {
        item,
        pickup
    }
    [Header("Runtime Stats")]
    public ItemState iState;

    public void ChangeState(ItemState _state, int _aux1, int _aux2)
    {
        switch(_state)
        {
            case ItemState.item:
                if(iSO.itemType == ItemSO.ItemType.gun)
                {
                    Gun gun = gameObject.AddComponent<Gun>();
                    gun.itemInfo = this;
                }
                if (iSO.itemType == ItemSO.ItemType.consumable)
                {
                    Consumable item = gameObject.AddComponent<Consumable>();
                    item.itemInfo = this;
                }
                iState = ItemState.item;
                break;
            case ItemState.pickup:
                Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                BoxCollider b = gameObject.AddComponent<BoxCollider>();
                ItemPickup i = gameObject.AddComponent<ItemPickup>();
                i.iSO = iSO;
                rb.mass = iSO.mass;
                b.center = iSO.colliderCentre;
                b.size = iSO.colliderSize;
                iState = ItemState.pickup;
                break;
        }

        if(iSO.itemType == ItemSO.ItemType.gun)
        {
            ((GunInfo)this).magAmmo = _aux1;
        }
    }
}
