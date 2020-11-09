using System.Linq;
using TMPro;
using UnityEngine;
using IPA;
using System.Reflection;
using IPA.Loader;
using PerfectionDisplay.Installers;
using SemVer;
using SiraUtil.Zenject;
using Logger = IPA.Logging.Logger;

namespace PerfectionDisplay
{
	[Plugin(RuntimeOptions.DynamicInit)]
	public class Plugin
	{
		private static PluginMetadata? _metadata;
		private static string? _name;
		private static Version? _version;

		public static string Name => _name ??= _metadata?.Name ?? Assembly.GetExecutingAssembly().GetName().Name;
		public static Version Version => _version ??= _metadata?.Version ?? new Version(Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

		public static string lastText = "";
		public static string lastPercent = "";
		public static string lastCount = "";

		public static TMP_FontAsset mainFont;

		[Init]
		public void Init(Logger logger, PluginMetadata metaData, Zenjector zenject)
		{
			_metadata = metaData;

			zenject.OnApp<AppCoreInstaller>().WithParameters(logger);
			zenject.OnMenu<MenuCoreInstaller>();
			zenject.OnGame<GameCoreInstaller>()
				.ShortCircuitForTutorial()
				.ShortCircuitForMultiplayer();
		}

		[OnEnable, OnDisable]
		public void OnPluginStateChanged()
		{
			// SiraUtil handles this for me, but just adding an empty body method to prevent warnings in the logs ^^
		}

		private void MenuSceneFresh()
		{
			/*BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "PerfectionDisplay.UI.Views.results.bsml"),
				Resources.FindObjectsOfTypeAll<ResultsViewController>().FirstOrDefault().gameObject, ResultsScreenText.instance);*/
			mainFont = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>().First(t => t.font?.name == "Teko-Medium SDF No Glow")?.font;
		}

		public void GameSceneActive()
		{
			//new GameObject("PerfectDisplay").AddComponent<PerfectDisplay>();
		}
	}
}