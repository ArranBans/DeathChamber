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
                if (iSO.itemType == ItemSO.ItemType.consumable)
                {
                    Consumable item = gameObject.AddComponent<Consumable>();
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
                    if(gameO.GetComponentInChildren<Camera>())
                    {
                        Destroy(gameO.GetComponentInChildren<Camera>().gameObject);
                    }
                    gameO.SetActive(false);
                }
                break;
            case ItemState.charModel:

                if (iSO.itemType == ItemSO.ItemType.gun)
                {
                    GunSO g = (GunSO)iSO;
                    transform.localPosition = g.hipPos;
                }
                else if (iSO.itemType == ItemSO.ItemType.consumable)
                {
                    ConsumableSO g = (ConsumableSO)iSO;
                    transform.localPosition = g.idlePos;
                }


                foreach (GameObject gameO in ViewmodelOnly)
                {
                    if (gameO.GetComponentInChildren<Camera>())
                    {
                        Destroy(gameO.GetComponentInChildren<Camera>().gameObject);
                    }
                    gameO.SetActive(false);
                }
                break;
        }
    }
}
