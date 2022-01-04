using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyTest : MonoBehaviour
{
    public EnemySO eSO;
    public float id;
    Vector3 ServerPos;
    Quaternion ServerRot;
    public Transform attackPoint;

    public GameObject bloodFX;

    private void Update()
    {
        float rotationCap = Mathf.Clamp(testGameManager.instance.StateInterp * Time.deltaTime, 0, 1);
        transform.position = Vector3.Lerp(transform.position, ServerPos, testGameManager.instance.StateInterp * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, ServerRot, rotationCap);
    }

    public void UpdateEnemyState(Vector3 _pos, Quaternion _rot)
    {
        ServerPos = _pos;
        ServerRot = _rot;
    }
}
