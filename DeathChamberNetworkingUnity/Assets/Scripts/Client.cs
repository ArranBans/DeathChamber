using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/*
public class Client
{
    public static int dataBufferSize = 4096;
    public int id;
    public PlayerManager playerManager;
    public TCP tcp;
    public UDP udp;
    public bool loaded;

    public Client(int _clientId)
    {
        id = _clientId;
        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;

        private readonly int id;
        public NetworkStream stream;
        private Packet receivedData;
        private byte[] receiveBuffer;


        public TCP(int _id)
        {
            id = _id;
        }

        public void Connect(TcpClient _socket)
        {
            socket = _socket;
            socket.ReceiveBufferSize = dataBufferSize;
            socket.SendBufferSize = dataBufferSize;

            stream = socket.GetStream();
            receivedData = new Packet();
            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(id, $"Welcome to server: {id}!");
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error sending data to player {id} voa TCP: {_ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult _result)
        {
            try
            {
                int _byteLength = stream.EndRead(_result);
                if (_byteLength <= 0)
                {
                    Server.clients[id].Disconnect();
                    return;
                }

                byte[] _data = new byte[_byteLength];
                Array.Copy(receiveBuffer, _data, _byteLength);

                receivedData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error receiving TCP data: {_ex}");
                Server.clients[id].Disconnect();
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;
            receivedData.SetBytes(_data);

            if (receivedData.UnreadLength() >= 4)
            {
                _packetLength = receivedData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }

            while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
            {
                byte[] _packetbytes = receivedData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetbytes))
                    {
                        int _packetID = _packet.ReadInt();
                        Server.packetHandlers[_packetID](id, _packet);
                    }
                });

                _packetLength = 0;

                if (receivedData.UnreadLength() >= 4)
                {
                    _packetLength = receivedData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (_packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect()
        {
            socket.Close();
            stream = null;
            receiveBuffer = null;
            receivedData = null;
            socket = null;
        }
    }

    public class UDP
    {
        public IPEndPoint endPoint;
        private int id;

        public UDP(int _id)
        {
            id = _id;
        }

        public void Connect(IPEndPoint _endPoint)
        {
            endPoint = _endPoint;
        }

        public void SendData(Packet _packet)
        {
            Server.SendUDPData(endPoint, _packet);
        }

        public void HandleData(Packet _packetData)
        {
            int _packetLength = _packetData.ReadInt();
            byte[] _packetBytes = _packetData.ReadBytes(_packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet _packet = new Packet(_packetBytes))
                {
                    int _packetId = _packet.ReadInt();
                    Server.packetHandlers[_packetId](id, _packet);
                }
            });
        }

        public void Disconnect()
        {
            endPoint = null;
        }
    }

    public void SetupPlayerManger(string _playername)
    {
        playerManager = NetworkManager.instance.InstantiatePlayerManager();
        playerManager.username = _playername;
        playerManager.id = id;
    }

    public void SendIntoGame()
    {
        

        foreach (Client _client in Server.clients.Values) // Spawn them on me
        {
            if (_client.playerManager != null)
            {
                if (_client.id != id)
                {
                    ServerSend.SpawnPlayer(id, _client.playerManager);

                    if (Server.clients[_client.id].playerManager.player)
                    {
                        ServerSend.Deploy(_client.id, Server.clients[_client.id].playerManager.player.transform.position, Server.clients[_client.id].playerManager.player.transform.rotation);
                    }
                }
                
            }

        }

        foreach (Client _client in Server.clients.Values) // Spawn me on everyone
        {
            if (_client.playerManager != null)
            {
                ServerSend.SpawnPlayer(_client.id, playerManager);
            }

        }

        foreach (Client _client in Server.clients.Values)//spawn Items on new joined client
        {
            if (_client.playerManager != null)
            {
                if(_client.id == id)
                {
                    
                    foreach (ItemPickup _item in testGameManager.instance.items)
                    {
                        if(_item != null)
                        {
                            int _aux1 = 0;
                            int _aux2 = 0;
                            if(_item.iSO.itemType == ItemSO.ItemType.gun)
                            {
                                _aux1 = _item.GetComponent<GunInfo>().magAmmo;
                            }


                            ServerSend.SpawnItem(id, _item.id, _item.iSO.id, _item.transform.position, _item.transform.rotation, _aux1, _aux2);
                        }
                        
                    }
                }
                 
            }
            else
            {
                
            }
        }

        foreach (Client _client in Server.clients.Values)//spawn Enemies on new joined client
        {
            if (_client.playerManager != null)
            {
                if (_client.id == id)
                {

                    foreach (EnemyTest _enemy in testGameManager.instance.enemies)
                    {
                        if (_enemy != null)
                        {
                            ServerSend.SpawnEnemy(id, _enemy.id, _enemy.eSO.id, _enemy.transform.position, _enemy.transform.rotation);
                        }

                    }
                }

            }
            else
            {
                
            }
        }
    }


    public IEnumerator Respawn(int id)
    {
        yield return new WaitForSeconds(2.5f);
        testGameManager.instance.Respawn(id);
        Debug.Log("Respawned Player: " + id);
    }

    private void Disconnect()
    {
        Debug.Log($"{tcp.socket.Client.RemoteEndPoint} has diconnected...");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            if(playerManager.player)
            {
                foreach (Item _i in playerManager.player.inventory)
                {
                    testGameManager.instance.SpawnItem(_i.itemSO.id, playerManager.player.dropTransform.position, playerManager.player.dropTransform.rotation);
                }
            }
            
            UnityEngine.Object.Destroy(playerManager.gameObject);
            playerManager = null;
        });  

        tcp.Disconnect();
        udp.Disconnect();

        ServerSend.PlayerDisconnected(id);
    }
}
*/