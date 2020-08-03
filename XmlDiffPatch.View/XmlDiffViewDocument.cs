// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewDocument
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffViewDocument : XmlDiffViewParentNode
  {
    internal XmlDiffViewDocument()
      : base(XmlNodeType.Document)
    {
    }

    internal override string OuterXml
    {
      get
      {
        throw new Exception("OuterXml is not supported on XmlDiffViewElement.");
      }
    }

    internal override XmlDiffViewNode Clone(bool bDeep)
    {
      throw new Exception("Clone method should never be called on a document node.");
    }

    internal override void DrawHtml(XmlWriter writer, int indent)
    {
      this.HtmlDrawChildNodes(writer, indent);
    }
  }
}
