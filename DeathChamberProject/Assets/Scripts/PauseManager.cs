using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public Player player;
    public bool paused = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            player.PauseGame();
        }
    }

    public void PauseButton()
    {
        paused = !paused;
        player.PauseGame();
    }
}
