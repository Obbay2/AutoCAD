using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;

namespace FindAndReplaceCAD.Util
{
    internal abstract class ITypeUtil
    {
        public abstract bool CanMaskBeEdited(DBObject entity);
        public abstract bool CanTextBeEdited(DBObject entity);
        public abstract bool GetMask(DBObject entity);
        public abstract string GetText(DBObject entity, Transaction t);
        public abstract void WriteMask(DBObject entity, bool newMask);
        public abstract void WriteText(DBObject entity, string newText, Transaction t);
        public abstract string GetInternalContentType(DBObject entity);
        public abstract void MoveViewPort(Editor ed, Transaction t, Entity entity);
        protected void MoveViewPort(Editor ed, Entity entity)
        {
            if (LayoutManager.Current.CurrentLayout != "Model")
            {
                var doc = Application.DocumentManager.MdiActiveDocument;
                using (doc.LockDocument())
                {
                    LayoutManager.Current.CurrentLayout = "Model";
                }
            }
            Extents3d ext = entity.GeometricExtents;
            ext.TransformBy(ed.CurrentUserCoordinateSystem.Inverse());
            ed.SetImpliedSelection(new[] { entity.Id });
            ZoomWin(ed, ext.MinPoint, ext.MaxPoint);
            ed.Regen(); // Update gizmos to be accurate after movement
        }

        protected T Cast<T>(DBObject obj) where T : DBObject
        {
            if (obj is T)
            {
                return obj as T;
            }

            throw new InvalidOperationException();
        }

        /// <summary>
        /// Moves and scales the viewport to center on the CAD element
        /// https://through-the-interface.typepad.com/through_the_interface/2008/06/zooming-to-a-wi.html
        /// http://docs.autodesk.com/ACD/2010/ENU/AutoCAD%20.NET%20Developer's%20Guide/index.html?url=WS1a9193826455f5ff2566ffd511ff6f8c7ca-35da.htm,topicNumber=d0e44236
        /// </summary>
        private static void ZoomWin(Editor ed, Point3d min, Point3d max)
        {
            Point2d min2d = new Point2d(min.X, min.Y);
            Point2d max2d = new Point2d(max.X, max.Y);

            ViewTableRecord view = new ViewTableRecord();
            view.CenterPoint = min2d + ((max2d - min2d) / 2.0);
            view.Height = (max2d.Y - min2d.Y) * 1.5;
            view.Width = (max2d.X - min2d.X) * 1.5;
            ed.SetCurrentView(view);
        }
    }
}