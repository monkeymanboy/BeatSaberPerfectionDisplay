using IPA;
using IPA.Config;
using IPA.Config.Stores;
using PerfectionDisplay.Installers;
using PerfectionDisplay.Settings;
using SiraUtil.Zenject;
using Logger = IPA.Logging.Logger;

namespace PerfectionDisplay
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		[Init]
		public void Init(Logger logger, Config config, Zenjector zenject)
		{
			zenject.OnApp<PerfectionAppInstaller>().WithParameters(logger, config.Generated<Configuration>());
			zenject.OnMenu<PerfectionMenuInstaller>().OnlyForStandard();
			zenject.OnGame<PerfectionGameInstaller>()
				.OnlyForStandard();
		}

		[OnEnable, OnDisable]
		public void OnPluginStateChanged()
		{
			// SiraUtil handles this for me, but just adding an empty body method to prevent warnings in the logs ^^
		}

		/*private void MenuSceneFresh()
		{
			mainFont = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().First(t => t.font?.name == "Teko-Medium SDF No Glow")?.font;
		}*/
	}
}