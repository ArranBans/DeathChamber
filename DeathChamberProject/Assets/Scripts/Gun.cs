using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    GunSO gunSO;
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
    public testPlayerController pTController;
    public Player player;

    private Vector3 _posRecoil;
    private Vector3 _rotRecoil;
    private Vector3 _camPosRecoil;
    private Vector3 _camRotRecoil;
    private bool recoiling;
    private bool aiming;

    void Start()
    {
        gunSO = (GunSO)itemInfo.iSO;
        if (GetComponentInParent<testPlayerController>())
        {
            pTController = GetComponentInParent<testPlayerController>();
        }

        desiredPos = gunSO.hipPos;
        cam = GetComponentInParent<Camera>();
        player = GetComponentInParent<Player>();
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

        #region Mouse Handling
        float[] inputs = new float[]
        {
            Input.GetAxisRaw("Mouse X"),
            Input.GetAxisRaw("Mouse Y")
        };

        if (player.paused)
        {
            inputs = new float[] { 0, 0 };
        }
        
        

        inputs[0] = Mathf.Clamp(inputs[0], -0.6f, 0.6f);
        inputs[1] = Mathf.Clamp(inputs[1], -0.6f, 0.6f);
        #endregion

        #region Firemode Handling
        if (!player.paused)
        {
            switch (fireMode)
            {
                case GunSO.FireMode.boltAction:
                    {
                        //boltfire
                        break;
                    }

                case GunSO.FireMode.semiAuto:
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0)) 
                        {
                            if (Time.time >= timeToNextFire)
                            {
                                Fire();
                                timeToNextFire = Time.time + fireRate;
                            }
                        }
                        break;
                    }
                case GunSO.FireMode.fullAuto:
                    {
                        if (Input.GetKey(KeyCode.Mouse0))
                        {
                            if (Time.time >= timeToNextFire)
                            {
                                Fire();
                                timeToNextFire = Time.time + fireRate;
                            }
                        }
                        break;
                    }
            }
        }

        #endregion

        #region Aiming
        if (!player.paused)
        {
            if (Input.GetKey(KeyCode.Mouse1))
            {
                desiredPos = gunSO.aimPos;
                aiming = true;
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, camFov * gunSO.aimFov, gunSO.aimSpeed * Time.deltaTime);
            }
            else
            {
                desiredPos = gunSO.hipPos;
                aiming = false;
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, camFov, gunSO.aimSpeed * Time.deltaTime);
            }
        }

        #endregion

        #region Recoil
        if (recoiling)
        {
            Recoil();
        }
         //&& recoiling == true
        if (Time.time >= timeToRecoilCompensate)
        {
            recoiling = false;
            transform.localPosition = Vector3.Lerp(transform.localPosition, desiredPos, gunSO.recoilReturnSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, gunSO.recoilReturnSpeed * Time.deltaTime);
        }

        if (Time.time >= timeToCamRecoilCompensate)
        {
            recoiling = false;
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, camDPos, gunSO.recoilReturnSpeed * Time.deltaTime);

            if(gunSO.cameraReturn)
            {
                cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.identity, gunSO.recoilReturnSpeed * Time.deltaTime);
            }
            else
            {
                cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.Euler(cam.transform.localRotation.eulerAngles.x, 0, 0), gunSO.recoilReturnSpeed * Time.deltaTime);
            }
            
        }
        #endregion

        #region WeaponSway
        float _hsway = gunSO.hMoveSway;
        float _vsway = gunSO.vMoveSway;
        float _tiltsway = gunSO.tiltSway;
        if(aiming)
        {
            _hsway *= gunSO.aimSwayModifier;
            _vsway *= gunSO.aimSwayModifier;
            _tiltsway *= 0.05f;
        }


            transform.localPosition = new Vector3(transform.localPosition.x + inputs[0] * _hsway, transform.localPosition.y + inputs[1] * _vsway, transform.localPosition.z);
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, transform.localRotation.eulerAngles.y, transform.localRotation.eulerAngles.z + inputs[0] * _tiltsway);    
        #endregion
    }

    void Fire()
    {
        Debug.Log("Firing");

        //SpawnBullet on client and server
        GameObject bullet = (GameObject)Instantiate(Resources.Load($"Projectiles/{gunSO.itemName}_Projectile"), cam.transform.position, cam.transform.rotation);
        ClientSend.FireWeapon();

        recoiling = true;
        timeToRecoilCompensate = Time.time + gunSO.recoilTime;
        timeToCamRecoilCompensate = Time.time + gunSO.cameraRecoilTime;
    }

    void Recoil()
    {
        //transform.localPosition = new Vector3(transform.localPosition.x + _posRecoil.x, transform.localPosition.y + posRecoil.y, transform.localPosition.z + posRecoil.z);
        //transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x + _rotRecoil.x, transform.localRotation.eulerAngles.y + _rotRecoil.y, transform.localRotation.eulerAngles.z + _rotRecoil.z);

        //cam.transform.localPosition = new Vector3(cam.transform.localPosition.x + camPosRecoil.x, cam.transform.localPosition.y + camPosRecoil.y, cam.transform.localPosition.z + camPosRecoil.z);
        //cam.transform.localRotation = Quaternion.Euler(cam.transform.localRotation.eulerAngles.x + _camRotRecoil.x, cam.transform.localRotation.eulerAngles.y + _camRotRecoil.y, cam.transform.localRotation.eulerAngles.z + _camRotRecoil.z);

        transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(transform.localPosition.x + _posRecoil.x, transform.localPosition.y + posRecoil.y, transform.localPosition.z + posRecoil.z), gunSO.recoilSnappiness * Time.deltaTime * 10000);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(transform.localRotation.eulerAngles.x + _rotRecoil.x, transform.localRotation.eulerAngles.y + _rotRecoil.y, transform.localRotation.eulerAngles.z + _rotRecoil.z), gunSO.recoilSnappiness * Time.deltaTime * 10000);

        if(gunSO.cameraReturn)
        {
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(cam.transform.localPosition.x + camPosRecoil.x, cam.transform.localPosition.y + camPosRecoil.y, cam.transform.localPosition.z + camPosRecoil.z), gunSO.recoilSnappiness * Time.deltaTime * 10000);
            cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.Euler(cam.transform.localRotation.x + _camRotRecoil.x, cam.transform.localRotation.eulerAngles.y + _camRotRecoil.y, cam.transform.localRotation.eulerAngles.z + _camRotRecoil.z), gunSO.recoilSnappiness * Time.deltaTime * 10000);
        }
        else
        {
            pTController._xRot += _camRotRecoil.x;
            pTController._yRot += _camRotRecoil.y;
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(cam.transform.localPosition.x + camPosRecoil.x, cam.transform.localPosition.y + camPosRecoil.y, cam.transform.localPosition.z + camPosRecoil.z), gunSO.recoilSnappiness * Time.deltaTime * 10000);
            cam.transform.localRotation = Quaternion.Lerp(cam.transform.localRotation, Quaternion.Euler(cam.transform.localRotation.eulerAngles.x , cam.transform.localRotation.eulerAngles.y , cam.transform.localRotation.eulerAngles.z + _camRotRecoil.z), gunSO.recoilSnappiness * Time.deltaTime * 10000);
        }

        //return null;
        Debug.Log("Recoiled");
    }
}
