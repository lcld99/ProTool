using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabIfBlockConfig", menuName = "PrefabConfigs/PrefabIfBlock")]
public class PrefabIfBlock : ScriptableObject
{
    public string valueToCompare;
    public TypeOfCondition condition;
    // Add other specific fields for Prefab Type 1
}

[CreateAssetMenu(fileName = "NewPrefabConfig", menuName = "PrefabConfigs/PrefabType2")]
public class PrefabType2Config : ScriptableObject
{
    public string stringValue;
    // Add other specific fields for Prefab Type 2
}
