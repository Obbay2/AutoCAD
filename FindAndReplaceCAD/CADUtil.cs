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
			Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;

			using (Transaction myT = db.TransactionManager.StartTransaction())
			{
				foreach (ObjectInformation objInfo in items)
				{
					ObjectId id = HandleToObjectId(db, StringToHandle(objInfo.Id));
					DBObject obj = myT.GetObject(id, OpenMode.ForWrite);

					if(obj is MLeader)
					{
						MLeader mLeader = obj as MLeader;
						MText newMText = mLeader.MText.Clone() as MText;
						newMText.Contents = ReplaceWithCADEscapeCharacters(objInfo.NewText);
						mLeader.MText = newMText;
					}

					if (obj is MText)
					{
						MText mText = obj as MText;
						mText.Contents = objInfo.NewText;
					}
				}
				myT.Commit();
			}

			Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor.UpdateScreen();
		}

		public static IList<ObjectInformation> ReadCADItems()
		{
			Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
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

					if (obj is MLeader)
					{
						MLeader mLeader = obj as MLeader;
						Console.WriteLine(mLeader.MText.ContentsRTF);
						Console.WriteLine(mLeader.MText.Text);
						textFound.Add(new ObjectInformation(typeof(MLeader), mLeader.Handle.ToString(), mLeader.MText.Text.Replace("\r", "")));
					}

					if (obj is MText)
					{
						MText mText = obj as MText;

						textFound.Add(new ObjectInformation(typeof(MText), mText.Handle.ToString(), mText.Text.Replace("\r", "")));
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
			data = data.Replace("\n", @"\P");
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
			Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
			Editor ed = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Editor;

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

				ed.SetImpliedSelection(new[] { objId });
				view.CenterPoint = new Point2d(mtext.Location.X, mtext.Location.Y);
				view.Height = mtext.ActualHeight * 3;
				view.Width = mtext.ActualWidth * 3;
				ed.SetCurrentView(view);
				ed.Regen(); // Update gizmos to be accurate after movement
				myT.Commit();
			}
		}
	}
}
