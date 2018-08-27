using IllusionPlugin;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PerfectionDisplay
{
    public class Plugin : IPlugin
    {
        public string Name => "Perfection Display";
        public string Version => "1.0";

        private readonly string[] env = { "DefaultEnvironment", "BigMirrorEnvironment", "TriangleEnvironment", "NiceEnvironment" };
        bool _init = false;

        public static Vector3 scoreCounterPosition = new Vector3(0, 2.3f, 7f);

        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void SceneManagerOnActiveSceneChanged(Scene arg0, Scene arg1)
        {
        }

        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        public void OnLevelWasLoaded(int level)
        {

        }

        public void OnLevelWasInitialized(int level)
        {
        }

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        private void OnSceneChanged(Scene _, Scene scene)
        {
            if (!env.Contains(scene.name)) return;

            new GameObject("PerfectDisplay").AddComponent<PerfectDisplay>();
        }
    }
}
