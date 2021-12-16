using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Consumable : Item
{
    public ConsumableSO conSO;
    private void Start()
    {
        conSO = (ConsumableSO)itemInfo.iSO;
        itemSO = conSO;
    }
}
