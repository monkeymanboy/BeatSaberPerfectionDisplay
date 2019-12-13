using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Text.RegularExpressions;
using BS_Utils.Utilities;

namespace PerfectionDisplay
{
    class PerfectDisplay : MonoBehaviour
    {
        ScoreController scoreController;
        StandardLevelGameplayManager standardLevelGameplayManager;
        MissionLevelGameplayManager missionLevelGameplayManager;
        MonoBehaviour gameplayManager;
        public Vector3 displayPosition;
        public int[] scoreRanges;
        public string[] hitScoreNames;
        public string[] colors;
        int[] scoreCount;
        DisplaySection[] sections;
        int misses = 0;
        int notes = 0;
        public bool showNumbers = false;
        public bool showPercent = false;
        public bool shouldHitscore = true;
        IEnumerator WaitForLoad()
        {
            bool loaded = false;
            while (!loaded)
            {
                scoreController = Resources.FindObjectsOfTypeAll<ScoreController>().FirstOrDefault();
                standardLevelGameplayManager = Resources.FindObjectsOfTypeAll<StandardLevelGameplayManager>().FirstOrDefault();
                missionLevelGameplayManager = Resources.FindObjectsOfTypeAll<MissionLevelGameplayManager>().FirstOrDefault();
                if (scoreController == null || (standardLevelGameplayManager == null && missionLevelGameplayManager == null))
                    yield return new WaitForSeconds(0.1f);
                else
                    loaded = true;
            }

            yield return new WaitForSeconds(0.1f);
            Init();
        }
        private void LoadHitScore()
        {
            HitScoreVisualizer.Config.Judgment[] judgments = HitScoreVisualizer.Config.instance.judgments;
            scoreRanges = new int[judgments.Length - 1];
            hitScoreNames = new string[judgments.Length];
            colors = new string[judgments.Length + 1];
            for (int i = 0; i < judgments.Length; i++)
            {
                if (i != scoreRanges.Length) scoreRanges[i] = judgments[i].threshold - 1;
                hitScoreNames[i] = Regex.Replace(judgments[i].text, "(%)\\w{1}", "").Trim();
                colors[i] = "#" + ((int)(judgments[i].color[0] * 255)).ToString("X2") + ((int)(judgments[i].color[1] * 255)).ToString("X2") + ((int)(judgments[i].color[2] * 255)).ToString("X2") + ((int)(judgments[i].color[3] * 255)).ToString("X2");
            }
            colors[colors.Length - 1] = "#afa5a3";
        }
        void Awake()
        {
            displayPosition = Settings.instance.displayPosition;
            scoreRanges = Settings.instance.scoreRanges;
            colors = Settings.instance.colors;
            shouldHitscore = Settings.instance.hitscoreIntegration;
            if (shouldHitscore && IPA.Loader.PluginManager.Plugins.Any(x => x.Name == "HitScoreVisualizer")) LoadHitScore();
            else shouldHitscore = false;
            showNumbers = Settings.instance.showCount;
            showPercent = Settings.instance.showPercentage;
            scoreCount = new int[scoreRanges.Length + 1];
            StartCoroutine(WaitForLoad());
        }
        private void Init()
        {
            sections = new DisplaySection[colors.Length];
            sections[scoreRanges.Length] = new GameObject().AddComponent<DisplaySection>();
            sections[scoreRanges.Length].perfectDisplay = this;
            sections[scoreRanges.Length].color = colors[scoreRanges.Length];
            sections[scoreRanges.Length].title = "<" + scoreRanges[scoreRanges.Length - 1];
            if (shouldHitscore) sections[scoreRanges.Length].title = hitScoreNames[hitScoreNames.Length - 1];
            sections[scoreRanges.Length + 1] = new GameObject().AddComponent<DisplaySection>();
            sections[scoreRanges.Length + 1].perfectDisplay = this;
            sections[scoreRanges.Length + 1].color = colors[scoreRanges.Length + 1];
            sections[scoreRanges.Length + 1].title = "MISS";
            for (int i = 0; i < scoreRanges.Length; i++)
            {
                sections[i] = new GameObject("PerfectionDisplaySection").AddComponent<DisplaySection>();
                sections[i].perfectDisplay = this;
                sections[i].color = colors[i];
                sections[i].title = ">" + scoreRanges[i];
                if (shouldHitscore) sections[i].title = hitScoreNames[i];
            }
            if (scoreController != null)
            {
                scoreController.noteWasMissedEvent += Miss;
                scoreController.noteWasCutEvent += Cut;
            }
            if (standardLevelGameplayManager != null)
            {
                standardLevelGameplayManager.levelFailedEvent += SongFinish;
                standardLevelGameplayManager.levelFinishedEvent += SongFinish;
            }
            if (missionLevelGameplayManager != null)
            {
                missionLevelGameplayManager.levelFailedEvent += SongFinish;
                missionLevelGameplayManager.levelFinishedEvent += SongFinish;
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
            info.swingRatingCounter.didFinishEvent += e =>
            {
                if (didDone) return;
                didDone = true;
                ScoreController.RawScoreWithoutMultiplier(info, out int before, out int after, out int distScore);
                int total = before + after + distScore;
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
            if (showNumbers || showPercent)
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
        }
        public void SongFinish()
        {
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
