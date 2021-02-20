using System;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using PerfectionDisplay.Models;
using PerfectionDisplay.Services;
using PerfectionDisplay.Settings;
using SiraUtil.Tools;
using TMPro;
using Zenject;

namespace PerfectionDisplay.UI
{
	internal class PerfectionDisplayResultsViewController : BSMLAutomaticViewController, IInitializable, IDisposable
	{
		private SiraLog _logger = null!;
		private Configuration _configuration = null!;
		private ResultsViewController _resultsViewController = null!;
		private ScoreProxyService _scoreProxyService = null!;

		[Inject]
		internal void Construct(SiraLog logger, Configuration configuration, ResultsViewController resultsViewController, ScoreProxyService scoreProxyService)
		{
			_logger = logger;
			_configuration = configuration;
			_scoreProxyService = scoreProxyService;
			_resultsViewController = resultsViewController;
		}

		public void Initialize()
		{
			if (_resultsViewController == null)
			{
				_logger.Warning($"{_resultsViewController} is null, not adding PerfectionDisplay");
				return;
			}

			_logger.Logger.Trace($"{_resultsViewController.GetType().FullName}");

			BSMLParser.instance.Parse(Utilities.GetResourceContent(Assembly.GetExecutingAssembly(), "PerfectionDisplay.UI.Views.results.bsml"), _resultsViewController.gameObject, this);

			_scoreProxyService.SongEnded += OnSongEnded;		}

		public void Dispose()
		{
			_scoreProxyService.SongEnded -= OnSongEnded;
		}

		private void OnSongEnded(object sender, SongEndedEventArgs e)
		{
			if (!(sender is ScoresTracker))
			{
				return;
			}

			_logger.Logger.Trace("Received song ended with scores");

			ModalButtonPositionY = e.State == LevelCompletionResults.LevelEndStateType.Cleared ? 49 : 38;
			Names = e.Names;
			Percents = e.Percents;
			Counts = e.Counts;

			NotifyPropertyChanged(nameof(ModalButtonPositionY));
			NotifyPropertyChanged(nameof(Names));
			NotifyPropertyChanged(nameof(Percents));
			NotifyPropertyChanged(nameof(Counts));
		}

		[UIValue("modal-button")]
		internal const string MODAL_BUTTON_TEXT = "Perfection\nDisplay";

		[UIValue("modal-enabled")]
		internal bool ModalEnabled => _configuration.ResultsButton;

		[UIValue("modal-button-pos-y")]
		internal int ModalButtonPositionY { get; private set; }

		[UIValue("names")]
		internal string Names { get; private set; } = "Placeholder";

		[UIValue("percents")]
		internal string Percents { get; private set; } = "Placeholder";

		[UIValue("counts")]
		internal string Counts { get; private set; } = "Placeholder";

		[UIComponent("name-mesh")]
		private TextMeshProUGUI _nameMesh = null!;

		[UIComponent("percent-mesh")]
		private TextMeshProUGUI _percentMesh = null!;

		[UIComponent("count-mesh")]
		private TextMeshProUGUI _countMesh = null!;


		[UIAction("#post-parse")]
		internal void PostParse()
		{
			_nameMesh.lineSpacing = -15f;
			_nameMesh.paragraphSpacing = -15f;

			_countMesh.lineSpacing = -15f;
			_countMesh.paragraphSpacing = -15f;

			_percentMesh.lineSpacing = -15f;
			_percentMesh.paragraphSpacing = -15f;
		}
	}
}