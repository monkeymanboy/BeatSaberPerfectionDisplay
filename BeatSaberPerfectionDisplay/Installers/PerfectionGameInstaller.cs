using PerfectionDisplay.Services;
using SiraUtil.Tools;
using Zenject;

namespace PerfectionDisplay.Installers
{
	public class PerfectionGameInstaller : Installer<PerfectionGameInstaller>
	{
		private readonly SiraLog _logger;

		public PerfectionGameInstaller(SiraLog logger)
		{
			_logger = logger;
		}

		public override void InstallBindings()
		{
			_logger.Debug($"Binding {nameof(ScoresTracker)}");
			Container.BindInterfacesAndSelfTo<ScoresTracker>().AsTransient().NonLazy();
		}
	}
}