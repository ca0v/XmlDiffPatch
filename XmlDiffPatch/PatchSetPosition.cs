// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.PatchSetPosition
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class PatchSetPosition : XmlPatchParentOperation
  {
    private XmlNode _matchNode;

    internal PatchSetPosition(XmlNode matchNode)
    {
      this._matchNode = matchNode;
    }

    internal override void Apply(XmlNode parent, ref XmlNode currentPosition)
    {
      if (this._matchNode.NodeType == XmlNodeType.Element)
        this.ApplyChildren(this._matchNode);
      currentPosition = this._matchNode;
    }
  }
}
