using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections.Generic;
using UnityEngine;

public class testPlayerManager : MonoBehaviour
{
    public static Dictionary<ushort, testPlayerManager> list = new Dictionary<ushort, testPlayerManager>();

    public ushort id;
    public string username;
    public bool isLocal;
    public GameObject playerObj;

    private void OnDestroy()
    {
        list.Remove(id);
    }

    public static void SpawnManager(ushort _id, string _username, Vector3 _position)
    {
        testPlayerManager playerM;
        if (_id == NetworkManager.instance.Client.Id)
        {
            playerM = Instantiate(NetworkManager.instance.localPlayerManagerPrefab, _position, Quaternion.identity).GetComponent<testPlayerManager>();
            playerM.isLocal = true;
        }
        else
        {
            playerM = Instantiate(NetworkManager.instance.playerManagerPrefab, _position, Quaternion.identity).GetComponent<testPlayerManager>();
            playerM.isLocal = false;
        }

        playerM.name = $"Player {_id} ({(string.IsNullOrEmpty(_username) ? "Guest" : _username)})";
        playerM.id = _id;
        playerM.username = _username;

        list.Add(_id, playerM);
    }

    #region Messages
    [MessageHandler((ushort)ServerToClientId.spawnPlayer)]
    private static void SpawnPlayerManager(Message message)
    {
        SpawnManager(message.GetUShort(), message.GetString(), message.GetVector3());
    }

    #endregion
}
