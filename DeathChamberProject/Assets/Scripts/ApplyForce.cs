using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyForce : MonoBehaviour
{
    Rigidbody rb;
    public float force;
    private enum Direction
    {
        x = 1,
        y,
        z
    }

    [SerializeField]
    private Direction direction;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Force();
        }
    }

    void Force()
    {
        switch (direction)
        {
            case Direction.x:
                rb.AddForce(new Vector3(1, 0, 0) * force);
                break;
            case Direction.y:
                rb.AddForce(new Vector3(0, 1, 0) * force);
                break;
            case Direction.z:
                rb.AddForce(new Vector3(0, 0, 1) * force);
                break;
        }
    }
}
