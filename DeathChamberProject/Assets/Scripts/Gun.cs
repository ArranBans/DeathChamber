using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GunSO gunSO;
    private float timeToNextFire;
    private float timeToRecoilCompensate;
    private float timeToCamRecoilCompensate;
    private float fireRate;
    private Camera cam;
    private float camFov;
    private Vector3 camDPos;
    private Vector3 camDRot;
    private float magAmmo;
    private float reserveAmmo;
    public GunSO.FireMode fireMode;
    private Vector3 posRecoil;
    private Vector3 rotRecoil;
    private Vector3 camPosRecoil;
    private Vector3 camRotRecoil;
    private Vector3 desiredPos;
    private Vector3 desiredRot;
    public PlayerTestController pTController;

    private Vector3 _posRecoil;
    private Vector3 _rotRecoil;
    private Vector3 _camPosRecoil;
    private Vector3 _camRotRecoil;

    void Start()
    {
        pTController = GetComponentInParent<PlayerTestController>();
        desiredPos = gunSO.hipPos;
        cam = GetComponentInParent<Camera>();
        camFov = cam.fieldOfView;
        camDPos = cam.transform.localPosition;
        camDRot = new Vector3(cam.transform.localRotation.eulerAngles.x, cam.transform.localRotation.eulerAngles.y, cam.transform.localRotation.eulerAngles.z);
        fireMode = gunSO.defaultFireMode;
        reserveAmmo = gunSO.maxAmmo;
        magAmmo = gunSO.magAmmo;
        fireRate = 1 / gunSO.fireRate;

        posRecoil = new Vector3(gunSO.xPosRecoil, gunSO.yPosRecoil, gunSO.zPosRecoil);
        rotRecoil = new Vector3(gunSO.xRotRecoil, gunSO.yRotRecoil, gunSO.zRotRecoil);

        camPosRecoil = new Vector3(gunSO.camXPosRecoil, gunSO.camYPosRecoil, gunSO.camZPosRecoil);
        camRotRecoil = new Vector3(gunSO.camXRotRecoil, gunSO.camYRotRecoil, gunSO.camZRotRecoil);

    }

    void Update()
    {
        #region Recoil Randomness
        _posRecoil.x = Random.Range(+posRecoil.x, -posRecoil.x);

        _rotRecoil.x = Random.Range(+rotRecoil.x, -rotRecoil.x) + gunSO.xRotOffset;
        _camRotRecoil.x = Random.Range(+camRotRecoil.x, -camRotRecoil.x) + gunSO.camXRotOffset;

        _rotRecoil.y = Random.Range(+rotRecoil.y, -rotRecoil.y);
        _camRotRecoil.y = Random.Range(+camRotRecoil.y, -camRotRecoil.y);

        _rotRecoil.z = Random.Range(+rotRecoil.z, -rotRecoil.z);
        _camRotRecoil.z = Random.Range(+camRotRecoil.z, -camRotRecoil.z);
        #endregion

        switch (fireMode)
        {
            case GunSO.FireMode.boltAction:
                {
                    //boltfire
                    break;
                }

            case GunSO.FireMode.semiAuto:
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0)) { Fire(); }
                    break;
                }
            case GunSO.FireMode.fullAuto:
                {
                    if (Input.GetKey(KeyCode.Mouse0)) 
                    { 
                        if(Time.time >= timeToNextFire)
                        {
                            Fire();
                            timeToNextFire = Time.time + fireRate;
                        }
                        
                    }
                    break;
                }
        }

        if(Input.GetKey(KeyCode.Mouse1))
        {
            desiredPos = gunSO.aimPos;
        }
        else
        {
            desiredPos = gunSO.hipPos;
        }

        if(Time.time >= timeToRecoilCompensate)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, gunSO.recoilReturnSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, gunSO.recoilReturnSpeed * Time.deltaTime);
        }

        if (Time.time >= timeToCamRecoilCompensate)
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, camDPos, gunSO.recoilReturnSpeed * Time.deltaTime);
            cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.identity, gunSO.recoilReturnSpeed * Time.deltaTime);
        }
        //Debug.Log("Recoil Returned");
    }

    void Fire()
    {
        Debug.Log("Firing");
        //GameObject bullet = Instantiate(gunSO.bulletObject, gunSO.bulletSpawnPoint.position, gunSO.bulletSpawnPoint.rotation);
        Recoil(); 
    }
    void Recoil()
    {
        transform.localPosition = new Vector3(transform.localPosition.x + _posRecoil.x, transform.localPosition.y + posRecoil.y, transform.localPosition.z + posRecoil.z);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x + _rotRecoil.x, transform.localRotation.eulerAngles.y + _rotRecoil.y, transform.localRotation.eulerAngles.z + _rotRecoil.z);

        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x + camPosRecoil.x, cam.transform.localPosition.y + camPosRecoil.y, cam.transform.localPosition.z + camPosRecoil.z);
        cam.transform.localRotation = Quaternion.Euler(cam.transform.localRotation.eulerAngles.x + _camRotRecoil.x, cam.transform.localRotation.eulerAngles.y + _camRotRecoil.y, cam.transform.localRotation.eulerAngles.z + _camRotRecoil.z);
        
        //return null;
        Debug.Log("Recoiled");

        timeToRecoilCompensate = Time.time + gunSO.recoilTime;
        timeToCamRecoilCompensate = Time.time + gunSO.cameraRecoilTime;
    }
}
