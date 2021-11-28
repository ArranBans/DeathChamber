using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
    public Sprite InventoryImage;
    public GameObject WorldModel;
    public GameObject ViewModel;
    public string itemName;
    public enum ItemType
    {
        singleUse,
        meleeWeapon,
        gun
    }
    public ItemType itemType;
}
