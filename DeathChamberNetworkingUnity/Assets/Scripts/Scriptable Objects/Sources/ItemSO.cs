using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
    public Sprite InventoryImage;
    public string WorldModelName;
    public string ViewModelName;
    public Item item;
    public enum ItemType
    {
        singleUse,
        meleeWeapon,
        gun
    }
    public ItemType itemType;
}
