using System.ComponentModel;

namespace CADApp
{
	public class ObjectInformation : INotifyPropertyChanged
	{
		private string _newText;
		public bool IsSelected { get; set; }
		public string Id { get; }
		public string OriginalText { get; }
		public string NewText
		{
			get
			{
				return _newText;
			}
			set
			{
				if (value != _newText)
				{
					_newText = value;
					NotifyPropertyChanged("NewText");
				}
			}
		}
		public event PropertyChangedEventHandler PropertyChanged;

		public ObjectInformation(string id, string textContent)
		{
			this.Id = id;
			this.OriginalText = textContent;
			this.NewText = textContent;
		}

		public ObjectInformation(string id, string originalTextContent, string newTextContent)
		{
			this.Id = id;
			this.OriginalText = originalTextContent;
			this.NewText = newTextContent;
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public override string ToString()
		{
			return CADUtil.ReplaceStandardNewLineWithCADNewLine($"{Id},\"{OriginalText}\",\"{NewText}\"");
		}
	}
}
