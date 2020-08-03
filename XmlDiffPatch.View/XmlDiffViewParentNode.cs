// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewParentNode
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal abstract class XmlDiffViewParentNode : XmlDiffViewNode
  {
    internal XmlDiffViewNode _childNodes;
    internal int _sourceChildNodesCount;
    private XmlDiffViewNode[] _sourceChildNodesIndex;

    internal XmlDiffViewParentNode(XmlNodeType nodeType)
      : base(nodeType)
    {
    }

    internal override XmlDiffViewNode FirstChildNode
    {
      get
      {
        return this._childNodes;
      }
    }

    internal XmlDiffViewNode GetSourceChildNode(int index)
    {
      if (index < 0 || index >= this._sourceChildNodesCount || this._sourceChildNodesCount == 0)
        throw new ArgumentException(nameof (index));
      if (this._sourceChildNodesCount == 0)
        return (XmlDiffViewNode) null;
      if (this._sourceChildNodesIndex == null)
        this.CreateSourceNodesIndex();
      return this._sourceChildNodesIndex[index];
    }

    internal void CreateSourceNodesIndex()
    {
      if (this._sourceChildNodesIndex != null || this._sourceChildNodesCount == 0)
        return;
      this._sourceChildNodesIndex = new XmlDiffViewNode[this._sourceChildNodesCount];
            var xmlDiffViewNode = this._childNodes;
      var index = 0;
      while (index < this._sourceChildNodesCount)
      {
        Debug.Assert(xmlDiffViewNode != null);
        this._sourceChildNodesIndex[index] = xmlDiffViewNode;
        ++index;
        xmlDiffViewNode = xmlDiffViewNode._nextSibbling;
      }
      Debug.Assert(xmlDiffViewNode == null);
    }

    internal void InsertChildAfter(
      XmlDiffViewNode newChild,
      XmlDiffViewNode referenceChild,
      bool bSourceNode)
    {
      Debug.Assert(newChild != null);
      if (referenceChild == null)
      {
        newChild._nextSibbling = this._childNodes;
        this._childNodes = newChild;
      }
      else
      {
        newChild._nextSibbling = referenceChild._nextSibbling;
        referenceChild._nextSibbling = newChild;
      }
      if (bSourceNode)
        ++this._sourceChildNodesCount;
      newChild._parent = (XmlDiffViewNode) this;
    }

    internal void HtmlDrawChildNodes(XmlWriter writer, int indent)
    {
      for (var xmlDiffViewNode = this._childNodes; xmlDiffViewNode != null; xmlDiffViewNode = xmlDiffViewNode._nextSibbling)
        xmlDiffViewNode.DrawHtml(writer, indent);
    }
  }
}
