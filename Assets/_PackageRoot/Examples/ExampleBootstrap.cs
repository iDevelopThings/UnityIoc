using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityIoc.Runtime;

namespace _PackageRoot.Examples
{

    public class ExampleBootstrap : Bootstrapper
    {
        // [RuntimeInitializeOnLoadMethod]
        // public static void Load() {
        //     var assetRef = IocConfig.Instance.assetReferences.FirstOrDefault();
        //     var asset    = assetRef.LoadAsset();
        //     Debug.Log("ExampleBootstrap.Load()");
        // }

        public override void BindGlobal(ContainerManager container) {
            Debug.Log($"ExampleBootstrap: BindGlobal");
        }

        public override void BindScene(ContainerManager container, Scene scene, LoadSceneMode mode) {
            Debug.Log($"ExampleBootstrap: BindScene");
        }

        public override void UnBindScene(ContainerManager container, Scene scene) {
            Debug.Log($"ExampleBootstrap: UnBindScene");
        }

        public override void OnSceneChange(ContainerManager container, Scene previousScene, Scene newScene) {
            Debug.Log($"ExampleBootstrap: OnSceneChange");
        }
    }
}