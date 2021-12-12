using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;
    public GameObject playerManagerPrefab;
    public GameObject playerObject;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        
        Server.Start(50, 16699);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public PlayerManager InstantiatePlayerManager()
    {
        return Instantiate(playerManagerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
    }

    public Player InstantiatePlayer(PlayerManager playerManager, Vector3 _SpawnPoint)
    {
        return Instantiate(playerObject, _SpawnPoint, Quaternion.identity, playerManager.transform).GetComponent<Player>();
    }
}
