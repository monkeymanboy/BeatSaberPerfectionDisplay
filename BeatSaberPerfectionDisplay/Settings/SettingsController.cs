﻿using BeatSaberMarkupLanguage.Attributes;
using Zenject;

namespace PerfectionDisplay.Settings
{
	internal class SettingsController
	{
		private readonly Configuration _configuration;

		[Inject]
		public SettingsController(Configuration configuration)
		{
			_configuration = configuration;
		}

		// Disabled until in-game hit counting has been reimplemented
		/*[UIValue("showCount")]
		public bool ShowCount
		{
			get => _configuration.ShowCount;
			set => _configuration.ShowCount = value;
		}

		[UIValue("showPercentage")]
		public bool ShowPercentage
		{
			get => _configuration.ShowPercentage;
			set => _configuration.ShowPercentage = value;
		}*/

		[UIValue("hsvIntegration")]
		public bool HSVIntegration
		{
			get => _configuration.HSVIntegration;
			set => _configuration.HSVIntegration = value;
		}

		[UIValue("resultsButton")]
		public bool ResultsButton
		{
			get => _configuration.ResultsButton;
			set => _configuration.ResultsButton = value;
		}
	}
}