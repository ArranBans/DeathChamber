using System;
using System.Collections.Generic;
using System.Text;

namespace DeathChamberNetworking
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromclient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _username = _packet.ReadString();
            Console.WriteLine($"{Server.clients[_fromclient].tcp.socket.Client.RemoteEndPoint} connected successfuly and is now player {_fromclient}.");
            if (_fromclient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromclient}) has assumed the wrong client ID ({_clientIdCheck})!!!");
            }
        }
    }
}
