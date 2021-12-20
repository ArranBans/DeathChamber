using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilTesting : MonoBehaviour
{

    public AnimationCurve recoilCurve;
    public float recoilTime;
    public Vector3 recoiliPosition;
    public Vector3 recoilRotation;
    public float delta;
    public float deltaCorrectionSnappiness;
    public float fireRate;

    float timeToNextFire;
    float timer = 0;
    float normalisedRecoilTime = 0;
    Vector3 startingPos;
    Vector3 desiredPos;

    private void Start()
    {
        timer = recoilTime;
        startingPos = transform.localPosition;
        desiredPos = transform.localPosition;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if(Input.GetKey(KeyCode.Mouse0) && Time.time >= timeToNextFire)
        {
            timer = 0;
            startingPos = transform.localPosition;
            timeToNextFire += 1 / fireRate;
        }

        normalisedRecoilTime = timer / recoilTime;
        float normalisedRecoil = recoilCurve.Evaluate(normalisedRecoilTime);
        transform.localPosition = desiredPos + (recoiliPosition * normalisedRecoil);
        transform.localRotation = Quaternion.Euler(recoilRotation * normalisedRecoil);
        
        if((transform.localPosition - startingPos).sqrMagnitude <= delta * delta)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startingPos, deltaCorrectionSnappiness * Time.deltaTime);
        }

    }
}
