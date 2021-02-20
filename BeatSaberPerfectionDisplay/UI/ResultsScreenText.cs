/*using System.ComponentModel;
using System.Runtime.CompilerServices;
using BeatSaberMarkupLanguage.Attributes;
using JetBrains.Annotations;
using PerfectionDisplay.Settings;
using TMPro;
using Zenject;

namespace PerfectionDisplay.UI
{
	class ResultsScreenText : INotifyPropertyChanged
	{
		private readonly Configuration _configuration;

		private string _names = "Placeholder";
		private string _counts = "Placeholder";
		private string _percents = "Placeholder";

		[Inject]
		public ResultsScreenText(Configuration configuration)
		{
			_configuration = configuration;
		}

		[UIValue("modal-enabled")]
		public bool ModalEnabled => _configuration.ResultsButton;

		[UIValue("names")]
		public string Names
		{
			get => _names;
			private set
			{
				_names = value;
				OnPropertyChanged(nameof(Names));
			}
		}

		[UIValue("counts")]
		public string Counts
		{
			get => _counts;
			private set
			{
				_counts = value;
				OnPropertyChanged(nameof(Counts));
			}
		}

		[UIValue("percents")]
		public string Percents
		{
			get => _percents;
			set
			{
				_percents = value;
				OnPropertyChanged(nameof(Percents));
			}
		}


		[UIComponent("name-mesh")]
		private TextMeshProUGUI NameMesh { get; set; } = null!;

		[UIComponent("count-mesh")]
		private TextMeshProUGUI CountMesh { get; set; } = null!;

		[UIComponent("percent-mesh")]
		private TextMeshProUGUI PercentMesh { get; set; } = null!;


		[UIAction("#post-parse")]
		private void PostParse()
		{
			NameMesh.lineSpacing = -15f;
			NameMesh.paragraphSpacing = -15f;
			CountMesh.lineSpacing = -15f;
			CountMesh.paragraphSpacing = -15f;
			PercentMesh.lineSpacing = -15f;
			PercentMesh.paragraphSpacing = -15f;
		}


		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}*/