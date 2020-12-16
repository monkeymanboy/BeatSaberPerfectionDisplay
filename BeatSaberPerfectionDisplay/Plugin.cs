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

		[Init]
		public void Init(Logger logger, PluginMetadata metaData, Zenjector zenject)
		{
			_metadata = metaData;

			zenject.OnApp<AppCoreInstaller>().WithParameters(logger);
			zenject.OnMenu<MenuCoreInstaller>().OnlyForStandard();
			zenject.OnGame<GameCoreInstaller>()
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