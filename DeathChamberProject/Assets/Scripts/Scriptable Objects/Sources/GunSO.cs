using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gun", menuName = "ScriptableObjects/Gun")]
public class GunSO : ItemSO
{
    [Header("Positional Recoil")]
    [Header("Gun Scriptable Object")]
    public float xPosRecoil;
    public float yPosRecoil;
    public float zPosRecoil;
    [Header("Rotational Recoil")]
    public float xRotRecoil;
    public float yRotRecoil;
    public float zRotRecoil;
    public float xRotOffset;
    [Header("Camera Positional Recoil")]
    public float camXPosRecoil;
    public float camYPosRecoil;
    public float camZPosRecoil;
    [Header("Camera Rotational Recoil")]
    public float camXRotRecoil;
    public float camYRotRecoil;
    public float camZRotRecoil;
    public float camXRotOffset;
    [Header("Misc Recoil")]
    public Transform recoilLocation;
    public float recoilReturnSpeed;
    public float recoilSnappiness;
    public float recoilTime;
    public float cameraRecoilTime;
    public bool cameraReturn;
    [Header("Firemode Variables")]
    public bool semiAuto;
    public bool fullAuto;
    public bool boltAction;
    public float fireRate;
    public enum FireMode
    {
        semiAuto,
        fullAuto,
        boltAction
    }
    public FireMode defaultFireMode;
    [Header("Ammo Variables")]
    public float maxAmmo;
    public float magAmmo;
    public enum AmmoType
    {
        rifleAmmo,
        pistolAmmo,
        shotgunAmmo,
        rocketAmmo,
        sniperAmmo
    }
    public AmmoType ammoType;
    [Header("Bullet Variables")]
    public float baseDamage;
    public float bulletSpeed;
    public float bulletMass;
    public Transform bulletSpawnPoint;
    public GameObject bulletObject;
    [Header("Weapon Sway Variables")]
    public float tiltSway;
    public float hMoveSway;
    public float vMoveSway;
    public float aimSwayModifier;
    [Header("Weapon FX")]
    public GameObject muzzleFlash;
    public AudioClip gunAudio;
    [Header("Aiming Variables")]
    public float aimSpeed;
    public Vector3 aimPos;
    public Vector3 hipPos;
    public float aimFov;
}
