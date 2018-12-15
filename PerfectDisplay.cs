using System;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace PerfectionDisplay
{
    class PerfectDisplay : MonoBehaviour
    {
        ScoreController scoreController;
        public static Vector3 displayPosition = new Vector3(0, 2.3f, 7f);
        public static int[] scoreRanges = { 100, 90, 50 };
        public static string[] hitScoreNames;
        public static bool shouldHitscore = true;
        public static string[] colors = { "#2175ff", "green", "yellow", "orange", "red" };
        int[] scoreCount;
        DisplaySection[] sections;
        int misses = 0;
        int notes = 0;
        public static bool showNumbers = false;
        public static bool showPercent = false;
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
            scoreCount = new int[scoreRanges.Length + 1];
            StartCoroutine(WaitForLoad());
        }
        private void Init()
        {
            sections = new DisplaySection[colors.Length];
            sections[scoreRanges.Length] = new GameObject().AddComponent<DisplaySection>();
            sections[scoreRanges.Length].color = colors[scoreRanges.Length];
            sections[scoreRanges.Length].title = "<" + scoreRanges[scoreRanges.Length - 1];
            if (shouldHitscore) sections[scoreRanges.Length].title = hitScoreNames[hitScoreNames.Length - 1];
            sections[scoreRanges.Length + 1] = new GameObject().AddComponent<DisplaySection>();
            sections[scoreRanges.Length + 1].color = colors[scoreRanges.Length + 1];
            sections[scoreRanges.Length + 1].title = "MISS";
            for (int i = 0; i < scoreRanges.Length; i++)
            {
                sections[i] = new GameObject("PerfectionDisplaySection").AddComponent<DisplaySection>();
                sections[i].color = colors[i];
                sections[i].title = ">" + scoreRanges[i];
                if (shouldHitscore) sections[i].title = hitScoreNames[i];
            }
            if (scoreController != null)
            {
                scoreController.noteWasMissedEvent += Miss;
                scoreController.noteWasCutEvent += Cut;
            }
            UpdateText();
        }
        public void Miss(NoteData data, int c)
        {
            if (data.noteType == NoteType.Bomb) return;
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
            info.afterCutSwingRatingCounter.didFinishEvent += e =>
            {
                if (didDone) return;
                didDone = true;
                ScoreController.ScoreWithoutMultiplier(info, info.afterCutSwingRatingCounter, out int before, out int after, out int distScore);
                int total = before + after;
                for (int i = 0; i < scoreRanges.Length; i++)
                {
                    if (scoreRanges[i] < total)
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
            if (PerfectDisplay.showNumbers || PerfectDisplay.showPercent)
            {
                float width = 0;
                for (int i = 0; i < scoreCount.Length; i++)
                {
                    sections[i].UpdateText(scoreCount[i], GetPercent(scoreCount[i]));
                    width += sections[i].GetWidth();
                }
                sections[scoreRanges.Length + 1].UpdateText(misses, GetPercent(misses));
                width += sections[scoreRanges.Length + 1].GetWidth();

                float curX = sections[0].GetWidth() / 2; ;
                for (int i = 0; i < scoreCount.Length; i++)
                {
                    sections[i].UpdatePosition(-(width / 2) + curX);
                    curX += sections[i].GetWidth() / 2;
                    curX += sections[i + 1].GetWidth() / 2;
                }
                sections[scoreRanges.Length + 1].UpdatePosition(-(width / 2) + curX);
            }


            Plugin.lastText = "Range\n";
            for (int i = 0; i < scoreRanges.Length; i++)
            {
                Plugin.lastText += "<color=" + colors[i] + ">" + (shouldHitscore ? hitScoreNames[i] : (">" + scoreRanges[i])) + "\n";
            }
            Plugin.lastText += "<color=" + colors[scoreRanges.Length] + ">" + (shouldHitscore ? hitScoreNames[scoreRanges.Length] : ("<" + scoreRanges[scoreRanges.Length - 1])) + "\n";
            Plugin.lastText += "<color=" + colors[scoreRanges.Length + 1] + ">" + "MISS";
            Plugin.lastCount = "Count\n";
            for (int i = 0; i < scoreRanges.Length; i++)
            {
                Plugin.lastCount += "<color=" + colors[i] + ">" + scoreCount[i] + "\n";
            }
            Plugin.lastCount += "<color=" + colors[scoreRanges.Length] + ">" + scoreCount[scoreRanges.Length] + "\n";
            Plugin.lastCount += "<color=" + colors[scoreRanges.Length + 1] + ">" + misses;
            Plugin.lastPercent = "Percent\n";
            for (int i = 0; i < scoreRanges.Length; i++)
            {
                Plugin.lastPercent += "<color=" + colors[i] + ">" + GetPercent(scoreCount[i]) + "%\n";
            }
            Plugin.lastPercent += "<color=" + colors[scoreRanges.Length] + ">" + GetPercent(scoreCount[scoreRanges.Length]) + "%\n";
            Plugin.lastPercent += "<color=" + colors[scoreRanges.Length + 1] + ">" + GetPercent(misses) + "%";
        }
        private String GetPercent(int hits)
        {
            if (hits == 0) return "0";
            return ((hits * 1f / notes) * 100).ToString("0.0");
        }
    }
}
