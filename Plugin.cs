using IllusionPlugin;
using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PerfectionDisplay
{
    public class Plugin : IPlugin
    {
        public string Name => "Perfection Display";
        public string Version => "1.2.0";

        public static string lastText = "";
        public static string lastPercent = "";
        public static string lastCount = "";

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
            if (PerfectDisplay.scoreRanges.Length + 2 > PerfectDisplay.colors.Length)
            {
                Console.WriteLine("[PerfectionDisplay] Config error - colors should have 2 more colors than there are score ranges, filling the remaining colors with white");
                string[] colors = new string[PerfectDisplay.scoreRanges.Length + 2];
                for (int i = 0; i < colors.Length; i++)
                {
                    if (i < PerfectDisplay.colors.Length) colors[i] = PerfectDisplay.colors[i];
                    else colors[i] = "white";
                }
                PerfectDisplay.colors = colors;
            }
            for(int i = 0; i < PerfectDisplay.colors.Length; i++)
            {
                if (!PerfectDisplay.colors[i].StartsWith("#")) PerfectDisplay.colors[i] = "\""+PerfectDisplay.colors[i]+ "\"";
            }
            PerfectDisplay.showNumbers = ModPrefs.GetBool("PerfectionDisplay", "Show Count", PerfectDisplay.showNumbers, true);
            PerfectDisplay.showPercent = ModPrefs.GetBool("PerfectionDisplay", "Show Percent", PerfectDisplay.showPercent, true);
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnSceneChanged(Scene _, Scene scene)
        {
            if(scene.name.Equals("Menu"))
            {
                foreach (var rootGameObject in scene.GetRootGameObjects())
                {
                    if (rootGameObject.name.Equals("ViewControllers"))
                    {
                        TextMeshProUGUI text = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), rootGameObject.transform.Find("Results").Find("Cleared"), false);
                        text.fontSize = 4;
                        text.color = Color.white;
                        text.text = lastText;
                        text.alignment = TextAlignmentOptions.Left;
                        text.rectTransform.localPosition = new Vector3(-20, 28, 0);
                        text = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), rootGameObject.transform.Find("Results").Find("Cleared"), false);
                        text.fontSize = 4;
                        text.color = Color.white;
                        text.text = lastCount;
                        text.alignment = TextAlignmentOptions.Left;
                        text.rectTransform.localPosition = new Vector3(3, 28, 0);
                        text = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), rootGameObject.transform.Find("Results").Find("Cleared"), false);
                        text.fontSize = 4;
                        text.color = Color.white;
                        text.text = lastPercent;
                        text.alignment = TextAlignmentOptions.Left;
                        text.rectTransform.localPosition = new Vector3(-10, 28, 0);
                        return;
                    }
                }
            }
            if (!env.Contains(scene.name)) return;

            new GameObject("PerfectDisplay").AddComponent<PerfectDisplay>();
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
    }
}
