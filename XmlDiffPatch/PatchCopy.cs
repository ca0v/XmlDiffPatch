// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.PatchCopy
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class PatchCopy : XmlPatchParentOperation
  {
    private XmlNodeList _matchNodes;
    private bool _bSubtree;

    internal PatchCopy(XmlNodeList matchNodes, bool bSubtree)
    {
      this._matchNodes = matchNodes;
      this._bSubtree = bSubtree;
    }

    internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
    {
            var enumerator = this._matchNodes.GetEnumerator();
      enumerator.Reset();
      while (enumerator.MoveNext())
      {
                var current = (XmlNode) enumerator.Current;
        XmlNode xmlNode;
        if (this._bSubtree)
        {
          xmlNode = current.Clone();
        }
        else
        {
          xmlNode = current.CloneNode(false);
          this.ApplyChildren(xmlNode);
        }
        parent.InsertAfter(xmlNode, currentPosition);
        currentPosition = xmlNode;
      }
    }
  }
}
