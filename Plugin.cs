using System;
using System.Globalization;
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
            /*
            if (text != null) GameObject.Destroy(text.gameObject);
            if (percent != null) GameObject.Destroy(percent.gameObject);
            if (count != null) GameObject.Destroy(count.gameObject);
            ResultsViewController results = Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault();
            int extraOffset = -12;
            text = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), results.transform, false);
            MonoBehaviour.Destroy(text.GetComponentInChildren<LocalizedTextMeshProUGUI>());
            text.fontSize = 5;
            text.color = Color.white;
            text.lineSpacing = -15f;
            text.paragraphSpacing = -15f;
            text.text = lastText;
            text.alignment = TextAlignmentOptions.TopLeft;
            text.rectTransform.localPosition = new Vector3(-20 + extraOffset, 35, 0);
            percent = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), results.transform, false);
            MonoBehaviour.Destroy(text.GetComponentInChildren<LocalizedTextMeshProUGUI>());
            percent.fontSize = 5;
            percent.color = Color.white;
            percent.paragraphSpacing = -15f;
            percent.lineSpacing = -15f;
            percent.text = lastCount;
            percent.alignment = TextAlignmentOptions.TopLeft;
            percent.rectTransform.localPosition = new Vector3(0 + extraOffset, 35, 0);
            count = MonoBehaviour.Instantiate(Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().Last(x => (x.name == "Title")), results.transform, false);
            MonoBehaviour.Destroy(text.GetComponentInChildren<LocalizedTextMeshProUGUI>());
            count.fontSize = 5;
            count.color = Color.white;
            count.lineSpacing = -15f;
            count.paragraphSpacing = -15f;
            count.text = lastPercent;
            count.alignment = TextAlignmentOptions.TopLeft;
            count.rectTransform.localPosition = new Vector3(15 + extraOffset, 35, 0);*/
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
