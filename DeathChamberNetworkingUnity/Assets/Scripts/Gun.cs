using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    public GunSO gunSO;
    private float timeToNextFire;
    private float fireRate;
    private int magAmmo;
    private int reserveAmmo;
    public GunSO.FireMode fireMode;

    void Start()
    {
        gunSO = (GunSO)itemInfo.iSO;
        itemSO = gunSO;
        fireMode = gunSO.defaultFireMode;
        reserveAmmo = gunSO.maxAmmo;
        magAmmo = gunSO.magAmmo;
        fireRate = 1 / gunSO.fireRate;

    }
}
