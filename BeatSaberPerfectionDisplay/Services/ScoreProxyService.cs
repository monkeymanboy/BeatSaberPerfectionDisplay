using System;
using PerfectionDisplay.Models;

namespace PerfectionDisplay.Services
{
	internal class ScoreProxyService
	{
		public event EventHandler<SongEndedEventArgs>? SongEnded;

		internal void NotifySongEnded(object sender, SongEndedEventArgs songEndedEventArgs)
		{
			SongEnded?.Invoke(sender, songEndedEventArgs);
		}
	}
}