using PerfectionDisplay.Settings;
using PerfectionDisplay.UI;
using SiraUtil;
using SiraUtil.Tools;
using Zenject;

namespace PerfectionDisplay.Installers
{
	public class MenuCoreInstaller : Installer<MenuCoreInstaller>
	{
		private readonly SiraLog _logger;

		public MenuCoreInstaller(SiraLog logger)
		{
			_logger = logger;
		}

		public override void InstallBindings()
		{
			_logger.Debug($"Binding {nameof(PerfectionDisplayResultsViewController)}");
			Container.BindViewController<PerfectionDisplayResultsViewController>();

			_logger.Debug($"Binding {nameof(SettingsController)}");
			Container.Bind<SettingsController>().AsSingle();

			_logger.Debug($"Binding {nameof(SettingsControllerManager)}");
			Container.BindInterfacesAndSelfTo<SettingsControllerManager>().AsSingle();
		}
	}
}