using PerfectionDisplay.Services;
using Zenject;

namespace PerfectionDisplay.Installers
{
	internal class PerfectionGameInstaller : Installer<PerfectionGameInstaller>
	{
		public override void InstallBindings()
		{
			Container.BindInterfacesAndSelfTo<ScoresTracker>().AsTransient().NonLazy();
		}
	}
}