using IPA.Config.Stores;
using IPA.Loader;
using IPA.Logging;
using PerfectionDisplay.Services;
using PerfectionDisplay.Settings;
using SemVer;
using SiraUtil;
using Zenject;
using Config = IPA.Config.Config;

namespace PerfectionDisplay.Installers
{
	public class AppCoreInstaller : Installer<Logger, AppCoreInstaller>
	{
		private readonly Logger _logger;

		public AppCoreInstaller(Logger logger)
		{
			_logger = logger;
		}

		public override void InstallBindings()
		{
			_logger.Debug("Binding logger");
			Container.BindLoggerAsSiraLogger(_logger);

			_logger.Debug($"Binding {nameof(Configuration)}");
			Configuration.Instance ??= Config.GetConfigFor(Plugin.Name).Generated<Configuration>();
			Container.BindInstance(Configuration.Instance).AsSingle();

			Container.Bind<ScoreProxyService>().AsSingle();

			var hsvPluginMetaData = PluginManager.GetPluginFromId("HitScoreVisualizer");
			if (hsvPluginMetaData != null && hsvPluginMetaData.Version >= new Version(3, 0, 2))
			{
				_logger.Debug($"Binding {nameof(HSVConfigProvider)}");
				Container.Bind<HSVConfigProvider>().AsSingle();
			}
		}
	}
}