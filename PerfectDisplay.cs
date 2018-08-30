using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using System.Collections;

namespace PerfectionDisplay
{
    class PerfectDisplay : MonoBehaviour
    {
        TextMeshPro scoreMesh;
        ScoreController scoreController;
        public static Vector3 displayPosition = new Vector3(0, 2.3f, 7f);
        public static int[] scoreRanges = { 100, 90, 50 };
        public static string[] colors = { "#2175ff", "green", "yellow", "orange", "red" };
        int[] scoreCount;
        int misses = 0;
        int notes = 0;
        public static bool showNumbers = true;
        public static bool showPercent = true;
        IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                if (scoreController == null)
                    yield return new WaitForSeconds(0.1f);
                else
                    loaded = true;
            }

            Init();
        }
        void Awake()
        {
            scoreCount = new int[scoreRanges.Length+1];
            StartCoroutine(WaitForLoad());
        }
        private void Init()
        {
            scoreMesh = this.gameObject.AddComponent<TextMeshPro>();
            scoreMesh.text = "";
            scoreMesh.fontSize = 3;
            scoreMesh.color = Color.white;
            scoreMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
            scoreMesh.alignment = TextAlignmentOptions.Center;
            scoreMesh.rectTransform.position = displayPosition;
            if (scoreController != null)
            {
                scoreController.noteWasMissedEvent += Miss;
                scoreController.noteWasCutEvent += Cut;
            }
            UpdateText();
        }
        public void Miss(NoteData data, int c)
        {
            misses++;
            notes++;
            UpdateText();
        }
        public void Cut(NoteData data, NoteCutInfo info, int combo)
        {
            notes++;
            if (!info.allIsOK)
            {
                misses++;
                UpdateText();
                return;
            }
            bool didDone = false;
            info.afterCutSwingRatingCounter.didFinishEvent += e => {
                if (didDone) return;
                didDone = true;
                ScoreController.ScoreWithoutMultiplier(info, info.afterCutSwingRatingCounter, out int before, out int after);
                int total = before + after;
                for (int i = 0; i < scoreRanges.Length; i++)
                {
                    if (scoreRanges[i] <total)
                    {
                        scoreCount[i]++;
                        UpdateText();
                        return;
                    }
                }
                scoreCount[scoreRanges.Length]++;
                UpdateText();
            };
        }

        public void UpdateText()
        {
            String text = "";
            if (showNumbers)
            {
                for (int i = 0; i < scoreRanges.Length; i++)
                {
                    text += "<color=" + colors[i] + ">" + ">" + scoreRanges[i] + "-" + scoreCount[i] + "<color=\"black\">|";
                }
                text += "<color=" + colors[scoreRanges.Length] + ">" + "<" + scoreRanges[scoreRanges.Length - 1] + "-" + scoreCount[scoreRanges.Length] + "<color=\"black\">|";
                text += "<color=" + colors[scoreRanges.Length + 1] + ">" + "MISS-" + misses +"\n";
            }
            if (showPercent)
            {
                for (int i = 0; i < scoreRanges.Length; i++)
                {
                    text += "<color=" + colors[i] + ">" + ">" + scoreRanges[i] + "-" + GetPercent(scoreCount[i]) + "%<color=\"black\">|";
                }
                text += "<color=" + colors[scoreRanges.Length] + ">" + "<" + scoreRanges[scoreRanges.Length - 1] + "-" + GetPercent(scoreCount[scoreRanges.Length]) + "%<color=\"black\">|";
                text += "<color=" + colors[scoreRanges.Length + 1] + ">" + "MISS-" + GetPercent(misses) + "%";
            }
            Plugin.lastText = "Range\n";
            for (int i = 0; i < scoreRanges.Length; i++)
            {
                Plugin.lastText += "<color=" + colors[i] + ">" + ">" + scoreRanges[i] + "\n";
            }
            Plugin.lastText += "<color=" + colors[scoreRanges.Length] + ">" + "<" + scoreRanges[scoreRanges.Length - 1] + "\n";
            Plugin.lastText += "<color=" + colors[scoreRanges.Length + 1] + ">" + "MISS";
            Plugin.lastCount = "Count\n";
            for (int i = 0; i < scoreRanges.Length; i++)
            {
                Plugin.lastCount += "<color=" + colors[i] + ">" + scoreCount[i] + "\n";
            }
            Plugin.lastCount += "<color=" + colors[scoreRanges.Length] + ">" + scoreCount[scoreRanges.Length - 1] + "\n";
            Plugin.lastCount += "<color=" + colors[scoreRanges.Length + 1] + ">" + misses;
            Plugin.lastPercent = "Percent\n";
            for (int i = 0; i < scoreRanges.Length; i++)
            {
                Plugin.lastPercent += "<color=" + colors[i] + ">" + GetPercent(scoreCount[i]) + "%\n";
            }
            Plugin.lastPercent += "<color=" + colors[scoreRanges.Length] + ">" + GetPercent(scoreCount[scoreRanges.Length - 1]) + "%\n";
            Plugin.lastPercent += "<color=" + colors[scoreRanges.Length + 1] + ">" + GetPercent(misses);
            scoreMesh.text = text;
        }
        private String GetPercent(int hits)
        {
            if (hits == 0) return "0";
            return ((hits * 1f / notes) * 100).ToString("0.0");
        }
    }
}
