using RiptideNetworking;
using RiptideNetworking.Utils;
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
    public static NetworkManager instance;
    public GameObject playerManagerPrefab;
    public GameObject playerObject;

    [SerializeField] ushort port = 16699;
    [SerializeField] ushort maxClientCount = 30;

    public Server Server { get; private set; }

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


        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);

        Server = new Server();
        Server.Start(port, maxClientCount);
        Server.ClientDisconnected += PlayerLeft;
    }


    private void FixedUpdate()
    {
        Server.Tick();
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

    private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
    {
        Destroy(PlayerManager.list[e.Id].gameObject);
    }
}
