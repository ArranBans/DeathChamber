using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.tcp.SendData(_packet);
    }

    private static void SendUDPData(Packet _packet)
    {
        _packet.WriteLength();
        Client.instance.udp.SendData(_packet);
    }

    #region packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.instance.myId);
            _packet.Write(NetworkUiManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void PlayerMovement(bool[] _inputs, int _tick)
    {
        using (Packet _packet = new Packet((int)ClientPackets.playeMovement))
        {
            _packet.Write(_inputs.Length);
            foreach(bool _input in _inputs)
            {
                _packet.Write(_input);
            }

            _packet.Write(testGameManager.players[Client.instance.myId].transform.rotation);

            _packet.Write(_tick);

            SendUDPData(_packet);
        }
    }

    public static void UDPTest(string _message)
    {
        using (Packet _packet = new Packet((int)ClientPackets.UDPTest))
        {
            _packet.Write(_message);

            SendUDPData(_packet);
            Debug.Log("UDPTest Send");
        }
    }
    #endregion
}
