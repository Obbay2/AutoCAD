using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using CADApp;

namespace FindAndReplaceCAD.Util
{
    internal class MTextUtil : ITypeUtil
    {
        public override string GetText(DBObject obj, Transaction t)
        {
            MText mText = Cast<MText>(obj);
            return mText.Contents;
        }

        public override void WriteText(DBObject obj, string newText, Transaction t)
        {
            MText mText = Cast<MText>(obj);
            mText.Contents = CADUtil.ReplaceWithCADEscapeCharacters(newText);
        }

        public override bool GetMask(DBObject obj)
        {
            MText mText = Cast<MText>(obj);
            return mText.BackgroundFill && mText.UseBackgroundColor;
        }

        public override void WriteMask(DBObject obj, bool newMask)
        {
            MText mText = Cast<MText>(obj);
            mText.BackgroundFill = newMask;
            mText.UseBackgroundColor = true;
        }

        public override bool CanTextBeEdited(DBObject obj)
        {
            return true;
        }

        public override bool CanMaskBeEdited(DBObject obj)
        {
            return true;
        }

        public override string GetInternalContentType(DBObject obj)
        {
            return TypeUtil.MTEXT;
        }

        public override void MoveViewPort(Editor ed, ViewTableRecord view, Transaction t, DBObject obj)
        {
            MText mText = Cast<MText>(obj);
            base.MoveViewPort(ed, view, obj, mText.Location, mText.ActualHeight, mText.ActualWidth);
        }
    }
}
