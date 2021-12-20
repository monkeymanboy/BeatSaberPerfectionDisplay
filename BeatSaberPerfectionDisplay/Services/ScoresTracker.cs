using PerfectionDisplay.Models;
using PerfectionDisplay.Settings;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace PerfectionDisplay.Services
{
	internal class ScoresTracker
	{
		private readonly SiraLog _logger;
		private readonly ScoreProxyService _scoreProxyService;
		private readonly ScoreController _scoreController;
		private readonly ILevelEndActions _levelEndActions;

		private int _misses = 0;
		private int _notes = 0;

		private Vector3 _displayPosition;
		private int[] _scoreRanges;
		private string[] _hitScoreNames;
		private string[] _colors;

		private int[] _scoreCount;

		//private DisplaySection[] _sections;
		private bool _showNumbers = false;
		private bool _showPercent = false;
		private readonly bool _shouldHitScore;


		[Inject]
		public ScoresTracker(SiraLog logger, ScoreProxyService scoreProxyService, ScoreController scoreController, ILevelEndActions levelEndActions,
			Configuration configuration, [InjectOptional] HSVConfigProvider? configProvider)
		{
			_logger = logger;
			_scoreProxyService = scoreProxyService;

			_logger.Debug("Entered ScoreTracker constructor");

			_scoreController = scoreController;
			_levelEndActions = levelEndActions;

			_logger.Debug("Constructed ScoreTracker");

			_scoreController.noteWasCutEvent += OnNoteCut;
			_scoreController.noteWasMissedEvent += OnNoteMiss;

			_levelEndActions.levelFailedEvent += OnSongFailed;
			_levelEndActions.levelFinishedEvent += OnSongFinished;


			_displayPosition = configuration.Position;
			_scoreRanges = configuration.ScoreRanges.ToArray();
			_colors = configuration.Colors.ToArray();
			_shouldHitScore = configuration.HSVIntegration;
			if (_shouldHitScore && configProvider != null)
			{
				_shouldHitScore = configProvider.Enrich(ref _scoreRanges, ref _hitScoreNames!, ref _colors);
			}
			else
			{
				_shouldHitScore = false;
			}

			_showNumbers = configuration.ShowCount;
			_showPercent = configuration.ShowPercentage;
			_scoreCount = new int[_scoreRanges.Length + 1];
		}

		private void OnNoteMiss(NoteData noteData, int multiplier)
		{
			if (noteData.colorType == ColorType.None)
			{
				return;
			}


			_misses++;
			_notes++;

			_logger.Trace($"Missed note, total misses: {_misses}, total notes: {_notes}");

			UpdateText();
		}

		private void OnNoteCut(NoteData noteData, NoteCutInfo noteCutInfo, int multiplier)
		{
			_notes++;

			if (!noteCutInfo.allIsOK)
			{
				_misses++;

				_logger.Trace($"Bad cut, total misses: {_misses}, total notes: {_notes}");

				UpdateText();
				return;
			}

			var didDone = false;
			noteCutInfo.swingRatingCounter.didFinishEvent += e =>
			{
				if (didDone)
				{
					return;
				}

				didDone = true;
				ScoreModel.RawScoreWithoutMultiplier(noteCutInfo, out var before, out var after, out var distScore);
				var total = before + after + distScore;

				_logger.Trace($"Decent cut, score: {total}, total notes: {_notes}");

				for (var i = 0; i < _scoreRanges.Length; i++)
				{
					if (_scoreRanges[i] >= total)
					{
						continue;
					}

					_scoreCount[i]++;
					UpdateText();
					return;
				}

				_scoreCount[_scoreRanges.Length]++;
				UpdateText();
			};
		}

		private void UpdateText()
		{
			/*if (!_showNumbers && !_showPercent)
			{
				return;
			}

			float width = 0;
			for (var i = 0; i < _scoreCount.Length; i++)
			{
				_sections[i].UpdateText(_scoreCount[i], GetPercent(_scoreCount[i]));
				width += _sections[i].GetWidth();
			}

			_sections[_scoreRanges.Length + 1].UpdateText(_misses, GetPercent(_misses));
			width += _sections[_scoreRanges.Length + 1].GetWidth();

			var curX = _sections[0].GetWidth() / 2;
			for (var i = 0; i < _scoreCount.Length; i++)
			{
				_sections[i].UpdatePosition(-(width / 2) + curX);
				curX += _sections[i].GetWidth() / 2;
				curX += _sections[i + 1].GetWidth() / 2;
			}

			_sections[_scoreRanges.Length + 1].UpdatePosition(-(width / 2) + curX);*/
		}

		private void OnSongFailed()
		{
			OnLevelExit(LevelCompletionResults.LevelEndStateType.Failed);
		}

		private void OnSongFinished()
		{
			OnLevelExit(LevelCompletionResults.LevelEndStateType.Cleared);
		}

		private void OnLevelExit(LevelCompletionResults.LevelEndStateType levelEndStateType)
		{
			_scoreController.noteWasCutEvent -= OnNoteCut;
			_scoreController.noteWasMissedEvent -= OnNoteMiss;

			_levelEndActions.levelFailedEvent -= OnSongFailed;
			_levelEndActions.levelFinishedEvent -= OnSongFinished;

			_logger.Trace($"Level exit, total misses: {_misses}, total notes: {_notes}");

			var lastText = "Range\n";
			for (var i = 0; i < _scoreRanges.Length; i++)
			{
				lastText += "<color=" + _colors[i] + ">" + (_shouldHitScore ? _hitScoreNames[i] : (">" + _scoreRanges[i])) + "\n";
			}

			lastText += "<color=" + _colors[_scoreRanges.Length] + ">" + (_shouldHitScore ? _hitScoreNames[_scoreRanges.Length] : ("<" + _scoreRanges[_scoreRanges.Length - 1])) + "\n";
			lastText += "<color=" + _colors[_scoreRanges.Length + 1] + ">" + "MISS";

			var lastCount = "Count\n";
			for (var i = 0; i < _scoreRanges.Length; i++)
			{
				lastCount += "<color=" + _colors[i] + ">" + _scoreCount[i] + "\n";
			}

			lastCount += $"<color={_colors[_scoreRanges.Length]}>{_scoreCount[_scoreRanges.Length]}\n" +
			             $"<color={_colors[_scoreRanges.Length + 1]}>{_misses}";

			var lastPercent = "Percent\n";
			for (var i = 0; i < _scoreRanges.Length; i++)
			{
				lastPercent += $"<color={_colors[i]}>{GetPercent(_scoreCount[i])}%\n";
			}

			lastPercent += $"<color={_colors[_scoreRanges.Length]}>{GetPercent(_scoreCount[_scoreRanges.Length])}%\n" +
			               $"<color={_colors[_scoreRanges.Length + 1]}>{GetPercent(_misses)}%";

			_logger.Trace("Notifying song ended with scores");
			_scoreProxyService.NotifySongEnded(this, new SongEndedEventArgs {State = levelEndStateType, Names = lastText, Percents = lastPercent, Counts = lastCount});
		}

		private string GetPercent(int hits)
		{
			return hits == 0 ? "0" : (((float) hits / _notes) * 100).ToString("0.0");
		}
	}
}