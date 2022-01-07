using RiptideNetworking;
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

        S_ChangeSelectedItem();
    }

    public void DropItem(int _index)
    {
        S_DropItem(_index);
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

    #region Messages
    private void S_ChangeSelectedItem()
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.changeSelectedItem);
        message.AddInt(selectedItem);
        NetworkManager.instance.Client.Send(message);
    }

    private void S_DropItem(int _index)
    {
        Message message = Message.Create(MessageSendMode.reliable, (ushort)ClientToServerId.dropItem);
        message.AddInt(_index);
        NetworkManager.instance.Client.Send(message);
    }

    [MessageHandler((ushort)ServerToClientId.addItemToInventory)]
    private static void R_AddItemToInventory(Message message)
    {
        int _itemId = message.GetInt();
        int _aux1 = message.GetInt();
        int _aux2 = message.GetInt();

        GameObject itemGO = (GameObject)Instantiate(Database.instance.GetItem(_itemId).empty, testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<testPlayerController>().cam.transform);
        itemGO.GetComponent<ItemInfo>().ChangeState(ItemInfo.ItemState.item, _aux1, _aux2);
        testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().AddItemToInventory(itemGO.GetComponent<Item>());
        testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().ChangeSelectedItem(testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().inventory.IndexOf(itemGO.GetComponent<Item>()));

    }

    [MessageHandler((ushort)ServerToClientId.removeItemFromInventory)]
    private static void R_RemoveItemFromInventory(Message message)
    {
        int _index = message.GetInt();
        bool _clear = message.GetBool();

        if (_clear)
        {
            foreach (Item i in testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().inventory)
            {
                Destroy(i);
            }

            testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().inventory = new List<Item>();
            return;
        }
        testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>().RemoveItemFromInventory(_index);
    }

    [MessageHandler((ushort)ServerToClientId.changeSelectedItem)]
    private static void R_ChangeSelectedItem(Message message)
    {
        int _id = message.GetInt();
        int _itemId = message.GetInt();
        int _aux1 = message.GetInt();
        int _aux2 = message.GetInt();

        if (testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem != null)
        {
            Destroy(testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem);
        }

        if (_itemId != 0)
        {
            NetPlayerController netPlayer = testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>();
            netPlayer.selectedItem = (GameObject)Instantiate(Database.instance.GetItem(_itemId).empty, testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().camTransform);
            netPlayer.selectedItemInfo = testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem.GetComponent<ItemInfo>();
            netPlayer.selectedItemInfo.ChangeState(ItemInfo.ItemState.charModel, _aux1, _aux2);
        }
    }

    [MessageHandler((ushort)ServerToClientId.fireWeapon)]
    private static void R_FireWeapon(Message message)
    {
        int _id = message.GetUShort();
        int _weaponId = message.GetInt();

        Instantiate(Resources.Load($"Projectiles/{Database.instance.GetItem(_weaponId).itemName}_Projectile"), testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().camTransform.position, testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().camTransform.rotation);
        if (testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem)
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem.GetComponent<GunInfo>().fireAudio.PlayOneShot(testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem.GetComponent<GunInfo>().fireAudio.clip);
        testPlayerManager.list[(ushort)_id].playerObj.GetComponent<NetPlayerController>().selectedItem.GetComponent<GunInfo>().muzzleFlash.Play();
    }

    [MessageHandler((ushort)ServerToClientId.changeHealth)]
    private static void R_ChangeHealth(Message message)
    {
        float _value = message.GetFloat();

        Player p = testPlayerManager.list[NetworkManager.instance.Client.Id].playerObj.GetComponent<Player>();
        if (p.health > _value)
        {
            p.TakeDamage();
        }
        p.health = _value;
        p.healthSlider.value = _value;
    }

    [MessageHandler((ushort)ServerToClientId.die)]
    private static void R_Die(Message message)
    {
        int _id = message.GetInt();

        if (_id == NetworkManager.instance.Client.Id)
        {
            testPlayerManager.list[(ushort)_id].playerObj.GetComponent<Player>().Die();
        }
        else
        {
            Destroy(testPlayerManager.list[(ushort)_id].playerObj);
        }
    }
    #endregion
}
