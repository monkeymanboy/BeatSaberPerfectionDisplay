using System;
using BeatSaberMarkupLanguage.Settings;
using Zenject;

namespace PerfectionDisplay.Settings
{
	internal class SettingsControllerManager : IInitializable, IDisposable
	{
		private SettingsController? _settingsHost;

		[Inject]
		public SettingsControllerManager(SettingsController settingsHost)
		{
			_settingsHost = settingsHost;
		}

		public void Initialize()
		{
			BSMLSettings.instance.AddSettingsMenu("Perfection Display", "PerfectionDisplay.UI.Views.settings.bsml", _settingsHost);
		}

		public void Dispose()
		{
			if (_settingsHost == null)
			{
				return;
			}

			BSMLSettings.instance.RemoveSettingsMenu(_settingsHost);
			_settingsHost = null!;
		}
	}
}