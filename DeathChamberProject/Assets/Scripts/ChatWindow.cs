using RiptideNetworking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatWindow : MonoBehaviour
{
    public Canvas chatWindow;
    public InputField chatInput;
    public bool chatWindowOpen;

    void Start()
    {
        chatWindow.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if(chatWindowOpen)
        {

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if(chatInput.text.StartsWith("/")) // Command
                {
                    string command = chatInput.text.Remove(0, 1);
                    

                    if(command.StartsWith("give")) // Give
                    {
                        command = command.Remove(0, 5);
                        int _index;
                        int.TryParse(command, out _index);
                        Debug.Log($"Give [{command}]");
                        S_Command(0, _index);
                    }

                    if (command.StartsWith("spawn")) // Spawn
                    {
                        command = command.Remove(0, 6);
                        int _index;
                        int.TryParse(command, out _index);
                        Debug.Log($"Spawn [{command}]");
                        S_Command(1, _index);
                    }

                    if (command.StartsWith("teleport")) // Teleport
                    {
                        command = command.Remove(0, 9);
                        int _index;
                        int.TryParse(command, out _index);
                        Debug.Log($"Teleport to player [{command}]");
                        S_Command(2, _index);
                    }

                    if (command.StartsWith("summon")) // Teleport
                    {
                        command = command.Remove(0, 7);
                        int _index;
                        int.TryParse(command, out _index);
                        Debug.Log($"Summon AI [{command}]");
                        S_Command(3, _index);
                    }
                }
                else // Chat
                {
                    Debug.Log(chatInput.text);
                }
                chatInput.text = "";
                ChatWindowOpen(false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ChatWindowOpen(true);
            }
        }
        
        
    }

    void ChatWindowOpen(bool open)
    {
        if(open)
        {
            chatWindow.gameObject.SetActive(true);
            chatInput.ActivateInputField();
            chatInput.Select();
            chatWindowOpen = true;
        }
        else
        {
            chatWindow.gameObject.SetActive(false);
            chatWindowOpen = false;
        }
    }

    #region Messages
    private static void S_Command(int _type, int _aux)
    {
        Message message = Message.Create(MessageSendMode.unreliable, (ushort)ClientToServerId.command);
        message.AddInt(_type);
        message.AddInt(_aux);
        
        NetworkManager.instance.Client.Send(message);
    }
    #endregion
}
