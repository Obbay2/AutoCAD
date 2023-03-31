using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using FindAndReplaceCAD.Util;
using System.Collections.Generic;

namespace CADApp
{
    class CADUtil
    {
		public static void WriteCADItems(IEnumerable<ObjectInformation> items)
		{
			Database db = Application.DocumentManager.MdiActiveDocument.Database;

			using (Transaction myT = db.TransactionManager.StartTransaction())
			{
				foreach (ObjectInformation objInfo in items)
				{
					ObjectId id = objInfo.Id;
					if (TypeUtil.IsSupportedType(id))
					{
                        DBObject obj = myT.GetObject(id, OpenMode.ForWrite);

                        TypeUtil.TypeInformation typeInfo = TypeUtil.GetTypeInformation(obj.Id);
                        ITypeUtil typeUtil = typeInfo.TypeUtil;

						if (typeUtil.CanTextBeEdited(obj) && objInfo.HasTextChanged)
						{
							typeUtil.WriteText(obj, objInfo.NewText, myT);
						}

						if (typeUtil.CanMaskBeEdited(obj) && objInfo.HasMaskChanged)
						{
							typeUtil.WriteMask(obj, objInfo.NewMask);
						}
						obj.Dispose();
                    }					
				}
				myT.Commit();
			}

			Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen();
		}

		public static IList<ObjectInformation> ReadCADItems()
		{
			Database db = Application.DocumentManager.MdiActiveDocument.Database;
			TransactionManager tm = db.TransactionManager;

			List<ObjectInformation> textFound = new List<ObjectInformation>();

			using (Transaction myT = tm.StartTransaction())
			{
                BlockTable bt = (BlockTable)tm.GetObject(db.BlockTableId, OpenMode.ForRead);
				BlockTableRecord btr = (BlockTableRecord)tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
				
				// iterate through block table to locate objects
				foreach (ObjectId id in btr)
				{
					if (TypeUtil.IsSupportedType(id))
					{
                        // open each object to read
                        DBObject obj = myT.GetObject(id, OpenMode.ForRead);
						textFound.Add(new ObjectInformation(obj, myT));
						obj.Dispose();
                    }
				}
				myT.Commit();
			}

			return textFound;
		}

		/// <summary>
		/// Moves and scales the viewport to center on the CAD element specified by its object ID
		/// https://through-the-interface.typepad.com/through_the_interface/2012/12/zooming-panning-and-orbiting-the-current-autocad-view-using-net.html
		/// </summary>
		/// <param name="id">Id of the AutoCAD element</param>
		public static void MoveViewPort(ObjectId objId)
		{
			Database db = Application.DocumentManager.MdiActiveDocument.Database;
			Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

			ViewTableRecord view = ed.GetCurrentView();
			
			using (Transaction myT = db.TransactionManager.StartTransaction())
			{
				TypeUtil.TypeInformation t = TypeUtil.GetTypeInformation(objId);
				ITypeUtil typeUtil = t.TypeUtil;
				Entity obj = (Entity) myT.GetObject(objId, OpenMode.ForRead);
				typeUtil.MoveViewPort(ed, myT, obj);
				obj.Dispose();

                myT.Commit();
            }
        }
    }
}
