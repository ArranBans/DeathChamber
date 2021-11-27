using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    public GunSO gunSO;
    private float timeToNextFire;
    private float fireRate;
    private float magAmmo;
    private float reserveAmmo;
    public GunSO.FireMode fireMode;

    void Start()
    {
        itemSO = gunSO;
        fireMode = gunSO.defaultFireMode;
        reserveAmmo = gunSO.maxAmmo;
        magAmmo = gunSO.magAmmo;
        fireRate = 1 / gunSO.fireRate;

    }

    public void Fire()
    {
        Debug.Log("Firing");
        //GameObject bullet = Instantiate(gunSO.bulletObject, gunSO.bulletSpawnPoint.position, gunSO.bulletSpawnPoint.rotation); <-------------- SPAWN BULLET TO KILL THINGS
    }
}
