using PerfectionDisplay.Services;
using SiraUtil.Tools;
using Zenject;

namespace PerfectionDisplay.Installers
{
	public class GameCoreInstaller : Installer<GameCoreInstaller>
	{
		private readonly SiraLog _logger;

		public GameCoreInstaller(SiraLog logger)
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