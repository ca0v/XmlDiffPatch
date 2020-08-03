// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffOperation
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
  internal enum XmlDiffOperation
  {
    Match,
    Add,
    Remove,
    ChangeElementName,
    ChangeElementAttr1,
    ChangeElementAttr2,
    ChangeElementAttr3,
    ChangeElementNameAndAttr1,
    ChangeElementNameAndAttr2,
    ChangeElementNameAndAttr3,
    ChangePI,
    ChangeER,
    ChangeCharacterData,
    ChangeXmlDeclaration,
    ChangeDTD,
    Undefined,
    ChangeAttr,
  }
}
