using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FastIK : MonoBehaviour
{
    public int chainLength = 2;
    public Transform target;
    public Transform pole;


    float[] bonesLength;
    float completeLength;
    Transform[] bones;
    Vector3[] positions;
    Vector3[] startDirectionSucc;
    Quaternion[] startRotationBone;
    Quaternion startRotationTarget;
    Quaternion startRotationRoot;

    public int iterations;
    public float delta;
    public float snapBackStrength;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        bones = new Transform[chainLength + 1];
        positions = new Vector3[chainLength + 1];
        bonesLength = new float[chainLength];
        startDirectionSucc = new Vector3[chainLength + 1];
        startRotationBone = new Quaternion[chainLength + 1];

        if(target == null)
        {
            target = new GameObject(gameObject.name + " Target").transform;
            target.position = transform.position;
        }
        startRotationTarget = target.rotation;
        completeLength = 0;

        //init data
        var current = transform;
        for(var i = bones.Length - 1; i >= 0; i--)
        {
            bones[i] = current;
            startRotationBone[i] = current.rotation;

            if(i == bones.Length - 1) //leaf bone
            {
                startDirectionSucc[i] = target.position - current.position;
            }
            else //mid bone
            {
                startDirectionSucc[i] = bones[i + 1].position - current.position;
                bonesLength[i] = startDirectionSucc[i].magnitude;
                completeLength += bonesLength[i];
            }

            current = current.parent;
        }

    }

    private void LateUpdate()
    {
        ResolveIK();
    }

    void ResolveIK()
    {
        if (target == null)
            return;

        if (bonesLength.Length != chainLength)
            Init();

        for (int i = 0; i < bones.Length; i++) // Get Position
            positions[i] = bones[i].position;

        var rootRot = (bones[0].parent != null) ? bones[0].parent.rotation : Quaternion.identity;
        var roorRotDiff = rootRot * Quaternion.Inverse(startRotationRoot);


        if((target.position - bones[0].position).sqrMagnitude >= completeLength * completeLength)
        {
            var direction = (target.position - positions[0]).normalized;

            for (int i = 1; i < positions.Length; i++)
                positions[i] = positions[i - 1] + direction * bonesLength[i - 1];
        }
        else
        {
            for (int i = 0; i < positions.Length - 1; i++)
                positions[i + 1] = Vector3.Lerp(positions[i + 1], positions[i] + roorRotDiff * startDirectionSucc[i], snapBackStrength);

            for (int _iteration = 0; _iteration < iterations; _iteration++)
            {
                for(int i = positions.Length - 1; i > 0; i--) // Back
                {
                    if (i == positions.Length - 1)
                        positions[i] = target.position;
                    else
                        positions[i] = positions[i + 1] + (positions[i] - positions[i + 1]).normalized * bonesLength[i];
                }

                for (int i = 1; i < positions.Length; i++) // Forward
                    positions[i] = positions[i - 1] + (positions[i] - positions[i - 1]).normalized * bonesLength[i - 1];

                if ((positions[positions.Length - 1] - target.position).sqrMagnitude < delta * delta) // Close enough
                    break;
            }
        }

        if(pole != null) //Move towards pole
        {
            for(int i = 1; i < positions.Length - 1; i++)
            {
                var plane = new Plane(positions[i + 1] - positions[i - 1], positions[i - 1]);
                var projectedPole = plane.ClosestPointOnPlane(pole.position);
                var projectedBone = plane.ClosestPointOnPlane(positions[i]);
                var angle = Vector3.SignedAngle(projectedBone - positions[i - 1], projectedPole - positions[i - 1], plane.normal);
                positions[i] = Quaternion.AngleAxis(angle, plane.normal) * (positions[i] - positions[i - 1]) + positions[i - 1];
            }
        }

        for (int i = 0; i < positions.Length; i++) // Set Position and rotation
        {
            if (i == positions.Length - 1)
                bones[i].rotation = target.rotation * Quaternion.Inverse(startRotationTarget) * startRotationBone[i];
            else
                bones[i].rotation = Quaternion.FromToRotation(startDirectionSucc[i], positions[i + 1] - positions[i]) * startRotationBone[i];
            bones[i].position = positions[i];
        }
            
    }

    private void OnDrawGizmos()
    {
        var current = this.transform;
        for(int i = 0; i < chainLength && current != null && current.parent != null; i++)
        {
            var scale = Vector3.Distance(current.position, current.parent.position) * 0.1f;
            //Handles.matrix = Matrix4x4.TRS(current.position, Quaternion.FromToRotation(Vector3.up, current.parent.position - current.position), new Vector3(scale, Vector3.Distance(current.parent.position, current.position), scale));
            //Handles.color = Color.green;
            //Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);
            current = current.parent;
        }
    }
}
