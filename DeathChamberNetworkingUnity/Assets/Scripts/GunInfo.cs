using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunInfo : ItemInfo
{
    [Header("Gun Runtime Stats")]
    public int magAmmo;
    public int reserveAmmo;
    public GunSO.FireMode fireMode;
}
