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

    void Start()
    {
        rayLocation = transform;
        oldRayPoint = rayLocation.transform.position;
        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * force);
        Destroy(gameObject, 2);
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
                Destroy(gameObject);
            }


        }

        oldRayPoint = rayLocation.position;
    }

}
