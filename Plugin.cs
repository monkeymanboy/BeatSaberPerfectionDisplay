using IllusionPlugin;
using IllusionInjector;
using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using HitScoreVisualizer;
using System.Text.RegularExpressions;

namespace PerfectionDisplay
{
    public class Plugin : IPlugin
    {
        public string Name => "Perfection Display";
        public string Version => "1.4.2";

        public static string lastText = "";
        public static string lastPercent = "";
        public static string lastCount = "";

        TextMeshProUGUI text;
        TextMeshProUGUI percent;
        TextMeshProUGUI count;

        bool init = true;
        GameScenesManager gameScenesManager = null;
        
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
            PerfectDisplay.shouldHitscore = ModPrefs.GetBool("PerfectionDisplay", "HitScoreVisualizer Integration", PerfectDisplay.shouldHitscore, true);
        }

        public void OnApplicationQuit()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }
        private void LoadHitScore()
        {
            Config.Judgment[] judgments = Config.instance.judgments;
            PerfectDisplay.scoreRanges = new int[judgments.Length - 1];
            PerfectDisplay.hitScoreNames = new string[judgments.Length];
            PerfectDisplay.colors = new string[judgments.Length + 1];
            for (int i = 0; i < judgments.Length; i++)
            {
                if (i != PerfectDisplay.scoreRanges.Length) PerfectDisplay.scoreRanges[i] = judgments[i].threshold - 1;
                PerfectDisplay.hitScoreNames[i] = Regex.Replace(judgments[i].text, "(%)\\w{1}", "").Trim();
                PerfectDisplay.colors[i] = "#" + ((int)(judgments[i].color[0] * 255)).ToString("X2") + ((int)(judgments[i].color[1] * 255)).ToString("X2") + ((int)(judgments[i].color[2] * 255)).ToString("X2") + ((int)(judgments[i].color[3] * 255)).ToString("X2");
            }
            PerfectDisplay.colors[PerfectDisplay.colors.Length - 1] = "#afa5a3";
        }
        private void OnSceneChanged(Scene _, Scene scene)
        {
            if (gameScenesManager == null)
            {
                gameScenesManager = Resources.FindObjectsOfTypeAll<GameScenesManager>().FirstOrDefault();
                if (gameScenesManager != null)
                {
                    gameScenesManager.transitionDidFinishEvent += OnTransition;
                }
            }
            if (init)
            {
                init = false;
                Console.WriteLine(PluginManager.Plugins);
                if (PerfectDisplay.shouldHitscore && PluginManager.Plugins.Any(x => x.Name == "HitScoreVisualizer")) LoadHitScore();
                else PerfectDisplay.shouldHitscore = false;
            }
            if(scene.name.Equals("Menu"))
            {
                foreach (var rootGameObject in scene.GetRootGameObjects())
                {
                    if (rootGameObject.name.Equals("ViewControllers"))
                    {
                        int extraOffset = 25;
                        Console.WriteLine(rootGameObject.transform.childCount);
                        text = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), rootGameObject.transform.Find("LevelSelection").Find("StandardLevelResultsViewController"), false);
                        text.fontSize = 5;
                        text.color = Color.white;
                        text.paragraphSpacing = -15f;
                        text.text = lastText;
                        text.alignment = TextAlignmentOptions.TopLeft;
                        text.rectTransform.localPosition = new Vector3(-20+extraOffset, 40, 0);
                        percent = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), rootGameObject.transform.Find("LevelSelection").Find("StandardLevelResultsViewController"), false);
                        percent.fontSize = 5;
                        percent.color = Color.white;
                        percent.paragraphSpacing = -15f;
                        percent.text = lastCount;
                        percent.alignment = TextAlignmentOptions.TopLeft;
                        percent.rectTransform.localPosition = new Vector3(0 + extraOffset, 40, 0);
                        count = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), rootGameObject.transform.Find("LevelSelection").Find("StandardLevelResultsViewController"), false);
                        count.fontSize = 5;
                        count.color = Color.white;
                        count.paragraphSpacing = -15f;
                        count.text = lastPercent;
                        count.alignment = TextAlignmentOptions.TopLeft;
                        count.rectTransform.localPosition = new Vector3(15 + extraOffset, 40, 0);
                        return;
                    }
                }
            }
            if (scene.name.Equals("GameCore")) new GameObject("PerfectDisplay").AddComponent<PerfectDisplay>();
        }
        private void OnSceneLoad(Scene _, LoadSceneMode mode)
        {
            Console.WriteLine(_.name);
        }
        public void OnTransition()
        {
            if (text != null) text.text = lastText;
            if (percent!= null) percent.text = lastPercent;
            if (count != null) count.text = lastCount;
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
