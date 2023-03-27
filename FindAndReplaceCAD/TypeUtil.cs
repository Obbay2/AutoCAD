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

		public static string GetText(DBObject obj, Transaction myT)
        {
			switch (obj)
			{
				case MLeader mLeader:
					if (mLeader.ContentType == ContentType.BlockContent)
					{
						return GetMLeaderText(mLeader, myT);
					}
					else if (mLeader.ContentType == ContentType.MTextContent)
					{
						return mLeader.MText.Text;
					}
					return "";
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

		private static string GetMLeaderText(MLeader obj, Transaction myT)
		{
            if (obj.HasContent() && obj.ContentType == ContentType.BlockContent)
            {
                var blockID = obj.BlockContentId;

                BlockTableRecord btr2 = myT.GetObject(blockID, OpenMode.ForRead) as BlockTableRecord;

                //List<string> attributeKeys = new List<string>();
                //if (btr2.HasAttributeDefinitions)
                //{
                //    foreach (ObjectId id2 in btr2)
                //    {
                //        if (id2.ObjectClass.DxfName == "ATTDEF")
                //        {
                //            AttributeDefinition attDef = myT.GetObject(id2, OpenMode.ForRead) as AttributeDefinition;
                //            attributeKeys.Add(attDef.Tag);
                //        }
                //    }
                //}

                var ids = btr2.GetBlockReferenceIds(true, true);
				foreach (ObjectId j in ids) {
					var br = (BlockReference)myT.GetObject(j, OpenMode.ForRead);
					foreach (ObjectId i in br.AttributeCollection)
					{
						var attRef = (AttributeReference)myT.GetObject(i, OpenMode.ForRead);
						//attRef.
						//if (attributeKeys.Contains(attRef.Tag))
						//{
						return attRef.TextString;

					}
				}
            }

			return "";
        }

		public static string GetContentType(DBObject obj)
		{
            switch (obj)
            {
                case MLeader mLeader:
                    if (mLeader.ContentType == ContentType.BlockContent)
                    {
                        return "Block";
                    }
                    else if (mLeader.ContentType == ContentType.MTextContent)
                    {
                        return "MText";
                    }
					return "None";
                case MText mText:
					return "MText";
                case DBText dbText:
					return "Text";
                case Dimension dimension:
					return "MText";
                default:
                    return "None";
            }
        }

		public static bool IsMasked(DBObject obj)
        {
			switch (obj)
			{
				case MLeader mLeader:
					if (mLeader.ContentType == ContentType.BlockContent)
                    {
                        return true;
                    }
                    else if (mLeader.ContentType == ContentType.MTextContent)
                    {
                        return mLeader.MText.BackgroundFill && mLeader.MText.UseBackgroundColor;
                    }
					return false;
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
                    if (mLeader.ContentType == ContentType.BlockContent)
                    {
                        return false;
                    }
                    else if (mLeader.ContentType == ContentType.MTextContent)
                    {
						return true;
                    }
					return false;
                case MText mText:
                case Dimension dimension:
					return true;
                case DBText dbText:
                default:
                    return false;
            }
        }

        public static bool CanTextBeEdited(DBObject obj)
        {
            switch (obj)
            {
                case MLeader mLeader:
                    if (mLeader.ContentType == ContentType.BlockContent)
                    {
                        return false;
                    }
                    else if (mLeader.ContentType == ContentType.MTextContent)
                    {
                        return true;
                    }
                    return false;
                case MText mText:
                case Dimension dimension:
                case DBText dbText:
					return true;
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
					MText newMText = mLeader.MText.Clone() as MText;
					newMText.BackgroundFill = mask;
                    newMText.UseBackgroundColor = true;
					mLeader.MText = newMText;
                    break;
                case MText mText:
                    mText.BackgroundFill = mask;
                    mText.UseBackgroundColor = true;
                    break;
                case Dimension dimension:
					dimension.Dimtfill = mask ? 1 : 0;
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
