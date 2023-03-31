using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using CADApp;
using System;

namespace FindAndReplaceCAD.Util
{
    internal class DBTextUtil : ITypeUtil
    {
        public override string GetText(DBObject obj, Transaction t)
        {
            DBText dbText = Cast<DBText>(obj);
            return dbText.TextString;
        }

        public override void WriteText(DBObject obj, string newText, Transaction t)
        {
            DBText dbText = Cast<DBText>(obj);
            dbText.TextString = newText;
        }

        public override bool GetMask(DBObject obj)
        {
            return false;
        }

        public override void WriteMask(DBObject obj, bool newMask)
        {
            throw new InvalidOperationException();
        }

        public override bool CanTextBeEdited(DBObject obj)
        {
            return true;
        }

        public override bool CanMaskBeEdited(DBObject obj)
        {
            return false;
        }

        public override string GetInternalContentType(DBObject obj)
        {
            return TypeUtil.DBTEXT;
        }

        public override void MoveViewPort(Editor ed, Transaction t, Entity obj)
        {
            base.MoveViewPort(ed, obj);
        }
    }
}
