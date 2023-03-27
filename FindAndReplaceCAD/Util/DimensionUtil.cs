using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using CADApp;
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FindAndReplaceCAD.Util
{
    internal class DimensionUtil : ITypeUtil
    {
        public override string GetText(DBObject obj, Transaction t)
        {
            Dimension dimension = Cast<Dimension>(obj);
            return dimension.DimensionText;
        }

        public override void WriteText(DBObject obj, string newText, Transaction t)
        {
            Dimension dimension = Cast<Dimension>(obj);
            dimension.DimensionText = newText;
        }

        public override bool GetMask(DBObject obj)
        {
            Dimension dimension = Cast<Dimension>(obj);
            return dimension.Dimtfill == 1;
        }

        public override void WriteMask(DBObject obj, bool newMask)
        {
            Dimension dimension = Cast<Dimension>(obj);
            dimension.Dimtfill = newMask ? 1 : 0;
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
            Dimension dimension = Cast<Dimension>(obj);
            var dimensionBlock = t.GetObject(dimension.DimBlockId, OpenMode.ForRead) as BlockTableRecord;
            foreach (var subId in dimensionBlock)
            {
                if (TypeUtil.GetTypeInformation(subId).Type == typeof(MText))
                {
                    var mText = t.GetObject(subId, OpenMode.ForRead) as MText;
                    base.MoveViewPort(ed, view, obj, mText.Location, mText.ActualHeight, mText.ActualWidth);
                }
            }
        }
    }
}
