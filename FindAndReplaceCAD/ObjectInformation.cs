using Autodesk.AutoCAD.DatabaseServices;
using FindAndReplaceCAD.Util;
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

		public ObjectInformation(DBObject obj, Transaction t)
		{
			TypeUtil.TypeInformation typeInfo = TypeUtil.GetTypeInformation(obj.Id);
			this.Type = typeInfo.Type;
			this.FriendlyType = typeInfo.FriendlyName;
			this.Id = obj.Id;

			ITypeUtil typeUtil = typeInfo.TypeUtil;
			string text = typeUtil.GetText(obj, t);
			this.OriginalText = text;
			this.NewText = text;
			this.NewMask = typeUtil.GetMask(obj);
			this.CanBeMasked = typeUtil.CanMaskBeEdited(obj);
			this.CanEditText = typeUtil.CanTextBeEdited(obj);
			this.ContentType = typeUtil.GetInternalContentType(obj);
		}

		public override string ToString()
		{
			return $"{Id},\"{OriginalText}\",\"{NewText}\"";
		}
	}
}
