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
		public static Handle StringToHandle(String strHandle)
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

		static public void WriteCADItems(IList<ObjectInformation> items)
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
						MText newMText = (MText)mLeader.MText.Clone();
						newMText.Contents = objInfo.NewText;
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

		static public IList<DisplayableObjectInformation> ReadCADItems()
		{
			Database db = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.Database;
			TransactionManager tm = db.TransactionManager;

			List<DisplayableObjectInformation> textFound = new List<DisplayableObjectInformation>();

			using (Transaction myT = tm.StartTransaction())
			{

				BlockTable bt = (BlockTable)tm.GetObject(db.BlockTableId, OpenMode.ForRead);
				BlockTableRecord btr = (BlockTableRecord)tm.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
				// iterate through block table to locate mtext objects
				// this foreach loop will loop through the block table record looking at each object in the drawing.
				foreach (ObjectId id in btr)
				{
					// open each object to read
					DBObject obj = myT.GetObject(id, OpenMode.ForRead);

					if (obj is MLeader)
					{
						MLeader mLeader = obj as MLeader;
						textFound.Add(new DisplayableObjectInformation(typeof(MLeader), mLeader.Handle.ToString(), mLeader.MText.Contents));
					}

					if (obj is MText)
					{
						MText mText = obj as MText;
						textFound.Add(new DisplayableObjectInformation(typeof(MText), mText.Handle.ToString(), mText.Contents));
					}
				}
				myT.Commit();
			}

			return textFound;
		}

		static public string ReplaceCADNewLineWithStandardNewLine(string data)
		{
			int backslashCount = 0;
			for (int i = 0; i < data.Length; i++)
			{
				if (data[i] == '\\')
				{
					backslashCount++;
					continue;
				}

				if (data[i] == 'P' && backslashCount % 2 == 1)
				{
					data = data.Remove(i - 1, 2);
					i--;
					data = data.Insert(i, "\n");
					i--;
					continue;
				}

				backslashCount = 0;
			}

			return data;
		}

		static public string ReplaceStandardNewLineWithCADNewLine(string data)
		{
			return data.Replace("\n", "\\P");
		}

		static public void MoveViewPort(string id)
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
