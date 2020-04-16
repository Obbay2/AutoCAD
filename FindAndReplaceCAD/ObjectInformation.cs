using CsvHelper.Configuration.Attributes;
using System;
using System.ComponentModel;

namespace CADApp
{
	public abstract class ObjectInformation
	{
		[Index(0)]
		public string Type { get; set; }

		[Index(1)]
		public string Id { get; set; }

		[Index(2)]
		public string OriginalText { get; set; }

		[Index(3)]
		public string NewText { get; set; }

		public override string ToString()
		{
			return CADUtil.ReplaceStandardNewLineWithCADNewLine($"{Id},\"{OriginalText}\",\"{NewText}\"");
		}
	}

	public class DisplayableObjectInformation : ObjectInformation, INotifyPropertyChanged
	{
		private string _displayableNewText;
		private string _displayableOriginalText;

		[Ignore]
		public bool IsSelected { get; set; }

		[Ignore]
		public string DisplayableOriginalText
		{
			get
			{
				return this._displayableOriginalText;
			}
			set
			{
				if (value != this._displayableOriginalText)
				{
					this._displayableOriginalText = value;
					this.OriginalText = CADUtil.ReplaceStandardNewLineWithCADNewLine(value);
				}
			}
		}

		[Ignore]
		public string DisplayableNewText
		{
			get
			{
				return this._displayableNewText;
			}
			set
			{
				if (value != this._displayableNewText)
				{
					this._displayableNewText = value;
					this.NewText = CADUtil.ReplaceStandardNewLineWithCADNewLine(value);
					NotifyPropertyChanged("DisplayableNewText");
				}
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public DisplayableObjectInformation(Type type, string id, string textContent) : this(type, id, textContent, textContent) { }

		public DisplayableObjectInformation(Type type, string id, string originalTextContent, string newTextContent)
		{
			this.Type = type.Name;
			this.Id = id;
			this.DisplayableOriginalText = originalTextContent;
			this.DisplayableNewText = newTextContent;
		}

	}
}
