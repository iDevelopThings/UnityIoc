using UnityEngine.SceneManagement;

namespace UnityIoc.Runtime
{
    public abstract class Bootstrapper
    {
        public abstract void BindGlobal(ContainerManager container);

        public abstract void BindScene(ContainerManager container, Scene scene, LoadSceneMode mode);

        public abstract void UnBindScene(ContainerManager container, Scene scene);

        public abstract void OnSceneChange(ContainerManager container, Scene previousScene, Scene newScene);

    }
}