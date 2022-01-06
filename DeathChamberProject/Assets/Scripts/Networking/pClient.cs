using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using UnityEngine.SceneManagement;
/*
public class pClient : MonoBehaviour
{
    public static int dataBufferSize = 4096;

    public string ip = "";
    public int port = 5225;
    public int myId = 0;
   
    public void Disconnect()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        testGameManager.players = new Dictionary<int, testPlayerManager>();
        testGameManager.itemPickups = new List<ItemPickup>();
        testGameManager.enemies = new List<EnemyTest>();
        Destroy(testGameManager.instance);
        Cursor.visible = true;
        SceneManager.LoadScene("Menu");
        Destroy(Database.instance.gameObject);
    }

    

    public void LoadMap(int _mapId)
    {
        StartCoroutine(LoadAsynchronously(_mapId));
    }

    IEnumerator LoadAsynchronously(int _mapId)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(Database.instance.GetMap(_mapId));

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            //Debug.Log($"Loading Map: {progress}");
            MenuOptions.instance.LoadingBar.value = progress;
            yield return null;
        }

        //          ClientSend.MapLoaded();
    }
}
*/
