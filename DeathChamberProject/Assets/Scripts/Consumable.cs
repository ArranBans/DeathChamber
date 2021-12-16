using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item
{
    ConsumableSO conSO;
    private float currentUsage;

    private void Start()
    {
        conSO = (ConsumableSO)itemInfo.iSO;
    }

    private void Update()
    {
        if(Input.GetKey(KeyCode.Mouse0))
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, conSO.usePos, conSO.beginUseTime * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(conSO.useRot), conSO.beginUseTime * Time.deltaTime);
            currentUsage += Time.deltaTime;
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, conSO.idlePos, conSO.beginUseTime * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(conSO.idleRot), conSO.beginUseTime * Time.deltaTime);
        }
        if(Input.GetKeyUp(KeyCode.Mouse0))
        {
            currentUsage = 0;
        }

        if(currentUsage >= conSO.useTime)
        {
            Debug.Log("Item used");
            ClientSend.Consumable();
            currentUsage = 0;
        }
    }
}
