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
        public abstract void MoveViewPort(Editor ed, ViewTableRecord view, Transaction t, DBObject obj);
        protected void MoveViewPort(Editor ed, ViewTableRecord view, DBObject entity, Point3d position, double height, double width)
        {
            ed.SetImpliedSelection(new[] { entity.Id });
            view.CenterPoint = new Point2d(position.X, position.Y);
            view.Height = height * 3;
            view.Width = width * 3;
            ed.SetCurrentView(view);
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
    }
}