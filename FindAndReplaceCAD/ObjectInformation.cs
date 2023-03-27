using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.ComponentModel;

namespace CADApp
{
	public class ObjectInformation : INotifyPropertyChanged
	{
		private string _newText;
		private bool _newMask;

		public bool IsSelected { get; set; }
		public Type Type { get; }

		public string FriendlyType { get; }

		public string ContentType { get; }

		public ObjectId Id { get; }

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
					NotifyPropertyChanged(nameof(NewText));
				}
			}
		}

		public bool CanEditText { get; }

		public bool NewMask { 
			get
			{
				return _newMask;
			}
			set {
                if (value != _newMask)
                {
                    _newMask = value;
                    NotifyPropertyChanged(nameof(NewMask));
                }
            }
		}

        public bool CanBeMasked { get; }

        public event PropertyChangedEventHandler PropertyChanged;

		protected void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ObjectInformation(DBObject obj, string originalText)
		{
			this.Type = TypeUtil.GetType(obj.Id);
			this.FriendlyType = TypeUtil.getFriendlyTypeName(obj.Id);
			this.Id = obj.Id;
			this.OriginalText = originalText;
			this.NewText = originalText;
			this.NewMask = TypeUtil.IsMasked(obj);
			this.CanBeMasked = TypeUtil.CanBeMasked(obj);
			this.CanEditText = TypeUtil.CanTextBeEdited(obj);
			this.ContentType = TypeUtil.GetContentType(obj);
		}

		public override string ToString()
		{
			return $"{Id},\"{OriginalText}\",\"{NewText}\"";
		}
	}
}
