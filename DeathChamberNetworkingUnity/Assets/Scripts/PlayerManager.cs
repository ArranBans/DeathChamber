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
    public bool loaded;
    
    public static void SpawnManager(ushort _id, string _username)
    {
        PlayerManager playerM = Instantiate(NetworkManager.instance.playerManagerPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerManager>();
        playerM.name = $"Player {_id} ({(string.IsNullOrEmpty(_username) ? "Guest" : _username)})";
        playerM.id = _id;
        playerM.username = string.IsNullOrEmpty(_username) ? $"Guest {_id}" : _username;

        playerM.S_SendMap(_id);
        list.Add(_id, playerM);
    }

    private void OnDestroy()
    {
        if (player)
        {
            foreach (Item _i in player.inventory)
            {
                testGameManager.instance.SpawnItem(_i.itemSO.id, player.dropTransform.position, player.dropTransform.rotation);
            }
        }

        list.Remove(id);
    }

    public void SendIntoGame()
    {
        S_SpawnPlayerManager();// Spawn me on everyone

        foreach (PlayerManager _client in list.Values) // Spawn them on me
        {
            if (_client != null)
            {
                if (_client.id != id)
                {
                    _client.S_SpawnPlayerManager(id);
                }

                if (_client.id != id)
                {
                    if (_client.player)
                    {
                        testGameManager.instance.S_Deploy(_client.id, _client.player.transform.position, _client.player.transform.rotation);
                    }
                }
            }
        }

        foreach (ItemPickup _item in testGameManager.instance.items)// Spawn Items on new client
        {
            if (_item != null)
            {
                int _aux1 = 0;
                int _aux2 = 0;
                if (_item.iSO.itemType == ItemSO.ItemType.gun)
                {
                    _aux1 = _item.GetComponent<GunInfo>().magAmmo;
                }


                testGameManager.instance.S_SpawnItem(id, _item.id, _item.iSO.id, _item.transform.position, _item.transform.rotation, _aux1, _aux2);
            }
        }

        foreach (EnemyTest _enemy in testGameManager.instance.enemies)// Spawn Enemies on new client
        {
            if (_enemy != null)
            {
                testGameManager.instance.S_SpawnEnemy(id, _enemy.id, _enemy.eSO.id, _enemy.transform.position, _enemy.transform.rotation);
            }
        }

        loaded = true;
    }

    #region Messages
    private Message AddSpawnData(Message message)
    {
        message.Add(id);
        message.Add(username);
        message.Add(transform.position);
        return message;
    }

    private Message AddMapID(Message message)
    {
        message.Add(testGameManager.instance.mapId);
        return message;
    }

    private void S_SpawnPlayerManager()
    {
        NetworkManager.instance.Server.SendToAll(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.spawnPlayer)));
    }

    private void S_SpawnPlayerManager(ushort _id)
    {
        NetworkManager.instance.Server.Send(AddSpawnData(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.spawnPlayer)), _id);
    }

    private void S_SendMap()
    {
        NetworkManager.instance.Server.SendToAll(AddMapID(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.sendMap)));
    }

    private void S_SendMap(ushort _id)
    {
        NetworkManager.instance.Server.Send(AddMapID(Message.Create(MessageSendMode.reliable, (ushort)ServerToClientId.sendMap)), _id);
    }

    [MessageHandler((ushort)ClientToServerId.name)]
    private static void R_Name(ushort _fromClient, Message _message)
    {
        SpawnManager(_fromClient, _message.GetString());
    }

    [MessageHandler((ushort)ClientToServerId.mapLoaded)]
    private static void R_MapLoaded(ushort _fromClient, Message message)
    {
        Debug.Log($"Client {_fromClient} has loaded map");
        list[_fromClient].SendIntoGame();
    }
    #endregion
}
