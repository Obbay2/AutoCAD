using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
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
                        TypeUtil.WriteText(obj, objInfo.NewText);

						if (TypeUtil.CanBeMasked(obj))
						{
							TypeUtil.WriteMask(obj, objInfo.NewMask);
						}
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
                        textFound.Add(new ObjectInformation(obj));
                    }
				}
				myT.Commit();
			}

			return textFound;
		}

		/// <summary>
		/// Replaces all standard text with the escaped version needed to place back into text contents for AutoCAD
		/// https://knowledge.autodesk.com/support/autocad/learn-explore/caas/CloudHelp/cloudhelp/2020/ENU/AutoCAD-Core/files/GUID-7D8BB40F-5C4E-4AE5-BD75-9ED7112E5967-htm.html
		/// </summary>
		/// <param name="data">Text for a single element</param>
		/// <returns>String with all characters escaped for AutoCAD</returns>
		public static string ReplaceWithCADEscapeCharacters(string data)
		{
			data = data.Replace(@"\", @"\\"); // Must come first
			data = data.Replace("\r\n", @"\P");
			data = data.Replace(@"{", @"\{");
			data = data.Replace(@"}", @"\}");
			return data;
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
				Type t = TypeUtil.GetType(objId);
                if (t == typeof(MLeader))
				{
                    MLeader obj = myT.GetObject(objId, OpenMode.ForRead) as MLeader;
                    moveToText(ed, view, objId, obj.MText);
                }

				if (t == typeof(MText))
				{
                    MText obj = myT.GetObject(objId, OpenMode.ForRead) as MText;
                    moveToText(ed, view, objId, obj);
                }

                if (t == typeof(Dimension))
                {
                    var obj = myT.GetObject(objId, OpenMode.ForRead) as Dimension;
					var dimensionBlock = myT.GetObject(obj.DimBlockId, OpenMode.ForRead) as BlockTableRecord;
					foreach(var subId in dimensionBlock)
					{
                        if (TypeUtil.GetType(subId) == typeof(MText))
                        {
							var mt = myT.GetObject(subId, OpenMode.ForRead) as MText;
                            moveToText(ed, view, objId, mt);
                        }
                    }
                }

                if (t == typeof(DBText))
				{
                    DBText obj = myT.GetObject(objId, OpenMode.ForRead) as DBText;
                    moveToText(ed, view, objId, obj.Position, obj.Height, obj.Bounds.Value.MaxPoint.X - obj.Bounds.Value.MinPoint.X);
                }
                myT.Commit();
            }
        }

		private static void moveToText(Editor ed, ViewTableRecord view, ObjectId objId, Point3d position, double height, double width)
		{
            ed.SetImpliedSelection(new[] { objId });
            view.CenterPoint = new Point2d(position.X, position.Y);
            view.Height = height * 3;
            view.Width = width * 3;
            ed.SetCurrentView(view);
            ed.Regen(); // Update gizmos to be accurate after movement
        }

        private static void moveToText(Editor ed, ViewTableRecord view, ObjectId objId, MText mText)
        {
			moveToText(ed, view, objId, mText.Location, mText.ActualHeight, mText.ActualWidth);
        }
    }
}
