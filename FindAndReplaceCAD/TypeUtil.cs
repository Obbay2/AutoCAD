using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace CADApp
{
    class TypeUtil
    {
		class TypeInformation
		{
			public string FriendlyName;
			public Type Type;

			public TypeInformation(Type type, string friendlyName) 
			{ 
				this.Type = type;
				this.FriendlyName = friendlyName;
			}
		}

        private static Dictionary<string, TypeInformation> types = new Dictionary<string, TypeInformation>()
		{
			{ "MULTILEADER", new TypeInformation(typeof(MLeader), "Multi-Leader Text") },
			{ "MTEXT", new TypeInformation(typeof(MText), "Multi-Line Text") },
			{ "TEXT", new TypeInformation(typeof(DBText), "Text")},
			{ "DIMENSION", new TypeInformation(typeof(Dimension), "Dimension")}
		};
		
		public static bool IsSupportedType(ObjectId obj)
		{
			if(obj == null)
			{
				return false;
			}

			return types.ContainsKey(obj.ObjectClass.DxfName);
		}

		public static Type GetType(ObjectId obj)
		{
			if (types.ContainsKey(obj.ObjectClass.DxfName))
			{
				return types[obj.ObjectClass.DxfName].Type;
			}

			return null;
		}

		public static string GetText(DBObject obj)
        {
			switch (obj)
			{
				case MLeader mLeader:
					return mLeader.MText.Text;
				case MText mText:
					return mText.Text;
				case DBText dbText:
					return dbText.TextString;
				case Dimension dimension:
					return dimension.DimensionText;
				default:
					return "";
			}
		}

		public static bool IsMasked(DBObject obj)
        {
			switch (obj)
			{
				case MLeader mLeader:
					return mLeader.MText.BackgroundFill && mLeader.MText.UseBackgroundColor;
				case MText mText:
					return mText.BackgroundFill && mText.UseBackgroundColor;
				case DBText dbText:
					return false;
				case Dimension dimension:
					return dimension.Dimtfill == 2;
				default:
					return false;
			}
        }

		public static bool CanBeMasked(DBObject obj)
		{
            switch (obj)
            {
                case MLeader mLeader:
                case MText mText:
                case Dimension dimension:
					return true;
                case DBText dbText:
                default:
                    return false;
            }

        }

		public static void WriteText(DBObject obj, string newText)
		{
			switch (obj)
			{
				case MLeader mLeader:
					MText newMText = mLeader.MText.Clone() as MText;
					newMText.Contents = CADUtil.ReplaceWithCADEscapeCharacters(newText);
					mLeader.MText = newMText;
					break;
				case MText mText:
					mText.Contents = CADUtil.ReplaceWithCADEscapeCharacters(newText);
					break;
				case DBText dbText:
					dbText.TextString = newText;
					break;
				case Dimension dimension:
					dimension.DimensionText = newText;
					break;
			}
		}

		public static void WriteMask(DBObject obj, bool mask)
		{
            switch (obj)
            {
                case MLeader mLeader:
                    mLeader.MText.BackgroundFill = mask;
                    mLeader.MText.UseBackgroundColor = true;
                    break;
                case MText mText:
                    mText.BackgroundFill = mask;
                    mText.UseBackgroundColor = true;
                    break;
                case Dimension dimension:
					dimension.Dimtfill = mask ? 2 : 0;
                    break;
            }
        }

		public static string getFriendlyTypeName(ObjectId obj)
		{
			AssertSupportedType(obj);
			return types[obj.ObjectClass.DxfName].FriendlyName;
		}

		private static void AssertSupportedType(ObjectId obj)
		{
			if (!IsSupportedType(obj)) throw new ArgumentException($"Unsupported Type: {obj.GetType()}");
		}
	}
}
