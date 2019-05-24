using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using HitScoreVisualizer;
using System.Text.RegularExpressions;
using IPA;
using IPA.Config;
using CustomUI.Utilities;
using System.Collections;

namespace PerfectionDisplay
{
    public class Plugin : IBeatSaberPlugin
    {
        public static string lastText = "";
        public static string lastPercent = "";
        public static string lastCount = "";

        TextMeshProUGUI text;
        TextMeshProUGUI percent;
        TextMeshProUGUI count;
        public static TMP_FontAsset mainFont;

        bool init = true;
        GameScenesManager gameScenesManager = null;
        
        public void OnApplicationStart()
        {
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

            BSEvents.menuSceneLoadedFresh += MenuSceneActive;
            BSEvents.menuSceneActive += MenuSceneActive;
            BSEvents.gameSceneActive += GameSceneActive;
        }

        public void OnApplicationQuit()
        {
            BSEvents.menuSceneActive -= MenuSceneActive;
            BSEvents.gameSceneActive -= GameSceneActive;
        }
        private void LoadHitScore()
        {
            HitScoreVisualizer.Config.Judgment[] judgments = HitScoreVisualizer.Config.instance.judgments;
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
        }
        private void OnSceneLoad(Scene _, LoadSceneMode mode)
        {
        }
        public void OnTransition()
        {
            if (text != null) text.text = lastText;
            if (percent!= null) percent.text = lastPercent;
            if (count != null) count.text = lastCount;
        }
        

        public void OnUpdate()
        {
        }

        public void OnFixedUpdate()
        {
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode sceneMode)
        {

        }

        public void OnSceneUnloaded(Scene scene)
        {

        }

        public void OnActiveSceneChanged(Scene _, Scene scene)
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
                if (PerfectDisplay.shouldHitscore && IPA.Loader.PluginManager.Plugins.Any(x => x.Name == "HitScoreVisualizer")) LoadHitScore();
                else PerfectDisplay.shouldHitscore = false;
            }
        }

        public void GameSceneActive()
        {
            new GameObject("PerfectDisplay").AddComponent<PerfectDisplay>();
        }
        public void MenuSceneActive()
        {
            if (mainFont == null) gameScenesManager.StartCoroutine(FontHunter());
            if (text != null) MonoBehaviour.Destroy(text);
            if (percent != null) MonoBehaviour.Destroy(percent);
            if (count != null) MonoBehaviour.Destroy(count);
            ResultsViewController results = Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();
            int extraOffset = -12;
            text = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), results.transform, false);
            text.fontSize = 5;
            text.color = Color.white;
            text.lineSpacing = -15f;
            text.paragraphSpacing = -15f;
            text.text = lastText;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.rectTransform.localPosition = new Vector3(-20 + extraOffset, 35, 0);
            percent = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), results.transform, false);
            percent.fontSize = 5;
            percent.color = Color.white;
            percent.paragraphSpacing = -15f;
            percent.lineSpacing = -15f;
            percent.text = lastCount;
            percent.alignment = TextAlignmentOptions.TopLeft;
            percent.rectTransform.localPosition = new Vector3(0 + extraOffset, 35, 0);
            count = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), results.transform, false);
            count.fontSize = 5;
            count.color = Color.white;
            count.lineSpacing = -15f;
            count.paragraphSpacing = -15f;
            count.text = lastPercent;
            count.alignment = TextAlignmentOptions.TopLeft;
            count.rectTransform.localPosition = new Vector3(15 + extraOffset, 35, 0);
        }
        //For some reason can't find the font right when scene loads so this will hunt it down
        public IEnumerator FontHunter()
        {
            while (mainFont == null)
            {
                try
                {
                    mainFont = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().First(t => t.font?.name == "Teko-Medium SDF No Glow").font;
                } catch
                {

                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
