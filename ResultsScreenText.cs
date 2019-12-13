using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using TMPro;

namespace PerfectionDisplay
{
    class ResultsScreenText : NotifiableSingleton<ResultsScreenText>
    {
        [UIValue("modal-enabled")]
        public bool ModalEnabled => Settings.instance.resultsButton;

        private string names = "Placeholder";
        [UIValue("names")]
        public string Names
        {
            get => names;
            set
            {
                names = value;
                NotifyPropertyChanged();
            }
        }
        private string counts = "Placeholder";
        [UIValue("counts")]
        public string Counts
        {
            get => counts;
            set
            {
                counts = value;
                NotifyPropertyChanged();
            }
        }
        private string percents = "Placeholder";
        [UIValue("percents")]
        public string Percents
        {
            get => percents;
            set
            {
                percents = value;
                NotifyPropertyChanged();
            }
        }
        [UIComponent("name-mesh")]
        private TextMeshProUGUI nameMesh;
        [UIComponent("count-mesh")]
        private TextMeshProUGUI countMesh;
        [UIComponent("percent-mesh")]
        private TextMeshProUGUI percentMesh;
        [UIAction("#post-parse")]
        private void PostParse()
        {
            nameMesh.lineSpacing = -15f;
            nameMesh.paragraphSpacing = -15f;
            countMesh.lineSpacing = -15f;
            countMesh.paragraphSpacing = -15f;
            percentMesh.lineSpacing = -15f;
            percentMesh.paragraphSpacing = -15f;
        }
    }
}
