// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffOptions
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
  public enum XmlDiffOptions
  {
    None = 0,
    IgnoreChildOrder = 1,
    IgnoreComments = 2,
    IgnorePI = 4,
    IgnoreWhitespace = 8,
    IgnoreNamespaces = 16, // 0x00000010
    IgnorePrefixes = 32, // 0x00000020
    IgnoreXmlDecl = 64, // 0x00000040
    IgnoreDtd = 128, // 0x00000080
  }
}
