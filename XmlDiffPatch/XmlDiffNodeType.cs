// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffNodeType
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
  internal enum XmlDiffNodeType
  {
    XmlDeclaration = -2, // 0xFFFFFFFE
    DocumentType = -1, // 0xFFFFFFFF
    None = 0,
    Element = 1,
    Attribute = 2,
    Text = 3,
    CDATA = 4,
    EntityReference = 5,
    ProcessingInstruction = 7,
    Comment = 8,
    Document = 9,
    SignificantWhitespace = 14, // 0x0000000E
    Namespace = 100, // 0x00000064
    ShrankNode = 101, // 0x00000065
  }
}
