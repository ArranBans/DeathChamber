using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Database", menuName = "ScriptableObjects/EnemyDatabase")]
public class EnemyDatabase : ScriptableObject
{
    public List<EnemySO> database = new List<EnemySO>();
}

