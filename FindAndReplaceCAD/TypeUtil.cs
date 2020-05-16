using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CADApp
{
    class TypeUtil
    {

		private static Dictionary<Type, string> types = new Dictionary<Type, string>() {
			{ typeof(MLeader), "Multi-Leader Text" },
			{ typeof(MText), "Multi-Line Text" },
			{ typeof(DBText), "Single Line Text" },
			{ typeof(Dimension), "Dimension Text" }
		};
		
		public static bool IsSupportedType(DBObject obj)
		{
			if(obj == null)
			{
				return false;
			}

			Type type = GetType(obj);

			if(type == null)
			{
				return false;
			}

			return types.ContainsKey(type);
		}

		public static Type GetType(DBObject obj)
		{
			switch (obj)
			{
				case MLeader _:
					return typeof(MLeader);
				case MText _:
					return typeof(MText);
				case DBText _:
					return typeof(DBText);
				case Dimension _:
					return typeof(Dimension);
				default:
					return null;
			}
		}

		public static string GetText(DBObject obj)
        {

			AssertSupportedType(obj);

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

		public static void WriteText(DBObject obj, string newText)
		{
			AssertSupportedType(obj);

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

		public static string getFriendlyTypeName(DBObject obj)
		{
			AssertSupportedType(obj);
			return types[GetType(obj)];
		}

		private static void AssertSupportedType(DBObject obj)
		{
			if (!IsSupportedType(obj)) throw new ArgumentException($"Unsupported Type: {obj.GetType()}");
		}
    }
}
