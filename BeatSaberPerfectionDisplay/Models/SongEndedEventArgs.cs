using System;

namespace PerfectionDisplay.Models
{
	public class SongEndedEventArgs : EventArgs
	{
		public LevelCompletionResults.LevelEndStateType State { get; set; }
		public string Names { get; set; } = "Placeholder";
		public string Counts { get; set; } = "Placeholder";
		public string Percents { get; set; } = "Placeholder";
	}
}