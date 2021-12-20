using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "ScriptableObjects/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Item Scriptable Object")]
    public Sprite InventoryImage;
    public GameObject empty;
    public string itemName;
    public int itemId;
    public float mass;
    public Vector3 colliderCentre;
    public Vector3 colliderSize;
    public enum ItemType
    {
        consumable,
        meleeWeapon,
        gun
    }
    public ItemType itemType;
    [Header("3rd Person Model")]
    public Vector3 charPos;
}
