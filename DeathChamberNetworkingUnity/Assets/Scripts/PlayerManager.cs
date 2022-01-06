using RiptideNetworking;
using RiptideNetworking.Utils;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static Dictionary<ushort, PlayerManager> list = new Dictionary<ushort, PlayerManager>();

    public Player player;
    public ushort id;
    public string username;
    
    public static void SpawnManager(ushort _id, string _username)
    {
        foreach (PlayerManager _otherPlayer in list.Values)
        {
            _otherPlayer.SpawnPlayerManager(_id);
        }


        PlayerManager playerM = Instantiate(NetworkManager.instance.playerManagerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
        playerM.name = $"Player {_id} ({(string.IsNullOrEmpty(_username) ? "Guest" : _username)})";
        playerM.id = _id;
        playerM.username = string.IsNullOrEmpty(_username) ? $"Guest {_id}" : _username;

        playerM.SpawnPlayerManager();
        list.Add(_id, playerM);
    }

    private void OnDestroy()
    {
        list.Remove(id);
    }

    #region Messages
    private Message AddSpawnData(Message message)
    {
        message.AddUShort(id);
        message.AddString(username);
        message.AddVector3(transform.position);
        return message;
    }

    private void SpawnPlayerManager()
    {
        NetworkManager.instance.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.spawnPlayer)));
    }

    private void SpawnPlayerManager(ushort _id)
    {
        NetworkManager.instance.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.spawnPlayer)), _id);
    }


    [MessageHandler((ushort)ClientToServerId.name)]
    private static void Name(ushort _fromClient, Message _message)
    {
        SpawnManager(_fromClient, _message.GetString());
    }
    #endregion
}
