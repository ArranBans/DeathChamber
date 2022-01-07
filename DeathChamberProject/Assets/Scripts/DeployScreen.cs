using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployScreen : MonoBehaviour
{
    public GameObject deployScreen;
    public void Deploy()
    {
        NetworkManager.S_Deploy();
    }
}
