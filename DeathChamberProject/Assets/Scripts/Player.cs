using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("Inventory")]
    public List<Item> inventory = new List<Item>();
    public int inventorySize = 5;
    public List<Item> hotbar = new List<Item>();
    public readonly int hotbarSize = 3;
    public int selectedItem = 0;
    [Header("UI")]
    public PauseManager pManager;
    public Canvas HudCanvas;
    public Canvas InventoryCanvas;
    public Canvas InteractCanvas;
    public bool inventoryOpen = false;
    [Header("Health")]
    public float maxHealth;
    public float health;
    public Slider healthSlider;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        foreach (Item i in inventory)
        {
            if (inventory.IndexOf(i) == selectedItem)
            {
                i.gameObject.SetActive(true);
            }
            else
            {
                i.gameObject.SetActive(false);
            }
        }
    }

    public void ChangeSelectedItem(int _index)
    {
        selectedItem = _index;

        foreach (Item i in inventory)
        {
            if (inventory.IndexOf(i) == selectedItem)
            {
                i.gameObject.SetActive(true);
            }
            else
            {
                i.gameObject.SetActive(false);
            }
        }

        ClientSend.ChangeSelectedItem(_index);
    }

    public void DropItem(int _index)
    {
        ClientSend.DropItem(_index);
    }

    public bool AddItemToInventory(Item _item)
    {
        if (inventory.Count < inventorySize)
        {
            inventory.Add(_item);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveItemFromInventory(int _index)
    {
        Item _item = inventory[_index];
        Destroy(inventory[_index].gameObject);
        inventory.RemoveAt(_index);
        ChangeSelectedItem(_index);
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

    public void Die()
    {
        pManager.GetComponent<DeployScreen>().deployScreen.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Destroy(gameObject);
    }

    public void Respawn()
    {
        gameObject.SetActive(true);
    }
}
