using IllusionPlugin;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PerfectionDisplay
{
    public class Plugin : IPlugin
    {
        public string Name => "Perfection Display";
        public string Version => "1.1";

        private readonly string[] env = { "DefaultEnvironment", "BigMirrorEnvironment", "TriangleEnvironment", "NiceEnvironment" };
        
        public void OnApplicationStart()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;

            if (ModPrefs.GetString("PerfectionDisplay", "Position") == "")
            {
                ModPrefs.SetString("PerfectionDisplay", "Position",FormattableString.Invariant($"{PerfectDisplay.displayPosition.x:0.00},{PerfectDisplay.displayPosition.y:0.00},{PerfectDisplay.displayPosition.z:0.00}"));
            }
            else
            {
                var posString = ModPrefs.GetString("PerfectionDisplay", "Position");
                var posVals = posString.Split(',').Select(f => float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
                PerfectDisplay.displayPosition = new Vector3(posVals[0], posVals[1], posVals[2]);
            }
            if (ModPrefs.GetString("PerfectionDisplay", "Score Ranges") == "")
            {
                ModPrefs.SetString("PerfectionDisplay", "Score Ranges", string.Join(",", PerfectDisplay.scoreRanges));
            }
            else
            {
                var rangeString = ModPrefs.GetString("PerfectionDisplay", "Score Ranges");
                PerfectDisplay.scoreRanges = rangeString.Split(',').Select(f => int.Parse(f, CultureInfo.InvariantCulture)).ToArray();
            }
            if (ModPrefs.GetString("PerfectionDisplay", "Colors") == "")
            {
                ModPrefs.SetString("PerfectionDisplay", "Colors", string.Join(",", PerfectDisplay.colors));
            }
            else
            {
                var rangeString = ModPrefs.GetString("PerfectionDisplay", "Colors");
                PerfectDisplay.colors = rangeString.Split(',');
            }
            if (PerfectDisplay.scoreRanges.Length+2 != PerfectDisplay.colors.Length) Console.WriteLine("[PerfectionDisplay] Config error - colors should have 2 more colors than there are score ranges");
            PerfectDisplay.showNumbers = ModPrefs.GetBool("PerfectionDisplay", "Show Count", PerfectDisplay.showNumbers, true);
            PerfectDisplay.showPercent = ModPrefs.GetBool("PerfectionDisplay", "Show Percent", PerfectDisplay.showPercent, true);
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
