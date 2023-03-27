﻿using Autodesk.AutoCAD.DatabaseServices;
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
            dbText.TextString = CADUtil.ReplaceWithCADEscapeCharacters(newText);
        }

        public override bool GetMask(DBObject obj)
        {
            throw new InvalidOperationException();
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

        public override void MoveViewPort(Editor ed, ViewTableRecord view, Transaction t, DBObject obj)
        {
            DBText dbText = Cast<DBText>(obj);
            base.MoveViewPort(ed, view, obj, dbText.Position, dbText.Height, dbText.Bounds.Value.MaxPoint.X - dbText.Bounds.Value.MinPoint.X);
        }
    }
}