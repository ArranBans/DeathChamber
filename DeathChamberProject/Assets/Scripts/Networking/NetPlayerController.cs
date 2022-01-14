using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetPlayerController : MonoBehaviour
{
    public Vector3 DesiredPos;
    public Quaternion DesiredRot;
    public float interpolationRate;
    public Transform camTransform;
    public GameObject selectedItem;
    public ItemInfo selectedItemInfo;
    public Animator anim;
    public float speedChange;
    int Direction = 0;
    bool sprinting = false;
    public GameObject rightHandObj;
    public GameObject leftHandObj;
    Vector3 rightHandIdle;
    Vector3 leftHandIdle;
    Quaternion rightHandRotIdle;
    Quaternion leftHandRotIdle;
    public FastIK rightHandIK;
    public FastIK leftHandIK;
    public GameObject bloodFX;
    float timeToLightOff;
    bool lightOn;

    private void Awake()
    {
        rightHandIdle = rightHandObj.transform.localPosition;
        leftHandIdle = leftHandObj.transform.localPosition;

        rightHandRotIdle = rightHandObj.transform.localRotation;
        leftHandRotIdle = leftHandObj.transform.localRotation;
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, DesiredPos, interpolationRate * 15 * Time.deltaTime);
        transform.rotation = DesiredRot;

        if (Direction == 1)
        {
            if (sprinting)
            {
                anim.SetFloat("MoveSpeed", Mathf.Lerp(anim.GetFloat("MoveSpeed"), 1f, speedChange * Time.deltaTime));
            }
            else
            {
                anim.SetFloat("MoveSpeed", Mathf.Lerp(anim.GetFloat("MoveSpeed"), 0.5f, speedChange * Time.deltaTime));
            }
        }
        else if(Direction == 0)
        {
            anim.SetFloat("MoveSpeed", Mathf.Lerp(anim.GetFloat("MoveSpeed"), 0f, speedChange * Time.deltaTime));
        }

        if (selectedItem != null)// if i am holding an item
        {
            rightHandObj.transform.position = selectedItemInfo.RightHandTarget.position;
            leftHandObj.transform.position = selectedItemInfo.LeftHandTarget.position;
            rightHandIK.enabled = true;
            leftHandIK.enabled = true;
}
        else // if i am not holding an item
        {
            rightHandObj.transform.localPosition = rightHandIdle;
            leftHandObj.transform.localPosition = leftHandIdle;
            rightHandIK.enabled = false;
            leftHandIK.enabled = false;
        }
        rightHandObj.transform.localRotation = rightHandRotIdle;
        leftHandObj.transform.localRotation = leftHandRotIdle;


        if(lightOn && Time.time >= timeToLightOff)
        {
            selectedItem.GetComponent<GunInfo>().muzzleLight.enabled = false;
            lightOn = false;
        }
    }

    public void PlayerMoved(bool[] inputs)
    {
        sprinting = false;
        Direction = 0;
        anim.SetBool("Moving", false);

        if (inputs[0])
        {
            Direction = 1;
            anim.SetBool("Moving", true);
        }
        else if (inputs[1])
        {
            Direction = 2;
            anim.SetBool("Moving", true);
        }
        else if (inputs[2])
        {
            Direction = 3;
            anim.SetBool("Moving", true);
        }
        else if (inputs[3])
        {
            Direction = 4;
            anim.SetBool("Moving", true);
        }

        if (inputs[4])
        {
            
        }
        if (inputs[5])
        {
            sprinting = true;
        }
    }

    public void FireWeapon(int _id)
    {
        Instantiate(Resources.Load($"Projectiles/{Database.instance.GetItem(_id).itemName}_Projectile"), camTransform.position, camTransform.rotation);
        if (selectedItem)
           selectedItem.GetComponent<GunInfo>().fireAudio.PlayOneShot(selectedItem.GetComponent<GunInfo>().fireAudio.clip);
        selectedItem.GetComponent<GunInfo>().muzzleFlash.Play();
        selectedItem.GetComponent<GunInfo>().muzzleLight.enabled = true;

        timeToLightOff = Time.time + 0.015f;
        lightOn = true;
    }
}
