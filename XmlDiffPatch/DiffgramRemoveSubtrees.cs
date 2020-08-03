// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramRemoveSubtrees
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class DiffgramRemoveSubtrees : DiffgramOperation
  {
    private XmlDiffNode _firstSourceNode;
    private XmlDiffNode _lastSourceNode;
    private bool _bSorted;

    internal DiffgramRemoveSubtrees(XmlDiffNode sourceNode, ulong operationID, bool bSorted)
      : base(operationID)
    {
      this._firstSourceNode = sourceNode;
      this._lastSourceNode = sourceNode;
      this._bSorted = bSorted;
    }

    internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
    {
      if (!this._bSorted)
        this.Sort();
      xmlWriter.WriteStartElement("xd", "remove", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      if (this._firstSourceNode == this._lastSourceNode)
        xmlWriter.WriteAttributeString("match", this._firstSourceNode.GetRelativeAddress());
      else
        xmlWriter.WriteAttributeString("match", DiffgramOperation.GetRelativeAddressOfNodeset(this._firstSourceNode, this._lastSourceNode));
      if (this._operationID != 0UL)
        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
      xmlWriter.WriteEndElement();
    }

    private void Sort()
    {
            var firstPreviousSibbling = (XmlDiffNode) null;
      XmlDiff.SortNodesByPosition(ref this._firstSourceNode, ref this._lastSourceNode, ref firstPreviousSibbling);
      this._bSorted = true;
    }

    internal bool SetNewFirstNode(XmlDiffNode srcNode)
    {
      if (this._operationID != 0UL || srcNode._nextSibling != this._firstSourceNode || (!srcNode.CanMerge || !this._firstSourceNode.CanMerge))
        return false;
      this._firstSourceNode = srcNode;
      return true;
    }

    internal bool SetNewLastNode(XmlDiffNode srcNode)
    {
      if (this._operationID != 0UL || this._lastSourceNode._nextSibling != srcNode || (!srcNode.CanMerge || !this._firstSourceNode.CanMerge))
        return false;
      this._lastSourceNode = srcNode;
      return true;
    }
  }
}
