using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using CADApp;
using System;

namespace FindAndReplaceCAD.Util
{
    internal class MLeaderUtil : ITypeUtil
    {
        public override string GetText(DBObject obj, Transaction t)
        {
            MLeader mLeader = Cast<MLeader>(obj);
            if (mLeader.ContentType == ContentType.BlockContent)
            {
                return GetMLeaderBlockText(mLeader, t);
            }
            else if (mLeader.ContentType == ContentType.MTextContent)
            {
                MText mText = mLeader.MText;
                string output = mText.Contents;
                mText.Dispose();
                return output;
            }

            throw new InvalidOperationException();
        }

        public override void WriteText(DBObject obj, string newText, Transaction t)
        {
            MLeader mLeader = Cast<MLeader>(obj);
            if (mLeader.ContentType == ContentType.BlockContent)
            {
                throw new InvalidOperationException();
            }
            else if (mLeader.ContentType == ContentType.MTextContent)
            {
                MText newMText = mLeader.MText.Clone() as MText;
                newMText.Contents = CADUtil.ReplaceWithCADEscapeCharacters(newText);
                mLeader.MText = newMText;
                newMText.Dispose();
            }

            throw new InvalidOperationException();
        }

        public override bool GetMask(DBObject obj)
        {
            MLeader mLeader = Cast<MLeader>(obj);
            if (mLeader.ContentType == ContentType.BlockContent)
            {
                return true;
            }
            else if (mLeader.ContentType == ContentType.MTextContent)
            {
                MText mText = mLeader.MText;
                bool output = mText.BackgroundFill && mText.UseBackgroundColor;
                mText.Dispose();
                return output;
            }

            throw new InvalidOperationException();
        }

        public override void WriteMask(DBObject obj, bool newMask)
        {
            MLeader mLeader = Cast<MLeader>(obj);
            if (mLeader.ContentType == ContentType.BlockContent)
            {
            }
            else if (mLeader.ContentType == ContentType.MTextContent)
            {
                MText newMText = mLeader.MText.Clone() as MText;
                newMText.BackgroundFill = newMask;
                newMText.UseBackgroundColor = true;
                mLeader.MText = newMText;
                newMText.Dispose();
            }

            throw new InvalidOperationException();
        }

        public override bool CanTextBeEdited(DBObject obj)
        {
            MLeader mLeader = Cast<MLeader>(obj);
            if (mLeader.ContentType == ContentType.BlockContent)
            {
                return false;
            }
            else if (mLeader.ContentType == ContentType.MTextContent)
            {
                return true;
            }

            throw new InvalidOperationException();
        }

        public override bool CanMaskBeEdited(DBObject obj)
        {
            MLeader mLeader = Cast<MLeader>(obj);
            if (mLeader.ContentType == ContentType.BlockContent)
            {
                return false;
            }
            else if (mLeader.ContentType == ContentType.MTextContent)
            {
                return true;
            }

            throw new InvalidOperationException();
        }

        private string GetMLeaderBlockText(MLeader obj, Transaction myT)
        {
            BlockTableRecord btr2 = myT.GetObject(obj.BlockContentId, OpenMode.ForRead) as BlockTableRecord;
            if (btr2.HasAttributeDefinitions)
            {
                foreach (ObjectId id2 in btr2)
                {
                    if (id2.ObjectClass.DxfName == "ATTDEF")
                    {
                        AttributeDefinition attDef = myT.GetObject(id2, OpenMode.ForRead) as AttributeDefinition;
                        AttributeReference attRef = obj.GetBlockAttribute(attDef.Id);
                        string output = attRef.TextString;
                        attRef.Dispose();
                        return output;
                    }
                }
            }

            return "";
        }

        public override string GetInternalContentType(DBObject obj)
        {
            MLeader mLeader = Cast<MLeader>(obj);

            if (mLeader.ContentType == ContentType.BlockContent)
            {
                return TypeUtil.BLOCK;
            }
            else if (mLeader.ContentType == ContentType.MTextContent)
            {
                return TypeUtil.MTEXT;
            }

            return "";
        }

        public override void MoveViewPort(Editor ed, ViewTableRecord view, Transaction t, DBObject obj)
        {
            MLeader mLeader = Cast<MLeader>(obj);

            if (mLeader.ContentType == ContentType.BlockContent)
            {
                throw new InvalidOperationException();
            }
            else if (mLeader.ContentType == ContentType.MTextContent)
            {
                MText mText = mLeader.MText;
                base.MoveViewPort(ed, view, obj, mText.Location, mText.ActualHeight, mText.ActualWidth);
                mText.Dispose();
            }

            
        }
    }
}
