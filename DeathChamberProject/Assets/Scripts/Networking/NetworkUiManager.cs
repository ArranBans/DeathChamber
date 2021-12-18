using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetworkUiManager : MonoBehaviour
{
    public static NetworkUiManager instance;

    public GameObject startMenu;
    public InputField usernameField;
    public InputField IpField;
    public OptionsManager options;

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
        Client.instance.ConnectToServer();
    }

    public void ConnectedToServer()
    {
        usernameField.interactable = false;
        IpField.interactable = false;
        options.fov = GetComponent<MenuOptions>().fov;
        options.sens = GetComponent<MenuOptions>().sens;
    }
}
