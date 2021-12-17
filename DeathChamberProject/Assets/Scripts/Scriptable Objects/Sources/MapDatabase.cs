using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Database", menuName = "ScriptableObjects/MapDatabase")]
public class MapDatabase : ScriptableObject
{
    public List<string> database = new List<string>();
}
