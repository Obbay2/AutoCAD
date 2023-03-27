using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.GraphicsInterface;
using FindAndReplaceCAD.Util;
using System;
using System.Collections.Generic;

namespace CADApp
{
    class TypeUtil
    {
		public class TypeInformation
		{
			public string FriendlyName;
			public Type Type;
			public ITypeUtil TypeUtil;

			public TypeInformation(Type type, string friendlyName, ITypeUtil typeUtil) 
			{ 
				this.Type = type;
				this.FriendlyName = friendlyName;
				this.TypeUtil = typeUtil;
			}
		}

		private static Dictionary<string, TypeInformation> types = new Dictionary<string, TypeInformation>()
		{
			{ "MULTILEADER", new TypeInformation(typeof(MLeader), "Multi-Leader Text", new MLeaderUtil()) },
			{ "MTEXT", new TypeInformation(typeof(MText), "Multi-Line Text", new MTextUtil()) },
			{ "TEXT", new TypeInformation(typeof(DBText), "Text", new DBTextUtil())},
			{ "DIMENSION", new TypeInformation(typeof(Dimension), "Dimension", new DimensionUtil())}
		};

		public static readonly string MTEXT = "MText";
		public static readonly string DBTEXT = "Text";
		public static readonly string BLOCK = "Block";
		
		public static bool IsSupportedType(ObjectId obj)
		{
			if(obj == null)
			{
				return false;
			}

			return types.ContainsKey(obj.ObjectClass.DxfName);
		}

        public static TypeInformation GetTypeInformation(ObjectId obj)
        {
            if (types.ContainsKey(obj.ObjectClass.DxfName))
            {
                return types[obj.ObjectClass.DxfName];
            }

            return null;
        }
	}
}
