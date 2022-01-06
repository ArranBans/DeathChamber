using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float force;
    public float hitforce;
    public Rigidbody rb;
    public GameObject SmokeEffect;
    public Transform rayLocation;
    public RaycastHit colliderHit;
    Vector3 oldRayPoint;
    public AudioSource hitMarker;
    public int myId;

    void Start()
    {
        rayLocation = transform;
        oldRayPoint = rayLocation.transform.position;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force);
        Destroy(gameObject, 2.5f);
    }
    private void Update()
    {

    }
    private void FixedUpdate()
    {
        Debug.DrawRay(oldRayPoint, rayLocation.position-oldRayPoint, Color.blue, 15f);
        if(Physics.Raycast(oldRayPoint, rayLocation.position-oldRayPoint, out colliderHit, Vector3.Distance(oldRayPoint, rayLocation.position)));
        {
            if (colliderHit.collider)
            {
                print(colliderHit.transform.name);
                Quaternion hitRot = Quaternion.Euler(-transform.rotation.eulerAngles.x, -transform.rotation.eulerAngles.y, -transform.rotation.eulerAngles.z);
                //Instantiate(SmokeEffect, colliderHit.point, hitRot);
                if(colliderHit.collider.GetComponent<Rigidbody>())
                {
                    colliderHit.collider.GetComponent<Rigidbody>().AddForce(rb.velocity*hitforce);
                }

                /*if(myId == Client.instance.myId)
                {
                    if (colliderHit.collider.GetComponent<NetPlayerController>())// Do we hit a player?
                    {
                        Instantiate(colliderHit.collider.GetComponent<NetPlayerController>().bloodFX, colliderHit.point, Quaternion.identity);
                        hitMarker.PlayOneShot(hitMarker.clip);
                    }
                    else if (colliderHit.collider.GetComponentInParent<NetPlayerController>())
                    {
                        Instantiate(colliderHit.collider.GetComponentInParent<NetPlayerController>().bloodFX, colliderHit.point, Quaternion.identity);
                        hitMarker.PlayOneShot(hitMarker.clip);
                    }
                    else if (colliderHit.collider.GetComponent<EnemyTest>())// Do we hit an enemy?
                    {
                        Instantiate(colliderHit.collider.GetComponent<EnemyTest>().bloodFX, colliderHit.point, Quaternion.identity);
                        hitMarker.PlayOneShot(hitMarker.clip);
                    }
                    else if (colliderHit.collider.GetComponentInParent<EnemyTest>())
                    {
                        Instantiate(colliderHit.collider.GetComponentInParent<EnemyTest>().bloodFX, colliderHit.point, Quaternion.identity);
                        hitMarker.PlayOneShot(hitMarker.clip);
                    }
                }*/
                
                Destroy(gameObject);
            }


        }

        oldRayPoint = rayLocation.position;
    }

}
