using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using UnityEngine;


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
        NetworkUiManager.instance.SendName();
    }
    private void FailedToConnect(object sender, EventArgs e)
    {
        NetworkUiManager.instance.BackToMain();
    }
    private void DidDisconnect(object sender, EventArgs e)
    {
        NetworkUiManager.instance.BackToMain();
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

    public void ChangeIp()
    {
        instance.ip = NetworkUiManager.instance.IpField.text;
    }
}
