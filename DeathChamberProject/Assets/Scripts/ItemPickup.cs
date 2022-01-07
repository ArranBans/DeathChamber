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
        float rotationCap = Mathf.Clamp(testGameManager.instance.StateInterp * Time.deltaTime, 0, 1);
        transform.position = Vector3.Lerp(transform.position, ServerPos, testGameManager.instance.StateInterp * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, ServerRot, rotationCap);
    }

    public void UpdateItemState(Vector3 _pos, Quaternion _rot)
    {
        ServerPos = _pos;
        ServerRot = _rot;
    }
}
