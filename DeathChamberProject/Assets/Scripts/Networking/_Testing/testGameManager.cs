using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGameManager : MonoBehaviour
{
    public static testGameManager instance;
    public static Dictionary<int, testPlayerManager> players = new Dictionary<int, testPlayerManager>();
    [SerializeField] public static List<ItemPickup> itemPickups = new List<ItemPickup>();
    [SerializeField] public static List<EnemyTest> enemies = new List<EnemyTest>();

    public GameObject localPlayerManagerPrefab;
    public GameObject playerManagerPrefab;
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    public float StateInterp;

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

    private void FixedUpdate()
    {
        //Debug.Log($"{itemPickups.Count}");
    }

    public void SpawnPlayerManager(int _id, string _username)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerManagerPrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            _player = Instantiate(playerManagerPrefab, Vector3.zero, Quaternion.identity);
        }

        _player.GetComponent<testPlayerManager>().id = _id;
        _player.GetComponent<testPlayerManager>().username = _username;
        players.Add(_id, _player.GetComponent<testPlayerManager>());
    }

    public void SpawnPlayer(int _id, Vector3 _pos, Quaternion _rot)
    {
        GameObject _player;
        if (_id == Client.instance.myId)
        {
            _player = Instantiate(localPlayerPrefab, _pos, _rot, players[_id].transform);
            players[_id].playerObj = _player;
            players[_id].playerObj.GetComponent<Player>().pManager = players[_id].GetComponent<PauseManager>();
            players[_id].GetComponent<PauseManager>().player = players[_id].playerObj.GetComponent<Player>();
            players[_id].GetComponent<DeployScreen>().deployScreen.SetActive(false);
            players[_id].playerObj.GetComponent<Player>().HudCanvas.gameObject.SetActive(true);
        }
        else
        {
            _player = Instantiate(playerPrefab, _pos, _rot, players[_id].transform);
        }

        players[_id].playerObj = _player;
    }
}