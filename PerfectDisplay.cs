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

        int[] scoreRanges = { 100, 90, 50 };
        string[] colors = { "blue", "green", "yellow", "orange", "red" };
        int[] scoreCount;
        int misses;
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
            scoreMesh.rectTransform.position = Plugin.scoreCounterPosition;
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
            UpdateText();
        }
        public void Cut(NoteData data, NoteCutInfo info, int combo)
        {
            bool didDone = false;
            info.afterCutSwingRatingCounter.didFinishEvent += c => {
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
            for(int i = 0; i < scoreRanges.Length; i++)
            {
                text += "<color=\""+colors[i] +"\">"+">"+scoreRanges[i] + "-" + scoreCount[i] + "<color=\"black\">|";
            }
            text += "<color=\"" + colors[scoreRanges.Length] + "\">" + "<" + scoreRanges[scoreRanges.Length-1] + "-" + scoreCount[scoreRanges.Length] + "<color=\"black\">|";
            text += "<color=\"" + colors[scoreRanges.Length + 1] + "\">" + "MISS-" +misses;
            scoreMesh.text = text;
        }
    }
}
