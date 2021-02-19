using PerfectionDisplay.Settings;
using PerfectionDisplay.UI;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace PerfectionDisplay.Installers
{
	public class PerfectionMenuInstaller : Installer<PerfectionMenuInstaller>
	{
		private readonly SiraLog _logger;

		public PerfectionMenuInstaller(SiraLog logger)
		{
			_logger = logger;
		}

		public override void InstallBindings()
		{
			_logger.Debug($"Binding {nameof(PerfectionDisplayResultsViewController)}");
			Container.Bind<PerfectionDisplayResultsViewController>().FromNewComponentAsViewController().AsSingle();

			_logger.Debug($"Binding {nameof(SettingsController)}");
			Container.Bind<SettingsController>().AsSingle();

			_logger.Debug($"Binding {nameof(SettingsControllerManager)}");
			Container.BindInterfacesTo<SettingsControllerManager>().AsSingle();
		}
	}
}