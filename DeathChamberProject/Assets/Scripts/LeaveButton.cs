using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveButton : MonoBehaviour
{
    public void LeaveGame()
    {
        Client.instance.Disconnect();
        foreach(testPlayerManager pManager in testGameManager.players.Values)
        {
            Destroy(pManager.gameObject);
        }
        testGameManager.players = new Dictionary<int, testPlayerManager>();
        foreach(ItemPickup i in testGameManager.itemPickups)
        {
            if(i != null)
            {
                Destroy(i.gameObject);
            }
            
        }
        testGameManager.itemPickups = new List<ItemPickup>();
        NetworkUiManager.instance.startMenu.SetActive(true);
        NetworkUiManager.instance.usernameField.interactable = true;
        NetworkUiManager.instance.IpField.interactable = true;
    }
}
