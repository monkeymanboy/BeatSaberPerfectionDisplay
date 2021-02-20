using PerfectionDisplay.Settings;
using PerfectionDisplay.UI;
using SiraUtil;
using Zenject;

namespace PerfectionDisplay.Installers
{
	internal class PerfectionMenuInstaller : Installer<PerfectionMenuInstaller>
	{
		public override void InstallBindings()
		{
			Container.Bind<PerfectionDisplayResultsViewController>().FromNewComponentAsViewController().AsSingle();
			Container.Bind<SettingsController>().AsSingle();
			Container.BindInterfacesTo<SettingsControllerManager>().AsSingle();
		}
	}
}