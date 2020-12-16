using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using SiraUtil.Converters;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PerfectionDisplay.Settings
{
	public class Configuration
	{
		internal static Configuration? Instance { get; set; }

		// General settings
		[NonNullable]
		[UseConverter(typeof(Vector3Converter))]
		public virtual Vector3 Position { get; set; } = new Vector3(0f, 2.3f, 7f);

		[NonNullable]
		[UseConverter(typeof(ListConverter<int>))]
		public virtual List<int> ScoreRanges { get; set; } = new List<int> {100, 90, 50};

		[NonNullable]
		[UseConverter(typeof(ListConverter<string>))]
		public virtual List<string> Colors { get; set; } = new List<string>
		{
			"#2175ff",
			"green",
			"yellow",
			"orange",
			"red"
		};

		public virtual bool HSVIntegration { get; set; } = true;
		public virtual bool ResultsButton { get; set; }

		// In-game settings
		public virtual bool ShowCount { get; set; }
		public virtual bool ShowPercentage { get; set; }

		/*public void Awake()
		{

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
				var colors = new string[scoreRanges.Length + 2];
				for (var i = 0; i < colors.Length; i++)
				{
					if (i < colors.Length)
					{
						colors[i] = colors[i];
					}
					else
					{
						colors[i] = "white";
					}
				}

				this.colors = colors;
			}

			for (var i = 0; i < colors.Length; i++)
			{
				if (!colors[i].StartsWith("#"))
				{
					colors[i] = "\"" + colors[i] + "\"";
				}
			}
		}*/
	}
}