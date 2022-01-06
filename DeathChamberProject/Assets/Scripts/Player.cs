using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public testPlayerController TPController;
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
    public Canvas TakeDamageCanvas;
    public bool inventoryOpen = false;
    [Header("Health")]
    public float maxHealth;
    public float health;
    public Slider healthSlider;
    public float takeDamageSpeed;
    float takeDamageAlpha;
    public float takeDamageVisability;
    [Header("Audio")]
    public AudioSource hitMarker;
    

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


    
    private void Update()
    {
        try
        {
            if (inventory[selectedItem].itemSO.itemType == ItemSO.ItemType.gun )
            {

            }
        }
        catch
        {
            TPController.turnSpeed = Mathf.Lerp(TPController.turnSpeed, OptionsManager.instance.sens, 15 * Time.deltaTime);
            TPController.cam.fieldOfView = Mathf.Lerp(TPController.cam.fieldOfView, OptionsManager.instance.fov, 15 * Time.deltaTime);
        }


        Image takeDamageImage = TakeDamageCanvas.GetComponentInChildren<Image>();
        Color takeDamageColor = takeDamageImage.color;
        takeDamageAlpha = Mathf.Lerp(takeDamageAlpha, 0f, takeDamageSpeed * Time.deltaTime);
        takeDamageColor.a = takeDamageAlpha;
        takeDamageImage.color = new Color(takeDamageImage.color.r, takeDamageImage.color.g, takeDamageImage.color.b, takeDamageColor.a);

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

        //          ClientSend.ChangeSelectedItem(_index);
    }

    public void DropItem(int _index)
    {
        //          ClientSend.DropItem(_index);
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

    public void TakeDamage()
    {
        takeDamageAlpha = takeDamageVisability;
    }

}
