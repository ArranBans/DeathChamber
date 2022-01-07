using RiptideNetworking;
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

    #region Messages
    [MessageHandler((ushort)ServerToClientId.enemyFire)]
    private static void R_EnemyFire(Message message)
    {
        int _enemyId = message.GetInt();
        Quaternion _fireRot = message.GetQuaternion();

        foreach (EnemyTest e in testGameManager.enemies)
        {
            if (e.id == _enemyId)
            {
                GameObject projectile = Instantiate(e.eSO.projectile, e.attackPoint.position, _fireRot);
                break;
            }
            else
            {
                continue;
            }

        }
    }

    [MessageHandler((ushort)ServerToClientId.enemyDie)]
    private static void R_EnemyDie(Message message)
    {
        int _enemyId = message.GetInt();
        foreach (EnemyTest e in testGameManager.enemies)
        {
            if (e.id == _enemyId)
            {
                Destroy(e.gameObject);
                break;
            }
            else
            {
                continue;
            }

        }
    }
    #endregion
}
