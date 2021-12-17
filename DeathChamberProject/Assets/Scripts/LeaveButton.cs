using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveButton : MonoBehaviour
{
    public void LeaveGame()
    {
        Client.instance.Disconnect();
        /*foreach(testPlayerManager pManager in testGameManager.players.Values)
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
        NetworkUiManager.instance.IpField.interactable = true;*/

        testGameManager.players = new Dictionary<int, testPlayerManager>();
        testGameManager.itemPickups = new List<ItemPickup>();
        Destroy(testGameManager.instance);
        SceneManager.LoadScene("Menu");
        Destroy(Database.instance.gameObject);
        Destroy(Client.instance.gameObject);

    }
}
