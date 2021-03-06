using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

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
            Server.clients[_fromclient].SendIntoGame(_username);
        }

        public static void PlayerMovement(int _fromClient, Packet _packet)
        {
            bool[] _inputs = new bool[_packet.ReadInt()];
            for (int i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = _packet.ReadBool();
            }
            Quaternion _rotation = _packet.ReadQuaternion();

            Server.clients[_fromClient].player.SetInput(_inputs, _rotation);
        }

        public static void UDPTest(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Console.WriteLine($"Client {_fromClient} says: {_msg}!!!");
        }
    }
}
