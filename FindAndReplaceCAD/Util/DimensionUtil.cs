using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using CADApp;

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

        public override void MoveViewPort(Editor ed, Transaction t, Entity obj)
        {
            Dimension dimension = Cast<Dimension>(obj);
            BlockTableRecord dimensionBlock = t.GetObject(dimension.DimBlockId, OpenMode.ForRead) as BlockTableRecord;
            foreach (ObjectId subId in dimensionBlock)
            {
                if (TypeUtil.GetTypeInformation(subId).Type == typeof(MText))
                {
                    MText mText = t.GetObject(subId, OpenMode.ForRead) as MText;
                    base.MoveViewPort(ed, obj);
                    mText.Dispose();
                }
            }
        }
    }
}
