using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.ComponentModel;

namespace CADApp
{
	public class ObjectInformation : INotifyPropertyChanged
	{
		private string _newText;
		public bool IsSelected { get; set; }
		public Type Type { get; }

		public string FriendlyType { get; }

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

		public ObjectInformation(DBObject obj) : this(obj, TypeUtil.GetText(obj), TypeUtil.GetText(obj)) { }

		public ObjectInformation(DBObject obj, string originalTextContent) : this(obj, originalTextContent, originalTextContent) { }

		public ObjectInformation(DBObject obj, string originalTextContent, string newTextContent)
		{
			this.Type = TypeUtil.GetType(obj);
			this.FriendlyType = TypeUtil.getFriendlyTypeName(obj);
			this.Id = obj.Handle.ToString();
			this.OriginalText = originalTextContent;
			this.NewText = newTextContent;
		}

		public override string ToString()
		{
			return $"{Id},\"{OriginalText}\",\"{NewText}\"";
		}
	}
}
