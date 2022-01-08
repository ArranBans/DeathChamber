using RiptideNetworking;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUiManager : MonoBehaviour
{
    public static NetworkUiManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public InputField IpField;

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

    public void ConnectToServer()
    {
        NetworkManager.instance.Connect();
    }

    public void ConnectedToServer()
    {
        Debug.Log("Did Connect To Server");
        usernameField.interactable = false;
        IpField.interactable = false;
        OptionsManager.instance.fov = MenuOptions.instance.fov;
        OptionsManager.instance.sens = MenuOptions.instance.sens;
    }

    public void BackToMain()
    {

    }

    #region Messages
    public void S_SendName()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.name);
        message.AddString(usernameField.text);
        NetworkManager.instance.Client.Send(message);
    }
    #endregion
}
