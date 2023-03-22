using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;

namespace CADApp
{
    class CADUtil
    {
		public static Handle StringToHandle(string strHandle)
		{
			Handle handle = new Handle();

			try
			{
				Int64 nHandle = Convert.ToInt64(strHandle, 16);
				handle = new Handle(nHandle);
			}
			catch (FormatException)
			{
			}
			return handle;
		}

		public static ObjectId HandleToObjectId(Database db, Handle h)
		{
			ObjectId id = ObjectId.Null;
			try
			{
				id = db.GetObjectId(false, h, 0);
			}
			catch (Autodesk.AutoCAD.Runtime.Exception x)
			{
				if (x.ErrorStatus != ErrorStatus.UnknownHandle)
				{
					throw x;
				}
			}
			return id;
		}

		public static void WriteCADItems(IList<ObjectInformation> items)
		{
			Database db = Application.DocumentManager.MdiActiveDocument.Database;

			using (Transaction myT = db.TransactionManager.StartTransaction())
			{
				foreach (ObjectInformation objInfo in items)
				{
					ObjectId id = HandleToObjectId(db, StringToHandle(objInfo.Id));
					DBObject obj = myT.GetObject(id, OpenMode.ForWrite);
					TypeUtil.WriteText(obj, objInfo.NewText);
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
					// open each object to read
					DBObject obj = myT.GetObject(id, OpenMode.ForRead);
					if(TypeUtil.IsSupportedType(obj))
					{
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
		public static void MoveViewPort(string id)
		{
			Database db = Application.DocumentManager.MdiActiveDocument.Database;
			Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;

			ViewTableRecord view = ed.GetCurrentView();
			
			using (Transaction myT = db.TransactionManager.StartTransaction())
			{
				ObjectId objId = HandleToObjectId(db, StringToHandle(id));
				DBObject obj = myT.GetObject(objId, OpenMode.ForRead);


				MText mtext = null;
				if (obj is MLeader)
				{
					MLeader mLeader = obj as MLeader;
					mtext = mLeader.MText;
				}

				if (obj is MText)
				{
					MText mText = obj as MText;
					mtext = mText;
                }

				if (mtext != null)
				{
					ed.SetImpliedSelection(new[] { objId });
					view.CenterPoint = new Point2d(mtext.Location.X, mtext.Location.Y);
					view.Height = mtext.ActualHeight * 3;
					view.Width = mtext.ActualWidth * 3;
					ed.SetCurrentView(view);
					ed.Regen(); // Update gizmos to be accurate after movement
				}
				myT.Commit();
			}
		}
	}
}
