using UnityEngine;
using UnityIoc.Runtime.Attributes;

[Prefab(Path = "Assets/_PackageRoot/Examples/PrefabAssetInjection/MyPrefab.prefab")]
public class MyPrefabScript : MonoBehaviour
{
    public void Start() {
        Debug.Log("MyPrefabScript.Start()");
    }
}