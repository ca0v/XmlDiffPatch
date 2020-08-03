// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffViewElement
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System;
using System.Diagnostics;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffViewElement : XmlDiffViewParentNode
  {
    internal string _localName;
    internal string _prefix;
    internal string _ns;
    internal string _name;
    internal XmlDiffViewAttribute _attributes;
    private bool _ignorePrefixes;

    internal XmlDiffViewElement(string localName, string prefix, string ns, bool ignorePrefixes)
      : base(XmlNodeType.Element)
    {
      this._localName = localName;
      this._prefix = prefix;
      this._ns = ns;
      this._name = !(this._prefix != string.Empty) ? this._localName : this._prefix + ":" + this._localName;
      this._ignorePrefixes = ignorePrefixes;
    }

    internal XmlDiffViewAttribute GetAttribute(string name)
    {
      for (XmlDiffViewAttribute diffViewAttribute = this._attributes; diffViewAttribute != null; diffViewAttribute = (XmlDiffViewAttribute) diffViewAttribute._nextSibbling)
      {
        if (diffViewAttribute._name == name && diffViewAttribute._op == XmlDiffViewOperation.Match)
          return diffViewAttribute;
      }
      return (XmlDiffViewAttribute) null;
    }

    internal void InsertAttributeAfter(XmlDiffViewAttribute newAttr, XmlDiffViewAttribute refAttr)
    {
      Debug.Assert(newAttr != null);
      if (refAttr == null)
      {
        newAttr._nextSibbling = (XmlDiffViewNode) this._attributes;
        this._attributes = newAttr;
      }
      else
      {
        newAttr._nextSibbling = refAttr._nextSibbling;
        refAttr._nextSibbling = (XmlDiffViewNode) newAttr;
      }
      newAttr._parent = (XmlDiffViewNode) this;
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
      XmlDiffViewElement xmlDiffViewElement = new XmlDiffViewElement(this._localName, this._prefix, this._ns, this._ignorePrefixes);
      XmlDiffViewAttribute diffViewAttribute = this._attributes;
      XmlDiffViewAttribute refAttr = (XmlDiffViewAttribute) null;
      for (; diffViewAttribute != null; diffViewAttribute = (XmlDiffViewAttribute) diffViewAttribute._nextSibbling)
      {
        XmlDiffViewAttribute newAttr = (XmlDiffViewAttribute) diffViewAttribute.Clone(true);
        xmlDiffViewElement.InsertAttributeAfter(newAttr, refAttr);
        refAttr = newAttr;
      }
      if (!bDeep)
        return (XmlDiffViewNode) xmlDiffViewElement;
      XmlDiffViewNode xmlDiffViewNode = this._childNodes;
      XmlDiffViewNode referenceChild = (XmlDiffViewNode) null;
      for (; xmlDiffViewNode != null; xmlDiffViewNode = xmlDiffViewNode._nextSibbling)
      {
        XmlDiffViewNode newChild = xmlDiffViewNode.Clone(true);
        xmlDiffViewElement.InsertChildAfter(newChild, referenceChild, false);
        referenceChild = newChild;
      }
      return (XmlDiffViewNode) xmlDiffViewElement;
    }

    internal override void DrawHtml(XmlWriter writer, int indent)
    {
      XmlDiffViewOperation diffViewOperation = this._op;
      bool flag1 = false;
      XmlDiffView.HtmlStartRow(writer);
      for (int paneNo = 0; paneNo < 2; ++paneNo)
      {
        XmlDiffView.HtmlStartCell(writer, indent);
        if (XmlDiffView.HtmlWriteToPane[(int) this._op, paneNo])
        {
          bool flag2 = this.OutputNavigation(writer);
          if (this._op == XmlDiffViewOperation.Change)
          {
            diffViewOperation = XmlDiffViewOperation.Match;
            XmlDiffView.HtmlWriteString(writer, diffViewOperation, "<");
            if (paneNo == 0)
              this.DrawHtmlNameChange(writer, this._localName, this._prefix);
            else
              this.DrawHtmlNameChange(writer, this._changeInfo._localName, this._changeInfo._prefix);
          }
          else
            this.DrawHtmlName(writer, diffViewOperation, "<", string.Empty);
          if (flag2)
          {
            writer.WriteEndElement();
            flag1 = false;
          }
          this.DrawHtmlAttributes(writer, paneNo);
          if (this._childNodes != null)
            XmlDiffView.HtmlWriteString(writer, diffViewOperation, ">");
          else
            XmlDiffView.HtmlWriteString(writer, diffViewOperation, "/>");
        }
        else
          XmlDiffView.HtmlWriteEmptyString(writer);
        XmlDiffView.HtmlEndCell(writer);
      }
      XmlDiffView.HtmlEndRow(writer);
      if (this._childNodes == null)
        return;
      this.HtmlDrawChildNodes(writer, indent + XmlDiffView.DeltaIndent);
      XmlDiffView.HtmlStartRow(writer);
      for (int index = 0; index < 2; ++index)
      {
        XmlDiffView.HtmlStartCell(writer, indent);
        if (XmlDiffView.HtmlWriteToPane[(int) this._op, index])
        {
          if (this._op == XmlDiffViewOperation.Change)
          {
            Debug.Assert(diffViewOperation == XmlDiffViewOperation.Match);
            XmlDiffView.HtmlWriteString(writer, diffViewOperation, "</");
            if (index == 0)
              this.DrawHtmlNameChange(writer, this._localName, this._prefix);
            else
              this.DrawHtmlNameChange(writer, this._changeInfo._localName, this._changeInfo._prefix);
            XmlDiffView.HtmlWriteString(writer, diffViewOperation, ">");
          }
          else
            this.DrawHtmlName(writer, diffViewOperation, "</", ">");
        }
        else
          XmlDiffView.HtmlWriteEmptyString(writer);
        XmlDiffView.HtmlEndCell(writer);
      }
      XmlDiffView.HtmlEndRow(writer);
    }

    private void DrawHtmlAttributes(XmlWriter writer, int paneNo)
    {
      if (this._attributes == null)
        return;
      string data = string.Empty;
      if (this._attributes._nextSibbling != null)
        data = XmlDiffView.GetIndent(this._name.Length + 2);
      XmlDiffViewAttribute attr = this._attributes;
      while (attr != null)
      {
        if (XmlDiffView.HtmlWriteToPane[(int) attr._op, paneNo])
        {
          if (attr == this._attributes)
            writer.WriteString(" ");
          else
            writer.WriteRaw(data);
          if (attr._op == XmlDiffViewOperation.Change)
          {
            if (paneNo == 0)
              this.DrawHtmlAttributeChange(writer, attr, attr._localName, attr._prefix, attr._value);
            else
              this.DrawHtmlAttributeChange(writer, attr, attr._changeInfo._localName, attr._changeInfo._prefix, attr._changeInfo._value);
          }
          else
            this.DrawHtmlAttribute(writer, attr, attr._op);
        }
        else
          XmlDiffView.HtmlWriteEmptyString(writer);
        attr = (XmlDiffViewAttribute) attr._nextSibbling;
        if (attr != null)
          XmlDiffView.HtmlBr(writer);
      }
    }

    private void DrawHtmlNameChange(XmlWriter writer, string localName, string prefix)
    {
      if (prefix != string.Empty)
        XmlDiffView.HtmlWriteString(writer, this._ignorePrefixes ? XmlDiffViewOperation.Ignore : (this._prefix == this._changeInfo._prefix ? XmlDiffViewOperation.Match : XmlDiffViewOperation.Change), prefix + ":");
      XmlDiffView.HtmlWriteString(writer, this._localName == this._changeInfo._localName ? XmlDiffViewOperation.Match : XmlDiffViewOperation.Change, localName);
    }

    private void DrawHtmlName(
      XmlWriter writer,
      XmlDiffViewOperation opForColor,
      string tagStart,
      string tagEnd)
    {
      if (this._prefix != string.Empty && this._ignorePrefixes)
      {
        XmlDiffView.HtmlWriteString(writer, opForColor, tagStart);
        XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Ignore, this._prefix + ":");
        XmlDiffView.HtmlWriteString(writer, opForColor, this._localName + tagEnd);
      }
      else
        XmlDiffView.HtmlWriteString(writer, opForColor, tagStart + this._name + tagEnd);
    }

    private void DrawHtmlAttributeChange(
      XmlWriter writer,
      XmlDiffViewAttribute attr,
      string localName,
      string prefix,
      string value)
    {
      if (prefix != string.Empty)
        XmlDiffView.HtmlWriteString(writer, this._ignorePrefixes ? XmlDiffViewOperation.Ignore : (attr._prefix == attr._changeInfo._prefix ? XmlDiffViewOperation.Match : XmlDiffViewOperation.Change), prefix + ":");
      XmlDiffView.HtmlWriteString(writer, attr._localName == attr._changeInfo._localName ? XmlDiffViewOperation.Match : XmlDiffViewOperation.Change, localName);
      if (attr._value != attr._changeInfo._value)
      {
        XmlDiffView.HtmlWriteString(writer, "=\"");
        XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Change, value);
        XmlDiffView.HtmlWriteString(writer, "\"");
      }
      else
        XmlDiffView.HtmlWriteString(writer, "=\"" + value + "\"");
    }

    private void DrawHtmlAttribute(
      XmlWriter writer,
      XmlDiffViewAttribute attr,
      XmlDiffViewOperation opForColor)
    {
      if (this._ignorePrefixes)
      {
        if (attr._prefix == "xmlns" || attr._localName == "xmlns" && attr._prefix == string.Empty)
        {
          XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Ignore, attr._name);
          XmlDiffView.HtmlWriteString(writer, opForColor, "=\"" + attr._value + "\"");
          return;
        }
        if (attr._prefix != string.Empty)
        {
          XmlDiffView.HtmlWriteString(writer, XmlDiffViewOperation.Ignore, attr._prefix + ":");
          XmlDiffView.HtmlWriteString(writer, opForColor, attr._localName + "=\"" + attr._value + "\"");
          return;
        }
      }
      XmlDiffView.HtmlWriteString(writer, opForColor, attr._name + "=\"" + attr._value + "\"");
    }
  }
}
