using System;
using System.ComponentModel;

namespace CADApp
{
	public class ObjectInformation : INotifyPropertyChanged
	{
		private string _newText;
		public bool IsSelected { get; set; }
		public string Type { get; }

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

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObjectInformation(Type type, string id, string textContent) : this(type, id, textContent, textContent) { }

		public ObjectInformation(Type type, string id, string originalTextContent, string newTextContent)
		{
			this.Type = type.Name;
			this.Id = id;
			this.OriginalText = originalTextContent;
			this.NewText = newTextContent;
		}

		public override string ToString()
		{
			return $"{Id},\"{OriginalText}\",\"{NewText}\"";
		}
	}
}
