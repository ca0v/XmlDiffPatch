// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewDocumentType
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System.Diagnostics;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffViewDocumentType : XmlDiffViewNode
  {
    internal string _name;
    internal string _systemId;
    internal string _publicId;
    internal string _subset;

    internal XmlDiffViewDocumentType(string name, string publicId, string systemId, string subset)
      : base(XmlNodeType.DocumentType)
    {
      this._name = name;
      this._publicId = publicId == null ? string.Empty : publicId;
      this._systemId = systemId == null ? string.Empty : systemId;
      this._subset = subset;
    }

    internal override string OuterXml
    {
      get
      {
        string str = "<!DOCTYPE " + this._name + " ";
        if (this._publicId != string.Empty)
          str = str + "PUBLIC \"" + this._publicId + "\" ";
        else if (this._systemId != string.Empty)
          str = str + "SYSTEM \"" + this._systemId + "\" ";
        if (this._subset != string.Empty)
          str = str + "[" + this._subset + "]";
        return str + ">";
      }
    }

    internal override XmlDiffViewNode Clone(bool bDeep)
    {
      Debug.Assert(false, "Clone method should never be called on document type node.");
      return (XmlDiffViewNode) null;
    }

    internal override void DrawHtml(XmlWriter writer, int indent)
    {
      if (this._op == XmlDiffViewOperation.Change)
      {
        XmlDiffView.HtmlStartRow(writer);
        for (int index = 0; index < 2; ++index)
        {
          XmlDiffView.HtmlStartCell(writer, indent);
          XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Match, "<!DOCTYPE ");
          if (index == 0)
            XmlDiffView.HtmlWriteString(writer, this._name == this._changeInfo._localName ? XmlDiffViewOperation.Match : XmlDiffViewOperation.Change, this._name);
          else
            XmlDiffView.HtmlWriteString(writer, this._name == this._changeInfo._localName ? XmlDiffViewOperation.Match : XmlDiffViewOperation.Change, this._changeInfo._localName);
          XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Match, " ");
          string str = "SYSTEM ";
          if (this._publicId == this._changeInfo._prefix)
          {
            if (this._publicId != string.Empty)
            {
              XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Match, "PUBLIC \"" + this._publicId + "\" ");
              str = string.Empty;
            }
          }
          else if (this._publicId == string.Empty)
          {
            if (index == 1)
            {
              XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Add, "PUBLIC \"" + this._changeInfo._prefix + "\" ");
              str = string.Empty;
            }
          }
          else if (this._changeInfo._prefix == string.Empty)
          {
            if (index == 0)
            {
              XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Remove, "PUBLIC \"" + this._publicId + "\" ");
              str = string.Empty;
            }
          }
          else
          {
            XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, "PUBLIC \"" + (index == 0 ? this._publicId : this._changeInfo._prefix) + "\" ");
            str = string.Empty;
          }
          if (this._systemId == this._changeInfo._ns)
          {
            if (this._systemId != string.Empty)
              XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Match, str + "\"" + this._systemId + "\" ");
          }
          else if (this._systemId == string.Empty)
          {
            if (index == 1)
              XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Add, str + "\"" + this._changeInfo._ns + "\" ");
          }
          else if (this._changeInfo._prefix == string.Empty)
          {
            if (index == 0)
              XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Remove, str + "\"" + this._systemId + "\" ");
          }
          else
            XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, str + "\"" + (index == 0 ? this._systemId : this._changeInfo._ns) + "\" ");
          if (this._subset == this._changeInfo._value)
          {
            if (this._subset != string.Empty)
              XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Match, "[" + this._subset + "]");
          }
          else if (this._subset == string.Empty)
          {
            if (index == 1)
              XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Add, "[" + this._changeInfo._value + "]");
          }
          else if (this._changeInfo._value == string.Empty)
          {
            if (index == 0)
              XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Remove, "[" + this._subset + "]");
          }
          else
            XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, "[" + (index == 0 ? this._subset : this._changeInfo._value) + "]");
          XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Match, ">");
          XmlDiffView.HtmlEndCell(writer);
        }
        XmlDiffView.HtmlEndRow(writer);
      }
      else
        this.DrawHtmlNoChange(writer, indent);
    }
  }
}
