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

        /// <inheritdoc />
        public override void BindGlobal(ContainerManager container) { }

        /// <inheritdoc />
        public override void BindScene(ContainerManager container, Scene scene, LoadSceneMode mode) { }

        /// <inheritdoc />
        public override void UnBindScene(ContainerManager container, Scene scene) { }

        /// <inheritdoc />
        public override void OnSceneChange(ContainerManager container, Scene previousScene, Scene newScene) { }
    }
}