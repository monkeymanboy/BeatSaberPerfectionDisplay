using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPA;
using System.Collections;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage;
using System.Reflection;
using BS_Utils.Utilities;
using System;

namespace PerfectionDisplay
{
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public static string lastText = "";
        public static string lastPercent = "";
        public static string lastCount = "";

        public static TMP_FontAsset mainFont;
        
        [OnStart]
        public void OnApplicationStart()
        {
            BSMLSettings.instance.AddSettingsMenu("Perfection Display", "PerfectionDisplay.settings.bsml", Settings.instance);
            BSEvents.gameSceneActive += GameSceneActive;
            BSEvents.menuSceneActive += MenuSceneActive;
            BSEvents.menuSceneLoadedFresh += MenuSceneFresh;
        }

        private void MenuSceneFresh()
        {
            BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "PerfectionDisplay.results.bsml"), Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault().gameObject, ResultsScreenText.instance);
            mainFont = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().First(t => t.font?.name == "Teko-Medium SDF No Glow")?.font;
        }

        public void GameSceneActive()
        {
            new GameObject("PerfectDisplay").AddComponent<PerfectDisplay>();
        }
        public void MenuSceneActive()
        {
            ResultsScreenText.instance.Names = lastText;
            ResultsScreenText.instance.Counts = lastCount;
            ResultsScreenText.instance.Percents = lastPercent;
        }
    }
}
