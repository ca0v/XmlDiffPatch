// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.PatchAddXmlFragment
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class PatchAddXmlFragment : XmlPatchOperation
  {
    private XmlNodeList _nodes;

    internal PatchAddXmlFragment(XmlNodeList nodes)
    {
      this._nodes = nodes;
    }

    internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
    {
      XmlDocument ownerDocument = parent.OwnerDocument;
      foreach (XmlNode node in this._nodes)
      {
        XmlNode newChild = ownerDocument.ImportNode(node, true);
        parent.InsertAfter(newChild, currentPosition);
        currentPosition = newChild;
      }
    }
  }
}
