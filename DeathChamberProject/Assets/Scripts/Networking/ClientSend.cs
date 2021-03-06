using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public class //          ClientSend : MonoBehaviour
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
            _packet.Write(NetworkManager.instance.Client.Id);
            _packet.Write(NetworkUiManager.instance.usernameField.text);

            SendTCPData(_packet);
        }
    }

    public static void MapLoaded()
    {
        using (Packet _packet = new Packet((int)ClientPackets.mapLoaded))
        { 
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

            _packet.Write(testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.transform.rotation);
            _packet.Write(testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<testPlayerController>().cam.transform.rotation);

            _packet.Write(_tick);

            SendUDPData(_packet);
        }
    }

    public static void Interact()
    {
        using (Packet _packet = new Packet((int)ClientPackets.interact))
        {
            SendTCPData(_packet);
        }
    }

    public static void ChangeSelectedItem(int _index)
    {
        using (Packet _packet = new Packet((int)ClientPackets.changeSelectedItem))
        {
            _packet.Write(_index);

            SendTCPData(_packet);
        }
    }

    public static void DropItem(int _index)
    {
        using (Packet _packet = new Packet((int)ClientPackets.dropItem))
        {
            _packet.Write(_index);
            //Debug.Log($"Removing item: {_index}");
            SendTCPData(_packet);
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

    public static void FireWeapon(bool _aiming, float _gunXRot)
    {
        using (Packet _packet = new Packet((int)ClientPackets.fireWeapon))
        {
            _packet.Write(_aiming);
            _packet.Write(_gunXRot);
            SendUDPData(_packet);
        }
    }

    public static void Deploy()
    {
        using (Packet _packet = new Packet((int)ClientPackets.deploy))
        {
            SendTCPData(_packet);
        }
    }

    public static void Consumable()
    {
        using (Packet _packet = new Packet((int)ClientPackets.consumable))
        {
            SendTCPData(_packet);
        }
    }

    public static void Command(int _commandType, int _index)
    {
        using (Packet _packet = new Packet((int)ClientPackets.command))
        {
            _packet.Write(_commandType);
            _packet.Write(_index);
            SendTCPData(_packet);
        }
    }

    #endregion
}
*/