using System.Linq;
using IPA;
using IPA.Loader;
using ModestTree;
using PerfectionDisplay.Settings;
using SiraUtil.Tools;
using UnityEngine;
using Zenject;

namespace PerfectionDisplay.Services
{
	public class ScoresTracker
	{
		private readonly SiraLog _logger;
		private readonly ScoreController _scoreController;
		private readonly ILevelEndActions _levelEndActions;
		private readonly HitScoreVisualizer.Services.ConfigProvider? _configProvider;

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
		private bool _shouldHitscore = true;


		[Inject]
		public ScoresTracker(SiraLog logger, ScoreController scoreController, ILevelEndActions levelEndActions, Configuration configuration, [InjectOptional] HitScoreVisualizer.Services.ConfigProvider? configProvider)
		{
			_logger = logger;

			_logger.Debug("Entered ScoreTracker constructor");

			Assert.IsNotNull(scoreController, "ScoreController was null I guess?");
			_scoreController = scoreController;
			Assert.IsNotNull(levelEndActions, "levelEndActions was null I guess?");
			_levelEndActions = levelEndActions;
			Assert.IsNotNull(configProvider, "configProvider was null I guess?");
			_configProvider = configProvider;

			_logger.Debug("Constructed ScoreTracker");

			_scoreController.noteWasCutEvent += OnNoteCut;
			_scoreController.noteWasMissedEvent += OnNoteMiss;

			_levelEndActions.levelFailedEvent += OnLevelExit;
			_levelEndActions.levelFinishedEvent += OnLevelExit;

			var hsvConfig = configProvider?.GetCurrentConfig();
			if (configuration.HSVIntegration && hsvConfig != null)
			{

			}
			else
			{

			}
			//LoadConfig()
		}

		private void OnNoteMiss(NoteData noteData, int multiplier)
		{
			if (noteData.colorType == ColorType.None)
			{
				return;
			}


			_misses++;
			_notes++;

			_logger.Logger.Trace($"Missed note, total misses: {_misses}, total notes: {_notes}");

			UpdateText();
		}

		private void OnNoteCut(NoteData noteData, NoteCutInfo noteCutInfo, int multiplier)
		{
			_notes++;

			if (!noteCutInfo.allIsOK)
			{
				_misses++;

				_logger.Logger.Trace($"Bad cut, total misses: {_misses}, total notes: {_notes}");

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

				_logger.Logger.Trace($"Decent cut, score: {total}, total notes: {_notes}");


				/*for (var i = 0; i < _scoreRanges.Length; i++)
				{
					if (_scoreRanges[i] < total)
					{
						_scoreCount[i]++;
						UpdateText();
						return;
					}
				}

				_scoreCount[_scoreRanges.Length]++;*/
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

		private void OnLevelExit()
		{
			_scoreController.noteWasCutEvent -= OnNoteCut;
			_scoreController.noteWasMissedEvent -= OnNoteMiss;

			_levelEndActions.levelFailedEvent -= OnLevelExit;
			_levelEndActions.levelFinishedEvent -= OnLevelExit;

			_logger.Logger.Trace($"Level exit, total misses: {_misses}, total notes: {_notes}");

			/*Plugin.lastText = "Range\n";
			for (var i = 0; i < _scoreRanges.Length; i++)
			{
				Plugin.lastText += "<color=" + _colors[i] + ">" + (_shouldHitscore ? _hitScoreNames[i] : (">" + _scoreRanges[i])) + "\n";
			}

			Plugin.lastText += "<color=" + _colors[_scoreRanges.Length] + ">" + (_shouldHitscore ? _hitScoreNames[_scoreRanges.Length] : ("<" + _scoreRanges[_scoreRanges.Length - 1])) + "\n";
			Plugin.lastText += "<color=" + _colors[_scoreRanges.Length + 1] + ">" + "MISS";
			Plugin.lastCount = "Count\n";
			for (var i = 0; i < _scoreRanges.Length; i++)
			{
				Plugin.lastCount += "<color=" + _colors[i] + ">" + _scoreCount[i] + "\n";
			}

			Plugin.lastCount += "<color=" + _colors[_scoreRanges.Length] + ">" + _scoreCount[_scoreRanges.Length] + "\n";
			Plugin.lastCount += "<color=" + _colors[_scoreRanges.Length + 1] + ">" + _misses;
			Plugin.lastPercent = "Percent\n";
			for (var i = 0; i < _scoreRanges.Length; i++)
			{
				Plugin.lastPercent += "<color=" + _colors[i] + ">" + GetPercent(_scoreCount[i]) + "%\n";
			}

			Plugin.lastPercent += "<color=" + _colors[_scoreRanges.Length] + ">" + GetPercent(_scoreCount[_scoreRanges.Length]) + "%\n";
			Plugin.lastPercent += "<color=" + _colors[_scoreRanges.Length + 1] + ">" + GetPercent(_misses) + "%";*/
		}

		private string GetPercent(int hits)
		{
			return hits == 0 ? "0" : (((float)hits / _notes) * 100).ToString("0.0");
		}
	}
}