using System.Text.RegularExpressions;
using HitScoreVisualizer.Services;

namespace PerfectionDisplay.Services
{
	internal class HSVConfigProvider
	{
		private readonly ConfigProvider _configProvider;

		public HSVConfigProvider(ConfigProvider configProvider)
		{
			_configProvider = configProvider;
		}

		public bool Enrich(ref int[] scoreRanges, ref string[] hitScoreNames, ref string[] colors)
		{
			var judgments = _configProvider.GetCurrentConfig()?.Judgments;
			if (judgments == null)
			{
				return false;
			}

			scoreRanges = new int[judgments.Count - 1];
			hitScoreNames = new string[judgments.Count];
			colors = new string[judgments.Count + 1];
			for (var i = 0; i < judgments.Count; i++)
			{
				if (i != scoreRanges.Length)
				{
					scoreRanges[i] = judgments[i].Threshold - 1;
				}

				hitScoreNames[i] = Regex.Replace(judgments[i].Text, "(%)\\w{1}", "").Trim();
				colors[i] = "#" +
				            ((int) (judgments[i].Color[0] * 255)).ToString("X2") +
				            ((int) (judgments[i].Color[1] * 255)).ToString("X2") +
				            ((int) (judgments[i].Color[2] * 255)).ToString("X2") +
				            ((int) (judgments[i].Color[3] * 255)).ToString("X2");
			}

			colors[colors.Length - 1] = "#afa5a3";

			return true;
		}
	}
}