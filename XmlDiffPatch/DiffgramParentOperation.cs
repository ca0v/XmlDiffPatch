// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramParentOperation
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class DiffgramParentOperation : DiffgramOperation
    {
        internal DiffgramOperation _firstChildOp;
        internal DiffgramOperation _lastChildOp;

        internal DiffgramParentOperation(ulong operationID)
          : base(operationID)
        {
            this._firstChildOp = null;
            this._lastChildOp = null;
        }

        internal void InsertAtBeginning(DiffgramOperation newOp)
        {
            newOp._nextSiblingOp = this._firstChildOp;
            this._firstChildOp = newOp;
            if (newOp._nextSiblingOp != null)
                return;
            this._lastChildOp = newOp;
        }

        internal void InsertAtEnd(DiffgramOperation newOp)
        {
            newOp._nextSiblingOp = null;
            if (this._lastChildOp == null)
            {
                this._firstChildOp = this._lastChildOp = newOp;
            }
            else
            {
                this._lastChildOp._nextSiblingOp = newOp;
                this._lastChildOp = newOp;
            }
        }

        internal void InsertAfter(DiffgramOperation newOp, DiffgramOperation refOp)
        {
            if (refOp == null)
                return;
            newOp._nextSiblingOp = refOp._nextSiblingOp;
            refOp._nextSiblingOp = newOp;
        }

        internal void InsertOperationAtBeginning(DiffgramOperation op)
        {
            op._nextSiblingOp = this._firstChildOp;
            this._firstChildOp = op;
        }

        internal void WriteChildrenTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
        {
            for (var diffgramOperation = this._firstChildOp; diffgramOperation != null; diffgramOperation = diffgramOperation._nextSiblingOp)
                diffgramOperation.WriteTo(xmlWriter, xmlDiff);
        }

        internal bool MergeRemoveSubtreeAtBeginning(XmlDiffNode subtreeRoot)
        {
            return this._firstChildOp is DiffgramRemoveSubtrees && ( this._firstChildOp as DiffgramRemoveSubtrees ).SetNewFirstNode(subtreeRoot);
        }

        internal bool MergeRemoveSubtreeAtEnd(XmlDiffNode subtreeRoot)
        {
            return this._lastChildOp is DiffgramRemoveSubtrees && ( this._lastChildOp as DiffgramRemoveSubtrees ).SetNewLastNode(subtreeRoot);
        }

        internal bool MergeRemoveAttributeAtBeginning(XmlDiffNode subtreeRoot)
        {
            return subtreeRoot.NodeType == XmlDiffNodeType.Attribute && this._firstChildOp is DiffgramRemoveAttributes && ( this._firstChildOp as DiffgramRemoveAttributes ).AddAttribute((XmlDiffAttribute)subtreeRoot);
        }

        internal bool MergeAddSubtreeAtBeginning(XmlDiffNode subtreeRoot)
        {
            return this._firstChildOp is DiffgramAddSubtrees && ( this._firstChildOp as DiffgramAddSubtrees ).SetNewFirstNode(subtreeRoot);
        }

        internal bool MergeAddSubtreeAtEnd(XmlDiffNode subtreeRoot)
        {
            return this._lastChildOp is DiffgramAddSubtrees && ( this._lastChildOp as DiffgramAddSubtrees ).SetNewLastNode(subtreeRoot);
        }
    }
}
