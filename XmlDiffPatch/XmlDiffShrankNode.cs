// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffShrankNode
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffShrankNode : XmlDiffNode
  {
    internal XmlDiffNode _firstNode;
    internal XmlDiffNode _lastNode;
    private XmlDiffShrankNode _matchingShrankNode;
    private string _localAddress;
    private ulong _opid;

    internal XmlDiffShrankNode(XmlDiffNode firstNode, XmlDiffNode lastNode)
      : base(-1)
    {
      this._firstNode = firstNode;
      this._lastNode = lastNode;
      this._matchingShrankNode = (XmlDiffShrankNode) null;
            var xmlDiffNode = firstNode;
      while (true)
      {
        this._hashValue += (this._hashValue << 7) + xmlDiffNode.HashValue;
        if (xmlDiffNode != lastNode)
          xmlDiffNode = xmlDiffNode._nextSibling;
        else
          break;
      }
      this._localAddress = DiffgramOperation.GetRelativeAddressOfNodeset(this._firstNode, this._lastNode);
    }

    internal override XmlDiffNodeType NodeType
    {
      get
      {
        return XmlDiffNodeType.ShrankNode;
      }
    }

    internal XmlDiffShrankNode MatchingShrankNode
    {
      get
      {
        return this._matchingShrankNode;
      }
      set
      {
        this._matchingShrankNode = value;
      }
    }

    internal ulong MoveOperationId
    {
      get
      {
        if (this._opid == 0UL)
          this._opid = this.MatchingShrankNode._opid;
        return this._opid;
      }
      set
      {
        this._opid = value;
      }
    }

    internal override bool CanMerge
    {
      get
      {
        return false;
      }
    }

    internal override void ComputeHashValue(XmlHash xmlHash)
    {
    }

    internal override XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff)
    {
      return changedNode.NodeType != XmlDiffNodeType.ShrankNode || (long) this._hashValue != (long) changedNode._hashValue ? XmlDiffOperation.Undefined : XmlDiffOperation.Match;
    }

    internal override void WriteTo(XmlWriter w)
    {
      this.WriteContentTo(w);
    }

    internal override void WriteContentTo(XmlWriter w)
    {
            var xmlDiffNode = this._firstNode;
      while (true)
      {
        xmlDiffNode.WriteTo(w);
        if (xmlDiffNode != this._lastNode)
          xmlDiffNode = xmlDiffNode._nextSibling;
        else
          break;
      }
    }

    internal override string GetRelativeAddress()
    {
      return this._localAddress;
    }
  }
}
