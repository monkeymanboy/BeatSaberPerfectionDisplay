using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Logging;
using PerfectionDisplay.Installers;
using PerfectionDisplay.Settings;
using SiraUtil.Zenject;

namespace PerfectionDisplay
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		[Init]
		public Plugin(Logger logger, Config config, Zenjector zenject)
		{
			zenject.UseLogger(logger);

			zenject.Install<PerfectionAppInstaller>(Location.App, config.Generated<Configuration>());
			zenject.Install<PerfectionMenuInstaller>(Location.Menu);
			zenject.Install<PerfectionGameInstaller>(Location.StandardPlayer);
		}

		[OnEnable, OnDisable]
		public void OnPluginStateChanged()
		{
			// SiraUtil handles this for me, but just adding an empty body method to prevent warnings in the logs ^^
		}
	}
}