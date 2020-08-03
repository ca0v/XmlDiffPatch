// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffParentNode
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
  internal abstract class XmlDiffParentNode : XmlDiffNode
  {
    private int _childNodesCount = -1;
    internal XmlDiffNode _firstChildNode;
    private int _nodesCount;
    internal bool _elementChildrenOnly;
    internal bool _bDefinesNamespaces;

    internal override bool HasChildNodes
    {
      get
      {
        return this._firstChildNode != null;
      }
    }

    internal override int NodesCount
    {
      get
      {
        return this._nodesCount;
      }
      set
      {
        this._nodesCount = value;
      }
    }

    internal int ChildNodesCount
    {
      get
      {
        if (this._childNodesCount == -1)
        {
          var num = 0;
          for (var xmlDiffNode = this._firstChildNode; xmlDiffNode != null; xmlDiffNode = xmlDiffNode._nextSibling)
            ++num;
          this._childNodesCount = num;
        }
        return this._childNodesCount;
      }
    }

    internal XmlDiffParentNode(int position)
      : base(position)
    {
      this._firstChildNode = (XmlDiffNode) null;
      this._nodesCount = 1;
      this._elementChildrenOnly = true;
      this._bDefinesNamespaces = false;
      this._hashValue = 0UL;
    }

    internal override XmlDiffNode FirstChildNode
    {
      get
      {
        return this._firstChildNode;
      }
    }

    internal virtual void InsertChildNodeAfter(XmlDiffNode childNode, XmlDiffNode newChildNode)
    {
      newChildNode._parent = this;
      if (childNode == null)
      {
        newChildNode._nextSibling = this._firstChildNode;
        this._firstChildNode = newChildNode;
      }
      else
      {
        newChildNode._nextSibling = childNode._nextSibling;
        childNode._nextSibling = newChildNode;
      }
      this._nodesCount += newChildNode.NodesCount;
      if (newChildNode.NodeType == XmlDiffNodeType.Element || newChildNode is XmlDiffAttributeOrNamespace)
        return;
      this._elementChildrenOnly = false;
    }
  }
}
