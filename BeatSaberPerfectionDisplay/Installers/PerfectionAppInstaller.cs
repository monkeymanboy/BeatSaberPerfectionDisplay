using IPA.Loader;
using IPA.Logging;
using PerfectionDisplay.Services;
using PerfectionDisplay.Settings;
using SemVer;
using SiraUtil;
using Zenject;

namespace PerfectionDisplay.Installers
{
	internal class PerfectionAppInstaller : Installer<Logger, Configuration, PerfectionAppInstaller>
	{
		private readonly Logger _logger;
		private readonly Configuration _configuration;

		public PerfectionAppInstaller(Logger logger, Configuration configuration)
		{
			_logger = logger;
			_configuration = configuration;
		}

		public override void InstallBindings()
		{
			Container.BindLoggerAsSiraLogger(_logger);
			Container.BindInstance(_configuration).AsSingle();
			Container.Bind<ScoreProxyService>().AsSingle();

			var hsvPluginMetaData = PluginManager.GetPluginFromId("HitScoreVisualizer");
			if (hsvPluginMetaData != null && hsvPluginMetaData.Version >= new Version(3, 0, 2))
			{
				Container.Bind<HSVConfigProvider>().AsSingle();
			}
		}
	}
}