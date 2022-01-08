using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGameManager : MonoBehaviour
{
    public static testGameManager instance;
    [SerializeField] public static List<ItemPickup> itemPickups = new List<ItemPickup>();
    [SerializeField] public static List<EnemyTest> enemies = new List<EnemyTest>();

    

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

    public void SpawnPlayer(int _id, Vector3 _pos, Quaternion _rot)
    {
        GameObject _player;
        if (_id == NetworkManager.instance.Client.Id)
        {
            _player = Instantiate(NetworkManager.instance.localPlayerPrefab, _pos, _rot, testPlayerManager.list[(ushort)_id].transform);
            testPlayerManager.list[(ushort)_id].playerObj = _player;
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<Player>().pManager = testPlayerManager.list[(ushort)_id].GetComponent<PauseManager>();
            testPlayerManager.list[(ushort)_id].GetComponent<PauseManager>().player = testPlayerManager.list[(ushort)_id].playerObj.GetComponent<Player>();
            testPlayerManager.list[(ushort)_id].GetComponent<DeployScreen>().deployScreen.SetActive(false);
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<Player>().HudCanvas.gameObject.SetActive(true);

            if(testPlayerManager.list[(ushort)_id].GetComponent<ChatWindow>().chatWindowOpen)
            {
                testPlayerManager.list[(ushort)_id].GetComponent<ChatWindow>().ChatWindowOpen(true);
            }
            else
            {
                testPlayerManager.list[(ushort)_id].GetComponent<ChatWindow>().ChatWindowOpen(false);
            }
        }
        else
        {
            _player = Instantiate(NetworkManager.instance.playerPrefab, _pos, _rot, testPlayerManager.list[(ushort)_id].transform);
        }

        testPlayerManager.list[(ushort)_id].playerObj = _player;
    }
}
