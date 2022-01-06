﻿using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using UnityEngine;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace PerfectionDisplay.Settings
{
	internal class Configuration
	{
		// General settings
		[NonNullable]
		[UseConverter]
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
	}
}