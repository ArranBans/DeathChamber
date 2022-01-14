using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ServerToClientId : ushort
{
    welcome = 1,
    spawnPlayer,
    playerPosition,
    playerRotation,
    playerDisconnected,
    spawnItems,
    itemPosition,
    addItemToInventory,
    removeItemFromInventory,
    removeItemPickup,
    changeSelectedItem,
    fireWeapon,
    changeHealth,
    die,
    respawn,
    serverDeploy,
    sendMap,
    spawnEnemy,
    enemyPosition,
    enemyFire,
    enemyDie
}

public enum ClientToServerId : ushort
{
    name = 1,
    welcomeReceived,
    playeMovement,
    UDPTest,
    interact,
    dropItem,
    changeSelectedItem,
    fireWeapon,
    deploy,
    consumable,
    command,
    mapLoaded,
}

public class NetworkManager : MonoBehaviour
{
    private static NetworkManager _instance;
    public static NetworkManager instance
    {
        get => _instance;
        private set
        {
            if (_instance == null)
                _instance = value;
            else if (_instance != value)
            {
                Debug.Log($"{nameof(NetworkManager)} instance already exists, destroying duplicate!");
                Destroy(value);
            }
        }
    }

    [SerializeField] string ip;
    [SerializeField] ushort port;

    public GameObject localPlayerManagerPrefab;
    public GameObject playerManagerPrefab;
    public GameObject localPlayerPrefab;
    public GameObject playerPrefab;

    public Client Client { get; private set; }

    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Client = new Client();
        Client.Connected += DidConnect;
        Client.ConnectionFailed += FailedToConnect;
        Client.Disconnected += DidDisconnect;
        Client.ClientDisconnected += PlayerLeft;
    }

    public void Connect()
    {
        Client.Connect($"{ip}:{port}");
    }

    private void DidConnect(object sender, EventArgs e)
    {
        NetworkUiManager.instance.ConnectedToServer();
        NetworkUiManager.instance.S_SendName();
    }
    private void FailedToConnect(object sender, EventArgs e)
    {
        NetworkUiManager.instance.BackToMain();
    }
    private void DidDisconnect(object sender, EventArgs e)
    {
        NetworkUiManager.instance.BackToMain();
        Disconnect();
    }
    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(testPlayerManager.list[e.Id].gameObject);
    }

    private void FixedUpdate()
    {
        Client.Tick();
    }

    private void OnApplicationQuit()
    {
        Client.Disconnect();
    }

    public void Disconnect()
    {
        Client.Disconnect();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        testGameManager.itemPickups = new List<ItemPickup>();
        testGameManager.enemies = new List<EnemyTest>();
        Destroy(testGameManager.instance);
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
        Destroy(Database.instance.gameObject);
    }

    public void ChangeIp()
    {
        instance.ip = NetworkUiManager.instance.IpField.text;
    }

    public void LoadMap(int _mapId)
    {
        StartCoroutine(LoadAsynchronously(_mapId));
    }

    public IEnumerator LoadAsynchronously(int _mapId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(Database.instance.GetMap(_mapId));

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //Debug.Log($"Loading Map: {progress}");
            MenuOptions.instance.LoadingBar.value = progress;
            yield return null;
        }

        Debug.Log("Loaded Map");
        S_MapLoaded();
    }

    #region Messages
    private static void S_MapLoaded()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.mapLoaded);
        instance.Client.Send(message);
    }
    public static void S_Deploy()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.deploy);
        instance.Client.Send(message);
    }

    [MessageHandler((ushort)ServerToClientId.sendMap)]
    private static void R_SendMap(Message message)
    {
        ushort mID = message.GetUShort();
        Debug.Log("Loading Map " + mID);
        instance.LoadMap(mID);
    }

    [MessageHandler((ushort)ServerToClientId.spawnItems)]
    private static void R_SpawnItem(Message message)
    {
        int _itemId = message.GetInt();
        int _databaseId = message.GetInt();
        Vector3 _pos = message.GetVector3();
        Quaternion _rot = message.GetQuaternion();
        int _aux1 = message.GetInt();
        int _aux2 = message.GetInt();

        GameObject _item = Instantiate(Database.instance.GetItem(_databaseId).empty, _pos, _rot);
        _item.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.pickup, _aux1, _aux2);
        _item.GetComponent<ItemPickup>().id = _itemId;
        testGameManager.itemPickups.Add(_item.GetComponent<ItemPickup>());
    }

    [MessageHandler((ushort)ServerToClientId.spawnEnemy)]
    private static void R_SpawnEnemy(Message message)
    {
        int _enemyId = message.GetInt();
        int _databaseId = message.GetInt();
        Vector3 _pos = message.GetVector3();
        Quaternion _rot = message.GetQuaternion();

        GameObject _enemy = Instantiate(Database.instance.GetEnemy(_databaseId).obj, _pos, _rot);
        _enemy.GetComponent<EnemyTest>().id = _enemyId;
        testGameManager.enemies.Add(_enemy.GetComponent<EnemyTest>());
    }

    [MessageHandler((ushort)ServerToClientId.itemPosition)]
    private static void R_ItemPosition(Message message)
    {
        int _itemId = message.GetInt();
        Vector3 _pos = message.GetVector3();
        Quaternion _rot = message.GetQuaternion();

        foreach (ItemPickup i in testGameManager.itemPickups)
        {
            if (i != null && i.id == _itemId)
                i.UpdateItemState(_pos, _rot);
        }
    }

    [MessageHandler((ushort)ServerToClientId.enemyPosition)]
    private static void R_EnemyPosition(Message message)
    {
        int _enemyId = message.GetInt();
        Vector3 _pos = message.GetVector3();
        Quaternion _rot = message.GetQuaternion();
        bool _moving = message.GetBool();

        foreach (EnemyTest e in testGameManager.enemies)
        {
            if (e != null && e.id == _enemyId)
                e.UpdateEnemyState(_pos, _rot, _moving);
        }
    }

    [MessageHandler((ushort)ServerToClientId.removeItemPickup)]
    private static void R_RemoveItem(Message message)
    {
        int _itemId = message.GetInt();

        foreach (ItemPickup _iPickup in testGameManager.itemPickups)
        {
            if (_iPickup != null)
            {
                if (_iPickup.id == _itemId)
                {
                    Destroy(_iPickup.gameObject);
                    testGameManager.itemPickups[testGameManager.itemPickups.IndexOf(_iPickup)] = null;
                }

            }
        }
    }
    #endregion
}
