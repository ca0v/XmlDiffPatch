// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewCharData
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System.Diagnostics;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffViewCharData : XmlDiffViewNode
  {
    internal string _value;

    internal XmlDiffViewCharData(string value, XmlNodeType nodeType)
      : base(nodeType)
    {
      this._value = value;
    }

    internal override string OuterXml
    {
      get
      {
        switch (this._nodeType)
        {
          case XmlNodeType.Text:
          case XmlNodeType.Whitespace:
            return this._value;
          case XmlNodeType.CDATA:
            return "<!CDATA[" + this._value + "]]>";
          case XmlNodeType.Comment:
            return "<!--" + this._value + "-->";
          default:
            Debug.Assert(false, "Invalid node type.");
            return string.Empty;
        }
      }
    }

    internal override XmlDiffViewNode Clone(bool bDeep)
    {
      return (XmlDiffViewNode) new XmlDiffViewCharData(this._value, this._nodeType);
    }

    internal override void DrawHtml(XmlWriter writer, int indent)
    {
      if (this._op == XmlDiffViewOperation.Change)
      {
        var str1 = string.Empty;
        var str2 = string.Empty;
        if (this._nodeType == XmlNodeType.CDATA)
        {
          str1 = "<!CDATA[";
          str2 = "]]>";
        }
        else if (this._nodeType == XmlNodeType.Comment)
        {
          str1 = "<!--";
          str2 = "-->";
        }
        XmlDiffView.HtmlStartRow(writer);
        XmlDiffView.HtmlStartCell(writer, indent);
        if (str1 != string.Empty)
        {
          XmlDiffView.HtmlWriteString(writer, str1);
          XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, this._value);
          XmlDiffView.HtmlWriteString(writer, str2);
        }
        else
          XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, this._value);
        XmlDiffView.HtmlEndCell(writer);
        XmlDiffView.HtmlStartCell(writer, indent);
        if (str1 != string.Empty)
        {
          XmlDiffView.HtmlWriteString(writer, str1);
          XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, this._changeInfo._value);
          XmlDiffView.HtmlWriteString(writer, str2);
        }
        else
          XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, this._changeInfo._value);
        XmlDiffView.HtmlEndCell(writer);
        XmlDiffView.HtmlEndRow(writer);
      }
      else
        this.DrawHtmlNoChange(writer, indent);
    }
  }
}
