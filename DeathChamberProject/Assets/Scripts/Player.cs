using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Inventory")]
    public List<Item> inventory = new List<Item>();
    public int inventorySize = 5;
    public List<Item> hotbar = new List<Item>();
    public readonly int hotbarSize = 3;
    public int selectedItem = 0;
    [Header("UI")]
    public Canvas PauseCanvas;
    public Canvas HudCanvas;
    public Canvas InventoryCanvas;
    public Canvas InteractCanvas;
    public bool paused = false;
    public bool inventoryOpen = false;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool AddItemToInventory(Item _item)
    {
        if(inventory.Count < inventorySize)
        {
            inventory.Add(_item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void PauseGame()
    {
        paused = true;
        inventoryOpen = false;
        PauseCanvas.gameObject.SetActive(true);
        HudCanvas.gameObject.SetActive(false);
        InventoryCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void UnpauseGame()
    {
        paused = false;
        PauseCanvas.gameObject.SetActive(false);
        HudCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void OpenInventory()
    {
        inventoryOpen = true;
        InventoryCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseInventory()
    {
        inventoryOpen = false;
        InventoryCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
