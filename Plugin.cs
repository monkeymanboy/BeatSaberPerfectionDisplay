using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using IPA;
using System.Collections;
using BeatSaberMarkupLanguage.Settings;
using BeatSaberMarkupLanguage;
using System.Reflection;

namespace PerfectionDisplay
{
    public class Plugin : IBeatSaberPlugin
    {
        public static string lastText = "";
        public static string lastPercent = "";
        public static string lastCount = "";

        public static TMP_FontAsset mainFont;
        GameScenesManager gameScenesManager = null;
        
        public void OnApplicationStart()
        {
        }

        public void OnApplicationQuit()
        {
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
            }
            if(scene.name == "MenuViewControllers")
            {
                if (_.name == "EmptyTransition")
                    BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "PerfectionDisplay.results.bsml"), Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault().gameObject, ResultsScreenText.instance);
                BSMLSettings.instance.AddSettingsMenu("Perfection Display", "PerfectionDisplay.settings.bsml", Settings.instance);
                MenuSceneActive();
            }
            if (scene.name == "GameCore")
                GameSceneActive();
        }

        public void GameSceneActive()
        {
            new GameObject("PerfectDisplay").AddComponent<PerfectDisplay>();
        }
        public void MenuSceneActive()
        {
            if (mainFont == null) gameScenesManager.StartCoroutine(FontHunter());
            ResultsScreenText.instance.Names = lastText;
            ResultsScreenText.instance.Counts= lastCount;
            ResultsScreenText.instance.Percents = lastPercent;
        }
        //For some reason can't find the font right when scene loads so this will hunt it down
        public IEnumerator FontHunter()
        {
            while (mainFont == null)
            {
                try
                {
                    TextMeshProUGUI textMesh = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().First(t => t.font?.name == "Teko-Medium SDF No Glow");
                    if(textMesh!=null) mainFont = textMesh.font;
                } catch
                {

                }
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
