using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public Player player;
    public bool paused = false;
    public Canvas PauseCanvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
            PauseGame();
        }
    }

    public void PauseButton()
    {
        paused = !paused;
        PauseGame();
    }

    public void PauseGame()
    {
        if (paused)
        {
            if(player)
            {
                player.inventoryOpen = false;
                player.HudCanvas.gameObject.SetActive(false);
                player.InventoryCanvas.gameObject.SetActive(false);
            } 
            PauseCanvas.gameObject.SetActive(true);
            
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if (player)
            {
                player.HudCanvas.gameObject.SetActive(true);
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            PauseCanvas.gameObject.SetActive(false);
            
            
        }

    }
}
