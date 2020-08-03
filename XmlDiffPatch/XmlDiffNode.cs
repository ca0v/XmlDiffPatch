// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffNode
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.IO;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal abstract class XmlDiffNode
  {
    internal ulong _hashValue = 0;
    internal bool _bSomeDescendantMatches = false;
    internal XmlDiffParentNode _parent;
    internal XmlDiffNode _nextSibling;
    internal int _position;
    internal bool _bExpanded;
    internal int _leftmostLeafIndex;
    internal bool _bKeyRoot;

    internal XmlDiffNode(int position)
    {
      this._parent = (XmlDiffParentNode) null;
      this._nextSibling = (XmlDiffNode) null;
      this._position = position;
      this._bExpanded = false;
    }

    internal abstract XmlDiffNodeType NodeType { get; }

    internal int Position
    {
      get
      {
        return this._position;
      }
    }

    internal bool IsKeyRoot
    {
      get
      {
        return this._bKeyRoot;
      }
    }

    internal int Left
    {
      get
      {
        return this._leftmostLeafIndex;
      }
      set
      {
        this._leftmostLeafIndex = value;
      }
    }

    internal virtual XmlDiffNode FirstChildNode
    {
      get
      {
        return (XmlDiffNode) null;
      }
    }

    internal virtual bool HasChildNodes
    {
      get
      {
        return false;
      }
    }

    internal virtual int NodesCount
    {
      get
      {
        return 1;
      }
      set
      {
      }
    }

    internal ulong HashValue
    {
      get
      {
        return this._hashValue;
      }
    }

    internal virtual string OuterXml
    {
      get
      {
                var stringWriter = new StringWriter();
                var xmlTextWriter = new XmlTextWriter((TextWriter) stringWriter);
        this.WriteTo((XmlWriter) xmlTextWriter);
        xmlTextWriter.Close();
        return stringWriter.ToString();
      }
    }

    internal virtual string InnerXml
    {
      get
      {
                var stringWriter = new StringWriter();
                var xmlTextWriter = new XmlTextWriter((TextWriter) stringWriter);
        this.WriteContentTo((XmlWriter) xmlTextWriter);
        xmlTextWriter.Close();
        return stringWriter.ToString();
      }
    }

    internal virtual bool CanMerge
    {
      get
      {
        return true;
      }
    }

    internal abstract void ComputeHashValue(XmlHash xmlHash);

    internal abstract XmlDiffOperation GetDiffOperation(
      XmlDiffNode changedNode,
      XmlDiff xmlDiff);

    internal virtual bool IsSameAs(XmlDiffNode node, XmlDiff xmlDiff)
    {
      return this.GetDiffOperation(node, xmlDiff) == XmlDiffOperation.Match;
    }

    internal abstract void WriteTo(XmlWriter w);

    internal abstract void WriteContentTo(XmlWriter w);

    internal virtual string GetRelativeAddress()
    {
      return this.Position.ToString();
    }

    internal string GetAbsoluteAddress()
    {
      var str = this.GetRelativeAddress();
      for (var parent = (XmlDiffNode) this._parent; parent.NodeType != XmlDiffNodeType.Document; parent = (XmlDiffNode) parent._parent)
        str = parent.GetRelativeAddress() + "/" + str;
      return "/" + str;
    }

    internal static string GetRelativeAddressOfInterval(XmlDiffNode firstNode, XmlDiffNode lastNode)
    {
      if (firstNode == lastNode)
        return firstNode.GetRelativeAddress();
      return firstNode._parent._firstChildNode == firstNode && lastNode._nextSibling == null ? "*" : firstNode.Position.ToString() + "-" + lastNode.Position.ToString();
    }
  }
}
