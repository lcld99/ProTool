using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabManager", menuName = "Custom/Prefab Manager")]
public class PrefabManager : ScriptableObject
{
    [System.Serializable]
    public class PrefabData
    {
        public string prefabName;
        public GameObject prefab;
    }

    public List<PrefabData> prefabs = new List<PrefabData>();
}
