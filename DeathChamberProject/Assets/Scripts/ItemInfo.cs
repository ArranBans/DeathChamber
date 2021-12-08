using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    [Header("Static Info")]
    public ItemSO iSO;
    public List<GameObject> ViewmodelOnly = new List<GameObject>();
    public enum ItemState
    {
        item,
        pickup,
        charModel
    }
    [Header("Runtime Stats")]
    public ItemState iState;

    public void ChangeState(ItemState _state)
    {
        switch (_state)
        {
            case ItemState.item:
                if (iSO.itemType == ItemSO.ItemType.gun)
                {
                    Gun gun = gameObject.AddComponent<Gun>();
                    gun.itemInfo = this;
                }
                if (iSO.itemType == ItemSO.ItemType.singleUse)
                {
                    Item item = gameObject.AddComponent<Item>();
                    item.itemInfo = this;
                }
                iState = ItemState.item;
                break;
            case ItemState.pickup:
                //Rigidbody rb = gameObject.AddComponent<Rigidbody>();
                BoxCollider b = gameObject.AddComponent<BoxCollider>();
                ItemPickup i = gameObject.AddComponent<ItemPickup>();
                i.iSO = iSO;
                //rb.mass = iSO.mass;
                b.center = iSO.colliderCentre;
                b.size = iSO.colliderSize;
                iState = ItemState.pickup;
                foreach(GameObject gameO in ViewmodelOnly)
                {
                    gameO.SetActive(false);
                }
                break;
            case ItemState.charModel:
                GunSO g = (GunSO)iSO;
                transform.localPosition = g.hipPos;
                foreach (GameObject gameO in ViewmodelOnly)
                {
                    gameO.SetActive(false);
                }
                break;
        }
    }
}
