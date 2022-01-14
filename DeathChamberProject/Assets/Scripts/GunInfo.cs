using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class GunInfo : ItemInfo
{
    [Header("Gun Runtime Stats")]
    public int magAmmo;
    public GunSO.FireMode fireMode;
    public AudioSource fireAudio;
    public VisualEffect muzzleFlash;
    public Light muzzleLight;

    private void Awake()
    {
        muzzleLight = muzzleFlash.GetComponent<Light>();
    }
}
