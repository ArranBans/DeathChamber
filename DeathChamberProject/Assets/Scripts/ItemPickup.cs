using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemSO iSO;
    public float id;
    Vector3 ServerPos;
    Quaternion ServerRot;

    public ItemSO Pickup()
    {
        return iSO;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, ServerPos, testGameManager.instance.StateInterp * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, ServerRot, testGameManager.instance.StateInterp * Time.deltaTime);
    }

    public void UpdateItemState(Vector3 _pos, Quaternion _rot)
    {
        ServerPos = _pos;
        ServerRot = _rot;
    }
}
