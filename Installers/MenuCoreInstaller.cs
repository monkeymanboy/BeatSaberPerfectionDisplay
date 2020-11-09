using PerfectionDisplay.Settings;
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
			_logger.Debug($"Binding {nameof(SettingsController)}");
			Container.BindInterfacesAndSelfTo<SettingsController>().AsSingle();

			_logger.Debug($"Binding {nameof(SettingsControllerManager)}");
			Container.BindInterfacesAndSelfTo<SettingsControllerManager>().AsSingle();
		}
	}
}