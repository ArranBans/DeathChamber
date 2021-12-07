using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NameTag : MonoBehaviour
{
    void Start()
    {
        TextMesh text = GetComponent<TextMesh>();
        text.text  = GetComponentInParent<testPlayerManager>().username;
    }

    
}
