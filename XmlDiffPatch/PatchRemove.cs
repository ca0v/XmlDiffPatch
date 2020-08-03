// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.PatchRemove
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class PatchRemove : XmlPatchParentOperation
  {
    private XmlNodeList _sourceNodes;
    private bool _bSubtree;

    internal PatchRemove(XmlNodeList sourceNodes, bool bSubtree)
    {
      this._sourceNodes = sourceNodes;
      this._bSubtree = bSubtree;
    }

    internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
    {
      if (!this._bSubtree)
      {
        XmlNode parent1 = this._sourceNodes.Item(0);
        this.ApplyChildren(parent1);
        currentPosition = parent1.PreviousSibling;
      }
      IEnumerator enumerator = this._sourceNodes.GetEnumerator();
      enumerator.Reset();
      while (enumerator.MoveNext())
      {
        XmlNode current = (XmlNode) enumerator.Current;
        if (current.NodeType == XmlNodeType.Attribute)
        {
          ((XmlElement) parent).RemoveAttributeNode((XmlAttribute) current);
        }
        else
        {
          if (!this._bSubtree)
          {
            while (current.FirstChild != null)
            {
              XmlNode firstChild = current.FirstChild;
              current.RemoveChild(firstChild);
              parent.InsertAfter(firstChild, currentPosition);
              currentPosition = firstChild;
            }
          }
          current.ParentNode.RemoveChild(current);
        }
      }
    }
  }
}
