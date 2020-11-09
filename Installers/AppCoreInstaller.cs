using IPA.Config.Stores;
using IPA.Logging;
using PerfectionDisplay.Settings;
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
			Container.BindInstance(Configuration.Instance).AsSingle().NonLazy();
		}
	}
}