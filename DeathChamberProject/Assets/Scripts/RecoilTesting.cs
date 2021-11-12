using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilTesting : MonoBehaviour
{
    
    [Header("Positional Recoil")]
    public float xPosRecoil;
    public float yPosRecoil;
    public float zPosRecoil;
    [Header("Rotational Recoil")]
    public float xRotRecoil;
    public float yRotRecoil;
    public float zRotRecoil;
    [Header("Transforms")]
    public Transform recoilLocation;
    private Vector3 startPos;
    private Quaternion startRot;
    public float recoilReturnStrength;
    public float errorMarginPos;
    public float errorMarginRot;

    private float _xPosRecoil;
    private float _yRotRecoil;
    private float _zRotRecoil;

    private void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;


    }
    void Update()
    {
        _xPosRecoil = Random.Range(+xPosRecoil, -xPosRecoil);
        _yRotRecoil = Random.Range(+yRotRecoil, -yRotRecoil);
        _zRotRecoil = Random.Range(+zRotRecoil, -zRotRecoil);

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire();
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, startPos, recoilReturnStrength * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, startRot, recoilReturnStrength * Time.deltaTime);

        /*if(transform.localPosition.x < startPos.x + errorMarginPos && transform.localPosition.x > startPos.x - errorMarginPos)
        {
            transform.localPosition = startPos;
        }*/
    }

    void Fire()
    {
        transform.localPosition = new Vector3(transform.localPosition.x + _xPosRecoil, transform.localPosition.y + yPosRecoil, transform.localPosition.z + zPosRecoil);
        transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x + xRotRecoil, transform.localRotation.eulerAngles.y + _yRotRecoil, transform.localRotation.eulerAngles.z + _zRotRecoil);
    }
}
