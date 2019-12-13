using BeatSaberMarkupLanguage.Attributes;
using BS_Utils.Utilities;
using System;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace PerfectionDisplay
{
    class Settings : PersistentSingleton<Settings>
    {
        private Config config;

        public Vector3 displayPosition = new Vector3(0, 2.3f, 7f);
        public int[] scoreRanges = { 100, 90, 50 };
        public string[] colors = { "#2175ff", "green", "yellow", "orange", "red" };
        [UIValue("showCount")]
        public bool showCount
        {
            get => config.GetBool("In Game Display", "showCount");
            set => config.SetBool("In Game Display", "showCount", value);
        }
        [UIValue("showPercentage")]
        public bool showPercentage
        {
            get => config.GetBool("In Game Display", "showPercentage");
            set => config.SetBool("In Game Display", "showPercentage", value);
        }
        [UIValue("hitscoreIntegration")]
        public bool hitscoreIntegration
        {
            get => config.GetBool("General", "hitscoreIntegration", true);
            set => config.SetBool("General", "hitscoreIntegration", value);
        }
        [UIValue("resultsButton")]
        public bool resultsButton
        {
            get => config.GetBool("General", "resultsButton", false);
            set => config.SetBool("General", "resultsButton", value);
        }

        public void Awake()
        {
            config = new Config("Perfection Display");

            if (config.GetString("General", "Position") == "")
            {
                config.SetString("General", "Position", FormattableString.Invariant($"{displayPosition.x:0.00},{displayPosition.y:0.00},{displayPosition.z:0.00}"));
            }
            else
            {
                var posString = config.GetString("General", "Position");
                var posVals = posString.Split(',').Select(f => float.Parse(f, CultureInfo.InvariantCulture)).ToArray();
                displayPosition = new Vector3(posVals[0], posVals[1], posVals[2]);
            }
            if (config.GetString("General", "Score Ranges") == "")
            {
                config.SetString("General", "Score Ranges", string.Join(",", scoreRanges));
            }
            else
            {
                var rangeString = config.GetString("General", "Score Ranges");
                scoreRanges = rangeString.Split(',').Select(f => int.Parse(f, CultureInfo.InvariantCulture)).ToArray();
            }
            if (config.GetString("General", "Colors") == "")
            {
                config.SetString("General", "Colors", string.Join(",", colors));
            }
            else
            {
                var rangeString = config.GetString("General", "Colors");
                colors = rangeString.Split(',');
            }
            if (scoreRanges.Length + 2 > colors.Length)
            {
                Console.WriteLine("[PerfectionDisplay] Config error - colors should have 2 more colors than there are score ranges, filling the remaining colors with white");
                string[] colors = new string[scoreRanges.Length + 2];
                for (int i = 0; i < colors.Length; i++)
                {
                    if (i < colors.Length) colors[i] = colors[i];
                    else colors[i] = "white";
                }
                this.colors = colors;
            }
            for (int i = 0; i < colors.Length; i++)
            {
                if (!colors[i].StartsWith("#")) colors[i] = "\"" + colors[i] + "\"";
            }
        }
    }
}
