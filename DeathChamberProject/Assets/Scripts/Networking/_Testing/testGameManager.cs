using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGameManager : MonoBehaviour
{
    public static testGameManager instance;
    public static Dictionary<int, testPlayerManager> players = new Dictionary<int, testPlayerManager>();

    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;


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

    public void SpawnPlayer(int _id, string _username, Vector3 _position, Quaternion _rotation)
    {
        GameObject _player;
        if(_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _position, _rotation);
        }
        else
        {
            _player = Instantiate(playerPrefab, _position, _rotation);
        }

        _player.GetComponent<testPlayerManager>().id = _id;
        _player.GetComponent<testPlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<testPlayerManager>());
    }
}
