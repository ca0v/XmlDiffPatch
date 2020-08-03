// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlPatchOperation
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal abstract class XmlPatchOperation
  {
    internal XmlPatchOperation _nextOp;

    internal abstract void Apply(XmlNode parent, ref XmlNode currentPosition);
  }
}
