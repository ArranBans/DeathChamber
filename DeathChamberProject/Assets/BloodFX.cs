using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodFX : MonoBehaviour
{
    private void Start()
    {
        ParticleSystem p = GetComponent<ParticleSystem>();
        Destroy(gameObject, p.main.duration);
    }
}
