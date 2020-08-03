// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffView
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    public sealed class XmlDiffView
    {
        internal static readonly string[] HtmlBgColor = new string[7]
        {
      "background-color: white",
      "background-color: white",
      "background-color: yellow",
      "background-color: yellow",
      "background-color: red",
      "background-color: red",
      "background-color: lightgreen"
        };
        internal static readonly string[] HtmlFgColor = new string[7]
        {
      "black",
      "#AAAAAA",
      "black",
      "blue",
      "black",
      "blue",
      "black"
        };
        internal static readonly bool[,] HtmlWriteToPane = new bool[7, 2]
        {
      {
        true,
        true
      },
      {
        true,
        true
      },
      {
        false,
        true
      },
      {
        false,
        true
      },
      {
        true,
        false
      },
      {
        true,
        false
      },
      {
        true,
        true
      }
        };
        internal static readonly int DeltaIndent = 15;
        private static readonly string Nbsp = "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
        private XmlDiffViewDocument _doc = null;
        private readonly Hashtable _descriptors = new Hashtable();
        private bool _bIgnoreChildOrder = false;
        private bool _bIgnoreComments = false;
        private bool _bIgnorePI = false;
        private bool _bIgnoreWhitespace = false;
        private bool _bIgnoreNamespaces = false;
        private bool _bIgnorePrefixes = false;
        private readonly bool _bIgnoreXmlDecl = false;
        private bool _bIgnoreDtd = false;
        private LoadState _loadState;

        public void Load(XmlReader sourceXml, XmlReader diffgram)
        {
            var diffgramDoc = new XmlDocument();
            diffgramDoc.Load(diffgram);
            this.PreprocessDiffgram(diffgramDoc);
            this._doc = new XmlDiffViewDocument();
            this.LoadSourceChildNodes(this._doc, sourceXml, false);
            this.ApplyDiffgram(diffgramDoc.DocumentElement, this._doc);
        }

        private void PreprocessDiffgram(XmlDocument diffgramDoc)
        {
            var namedItem = (XmlAttribute)diffgramDoc.DocumentElement.Attributes.GetNamedItem("options");
            if (namedItem == null)
                throw new Exception("Missing 'options' attribute in the diffgram.");
            var options = XmlDiff.ParseOptions(namedItem.Value);
            this._bIgnoreChildOrder = ( options & XmlDiffOptions.IgnoreChildOrder ) > XmlDiffOptions.None;
            this._bIgnoreComments = ( options & XmlDiffOptions.IgnoreComments ) > XmlDiffOptions.None;
            this._bIgnorePI = ( options & XmlDiffOptions.IgnorePI ) > XmlDiffOptions.None;
            this._bIgnoreWhitespace = ( options & XmlDiffOptions.IgnoreWhitespace ) > XmlDiffOptions.None;
            this._bIgnoreNamespaces = ( options & XmlDiffOptions.IgnoreNamespaces ) > XmlDiffOptions.None;
            this._bIgnorePrefixes = ( options & XmlDiffOptions.IgnorePrefixes ) > XmlDiffOptions.None;
            this._bIgnoreDtd = ( options & XmlDiffOptions.IgnoreDtd ) > XmlDiffOptions.None;
            if (this._bIgnoreNamespaces)
                this._bIgnorePrefixes = true;
            foreach (var childNode in diffgramDoc.DocumentElement.ChildNodes)
            {
                var xmlElement = childNode as XmlElement;
                if (xmlElement is XmlElement && xmlElement.LocalName == "descriptor")
                {
                    var opid = int.Parse(xmlElement.GetAttribute("opid"));
                    OperationDescriptor.Type type;
                    switch (xmlElement.GetAttribute("type"))
                    {
                        case "move":
                            type = OperationDescriptor.Type.Move;
                            break;
                        case "prefix change":
                            type = OperationDescriptor.Type.PrefixChange;
                            break;
                        case "namespace change":
                            type = OperationDescriptor.Type.NamespaceChange;
                            break;
                        default:
                            throw new Exception("Invalid descriptor type.");
                    }
                    var operationDescriptor = new OperationDescriptor(opid, type);
                    this._descriptors[opid] = operationDescriptor;
                }
            }
        }

        private void LoadSourceChildNodes(
          XmlDiffViewParentNode parent,
          XmlReader reader,
          bool bEmptyElement)
        {
            var loadState = this._loadState;
            this._loadState.Reset();
            while (reader.MoveToNextAttribute())
            {
                XmlDiffViewAttribute newAttr;
                if (reader.Prefix == "xmlns" || reader.Prefix == string.Empty && reader.LocalName == "xmlns")
                {
                    newAttr = new XmlDiffViewAttribute(reader.LocalName, reader.Prefix, reader.NamespaceURI, reader.Value);
                    if (this._bIgnoreNamespaces)
                        newAttr._op = XmlDiffViewOperation.Ignore;
                }
                else
                {
                    var str = this._bIgnoreWhitespace ? XmlDiffView.NormalizeText(reader.Value) : reader.Value;
                    newAttr = new XmlDiffViewAttribute(reader.LocalName, reader.Prefix, reader.NamespaceURI, str);
                }
              ( (XmlDiffViewElement)parent ).InsertAttributeAfter(newAttr, this._loadState._curLastAttribute);
                this._loadState._curLastAttribute = newAttr;
            }
            if (!bEmptyElement)
            {
                while (reader.Read())
                {
                    if (reader.NodeType != XmlNodeType.Whitespace)
                    {
                        XmlDiffViewNode newChild = null;
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                var isEmptyElement = reader.IsEmptyElement;
                                var xmlDiffViewElement = new XmlDiffViewElement(reader.LocalName, reader.Prefix, reader.NamespaceURI, this._bIgnorePrefixes);
                                this.LoadSourceChildNodes(xmlDiffViewElement, reader, isEmptyElement);
                                newChild = xmlDiffViewElement;
                                break;
                            case XmlNodeType.Attribute:
                                Debug.Assert(false, "We should never get to this point, attributes should be read at the beginning of thid method.");
                                break;
                            case XmlNodeType.Text:
                                newChild = new XmlDiffViewCharData(this._bIgnoreWhitespace ? XmlDiffView.NormalizeText(reader.Value) : reader.Value, XmlNodeType.Text);
                                break;
                            case XmlNodeType.CDATA:
                                newChild = new XmlDiffViewCharData(reader.Value, XmlNodeType.CDATA);
                                break;
                            case XmlNodeType.EntityReference:
                                newChild = new XmlDiffViewER(reader.Name);
                                break;
                            case XmlNodeType.ProcessingInstruction:
                                newChild = new XmlDiffViewPI(reader.Name, reader.Value);
                                if (this._bIgnorePI)
                                {
                                    newChild._op = XmlDiffViewOperation.Ignore;
                                    break;
                                }
                                break;
                            case XmlNodeType.Comment:
                                newChild = new XmlDiffViewCharData(reader.Value, XmlNodeType.Comment);
                                if (this._bIgnoreComments)
                                {
                                    newChild._op = XmlDiffViewOperation.Ignore;
                                    break;
                                }
                                break;
                            case XmlNodeType.DocumentType:
                                newChild = new XmlDiffViewDocumentType(reader.Name, reader.GetAttribute("PUBLIC"), reader.GetAttribute("SYSTEM"), reader.Value);
                                if (this._bIgnoreDtd)
                                {
                                    newChild._op = XmlDiffViewOperation.Ignore;
                                    break;
                                }
                                break;
                            case XmlNodeType.SignificantWhitespace:
                                if (reader.XmlSpace == XmlSpace.Preserve)
                                {
                                    newChild = new XmlDiffViewCharData(reader.Value, XmlNodeType.SignificantWhitespace);
                                    if (this._bIgnoreWhitespace)
                                    {
                                        newChild._op = XmlDiffViewOperation.Ignore;
                                        break;
                                    }
                                    break;
                                }
                                break;
                            case XmlNodeType.EndElement:
                                goto label_29;
                            case XmlNodeType.XmlDeclaration:
                                newChild = new XmlDiffViewXmlDeclaration(XmlDiffView.NormalizeText(reader.Value));
                                if (this._bIgnoreXmlDecl)
                                {
                                    newChild._op = XmlDiffViewOperation.Ignore;
                                    break;
                                }
                                break;
                            default:
                                Debug.Assert(false, "Invalid node type");
                                break;
                        }
                        parent.InsertChildAfter(newChild, this._loadState._curLastChild, true);
                        this._loadState._curLastChild = newChild;
                    }
                }
            }
            label_29:
            this._loadState = loadState;
        }

        private void ApplyDiffgram(XmlNode diffgramParent, XmlDiffViewParentNode sourceParent)
        {
            sourceParent.CreateSourceNodesIndex();
            XmlDiffViewNode currentPosition = null;
            var enumerator = diffgramParent.ChildNodes.GetEnumerator();
            while (enumerator.MoveNext())
            {
                if (( (XmlNode)enumerator.Current ).NodeType != XmlNodeType.Comment)
                {
                    if (!( enumerator.Current is XmlElement ))
                        throw new Exception("Invalid node in diffgram.");
                    var current = enumerator.Current as XmlElement;
                    if (current.NamespaceURI != "http://schemas.microsoft.com/xmltools/2002/xmldiff")
                        throw new Exception("Invalid element in diffgram.");
                    var attribute1 = current.GetAttribute("match");
                    XmlDiffPathNodeList matchNodes = null;
                    if (attribute1 != string.Empty)
                        matchNodes = XmlDiffPath.SelectNodes(this._doc, sourceParent, attribute1);
                    switch (current.LocalName)
                    {
                        case "node":
                            if (matchNodes.Count != 1)
                                throw new Exception("The 'match' attribute of 'node' element must select a single node.");
                            matchNodes.MoveNext();
                            if (current.ChildNodes.Count > 0)
                                this.ApplyDiffgram(current, (XmlDiffViewParentNode)matchNodes.Current);
                            currentPosition = matchNodes.Current;
                            continue;
                        case "add":
                            if (attribute1 != string.Empty)
                            {
                                this.OnAddMatch(current, matchNodes, sourceParent, ref currentPosition);
                                continue;
                            }
                            var attribute2 = current.GetAttribute("type");
                            if (attribute2 != string.Empty)
                            {
                                this.OnAddNode(current, attribute2, sourceParent, ref currentPosition);
                                continue;
                            }
                            this.OnAddFragment(current, sourceParent, ref currentPosition);
                            continue;
                        case "remove":
                            this.OnRemove(current, matchNodes, sourceParent, ref currentPosition);
                            continue;
                        case "change":
                            this.OnChange(current, matchNodes, sourceParent, ref currentPosition);
                            continue;
                        default:
                            continue;
                    }
                }
            }
        }

        private void OnRemove(
          XmlElement diffgramElement,
          XmlDiffPathNodeList matchNodes,
          XmlDiffViewParentNode sourceParent,
          ref XmlDiffViewNode currentPosition)
        {
            var op = XmlDiffViewOperation.Remove;
            var opid = 0;
            OperationDescriptor operationDescriptor = null;
            var attribute = diffgramElement.GetAttribute("opid");
            if (attribute != string.Empty)
            {
                opid = int.Parse(attribute);
                operationDescriptor = this.GetDescriptor(opid);
                if (operationDescriptor._type == OperationDescriptor.Type.Move)
                    op = XmlDiffViewOperation.MoveFrom;
            }
            if (!( diffgramElement.GetAttribute("subtree") != "no" ))
            {
                if (matchNodes.Count != 1)
                    throw new Exception("The 'match' attribute of 'remove' element must select a single node when the 'subtree' attribute is specified.");
                matchNodes.MoveNext();
                var current = matchNodes.Current;
                this.AnnotateNode(current, op, opid, false);
                if (opid != 0)
                    operationDescriptor._nodeList.AddNode(current);
                this.ApplyDiffgram(diffgramElement, (XmlDiffViewParentNode)current);
            }
            else
            {
                matchNodes.Reset();
                while (matchNodes.MoveNext())
                {
                    if (opid != 0)
                        operationDescriptor._nodeList.AddNode(matchNodes.Current);
                    this.AnnotateNode(matchNodes.Current, op, opid, true);
                }
            }
        }

        private void OnAddMatch(
          XmlElement diffgramElement,
          XmlDiffPathNodeList matchNodes,
          XmlDiffViewParentNode sourceParent,
          ref XmlDiffViewNode currentPosition)
        {
            var attribute = diffgramElement.GetAttribute("opid");
            if (attribute == string.Empty)
                throw new Exception("Missing opid attribute.");
            var opid = int.Parse(attribute);
            var descriptor = this.GetDescriptor(opid);
            if (!( diffgramElement.GetAttribute("subtree") != "no" ))
            {
                if (matchNodes.Count != 1)
                    throw new Exception("The 'match' attribute of 'add' element must select a single node when the 'subtree' attribute is specified.");
                matchNodes.MoveNext();
                var xmlDiffViewNode = matchNodes.Current.Clone(false);
                this.AnnotateNode(xmlDiffViewNode, XmlDiffViewOperation.MoveTo, opid, true);
                descriptor._nodeList.AddNode(xmlDiffViewNode);
                sourceParent.InsertChildAfter(xmlDiffViewNode, currentPosition, false);
                currentPosition = xmlDiffViewNode;
                this.ApplyDiffgram(diffgramElement, (XmlDiffViewParentNode)xmlDiffViewNode);
            }
            else
            {
                matchNodes.Reset();
                while (matchNodes.MoveNext())
                {
                    var xmlDiffViewNode = matchNodes.Current.Clone(true);
                    this.AnnotateNode(xmlDiffViewNode, XmlDiffViewOperation.MoveTo, opid, true);
                    descriptor._nodeList.AddNode(xmlDiffViewNode);
                    sourceParent.InsertChildAfter(xmlDiffViewNode, currentPosition, false);
                    currentPosition = xmlDiffViewNode;
                }
            }
        }

        private void OnAddNode(
          XmlElement diffgramElement,
          string nodeTypeAttr,
          XmlDiffViewParentNode sourceParent,
          ref XmlDiffViewNode currentPosition)
        {
            var nodeType = (XmlNodeType)int.Parse(nodeTypeAttr);
            var attribute1 = diffgramElement.GetAttribute("name");
            var attribute2 = diffgramElement.GetAttribute("prefix");
            var attribute3 = diffgramElement.GetAttribute("ns");
            var attribute4 = diffgramElement.GetAttribute("opid");
            var num = attribute4 == string.Empty ? 0 : int.Parse(attribute4);
            if (nodeType == XmlNodeType.Attribute)
            {
                Debug.Assert(attribute1 != string.Empty);
                var newAttr = new XmlDiffViewAttribute(attribute1, attribute2, attribute3, diffgramElement.InnerText);
                newAttr._op = XmlDiffViewOperation.Add;
                newAttr._opid = num;
                ( (XmlDiffViewElement)sourceParent ).InsertAttributeAfter(newAttr, null);
            }
            else
            {
                XmlDiffViewNode newChild = null;
                switch (nodeType)
                {
                    case XmlNodeType.Element:
                        Debug.Assert(attribute1 != string.Empty);
                        newChild = new XmlDiffViewElement(attribute1, attribute2, attribute3, this._bIgnorePrefixes);
                        this.ApplyDiffgram(diffgramElement, (XmlDiffViewParentNode)newChild);
                        break;
                    case XmlNodeType.Text:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Comment:
                        Debug.Assert(diffgramElement.InnerText != string.Empty);
                        newChild = new XmlDiffViewCharData(diffgramElement.InnerText, nodeType);
                        break;
                    case XmlNodeType.EntityReference:
                        Debug.Assert(attribute1 != string.Empty);
                        newChild = new XmlDiffViewER(attribute1);
                        break;
                    case XmlNodeType.ProcessingInstruction:
                        Debug.Assert(diffgramElement.InnerText != string.Empty);
                        Debug.Assert(attribute1 != string.Empty);
                        newChild = new XmlDiffViewPI(attribute1, diffgramElement.InnerText);
                        break;
                    case XmlNodeType.DocumentType:
                        newChild = new XmlDiffViewDocumentType(diffgramElement.GetAttribute("name"), diffgramElement.GetAttribute("publicId"), diffgramElement.GetAttribute("systemId"), diffgramElement.InnerText);
                        break;
                    case XmlNodeType.XmlDeclaration:
                        Debug.Assert(diffgramElement.InnerText != string.Empty);
                        newChild = new XmlDiffViewXmlDeclaration(diffgramElement.InnerText);
                        break;
                    default:
                        Debug.Assert(false, "Invalid node type.");
                        break;
                }
                Debug.Assert(newChild != null);
                newChild._op = XmlDiffViewOperation.Add;
                newChild._opid = num;
                sourceParent.InsertChildAfter(newChild, currentPosition, false);
                currentPosition = newChild;
            }
        }

        private void OnAddFragment(
          XmlElement diffgramElement,
          XmlDiffViewParentNode sourceParent,
          ref XmlDiffViewNode currentPosition)
        {
            foreach (XmlNode childNode in diffgramElement.ChildNodes)
            {
                var xmlDiffViewNode = this.ImportNode(childNode);
                sourceParent.InsertChildAfter(xmlDiffViewNode, currentPosition, false);
                currentPosition = xmlDiffViewNode;
                this.AnnotateNode(xmlDiffViewNode, XmlDiffViewOperation.Add, 0, true);
            }
        }

        private XmlDiffViewNode ImportNode(XmlNode node)
        {
            XmlDiffViewNode xmlDiffViewNode = null;
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    var xmlElement = (XmlElement)node;
                    var xmlDiffViewElement = new XmlDiffViewElement(xmlElement.LocalName, xmlElement.Prefix, xmlElement.NamespaceURI, this._bIgnorePrefixes);
                    var enumerator1 = node.Attributes.GetEnumerator();
                    XmlDiffViewAttribute refAttr = null;
                    while (enumerator1.MoveNext())
                    {
                        var current = (XmlAttribute)enumerator1.Current;
                        var newAttr = new XmlDiffViewAttribute(current.LocalName, current.Prefix, current.NamespaceURI, current.Value);
                        xmlDiffViewElement.InsertAttributeAfter(newAttr, refAttr);
                        refAttr = newAttr;
                    }
                    var enumerator2 = node.ChildNodes.GetEnumerator();
                    XmlDiffViewNode referenceChild = null;
                    while (enumerator2.MoveNext())
                    {
                        var newChild = this.ImportNode((XmlNode)enumerator2.Current);
                        xmlDiffViewElement.InsertChildAfter(newChild, referenceChild, false);
                        referenceChild = newChild;
                    }
                    xmlDiffViewNode = xmlDiffViewElement;
                    break;
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.Comment:
                    xmlDiffViewNode = new XmlDiffViewCharData(node.Value, node.NodeType);
                    break;
                case XmlNodeType.EntityReference:
                    xmlDiffViewNode = new XmlDiffViewER(node.Name);
                    break;
                case XmlNodeType.ProcessingInstruction:
                    xmlDiffViewNode = new XmlDiffViewPI(node.Name, node.Value);
                    break;
                default:
                    Debug.Assert(false, "Invalid node type.");
                    break;
            }
            Debug.Assert(xmlDiffViewNode != null);
            return xmlDiffViewNode;
        }

        private void OnChange(
          XmlElement diffgramElement,
          XmlDiffPathNodeList matchNodes,
          XmlDiffViewParentNode sourceParent,
          ref XmlDiffViewNode currentPosition)
        {
            Debug.Assert(matchNodes.Count == 1);
            matchNodes.Reset();
            matchNodes.MoveNext();
            var current = matchNodes.Current;
            if (current._nodeType != XmlNodeType.Attribute)
                currentPosition = current;
            var changeInfo = new XmlDiffViewNode.ChangeInfo();
            var str1 = diffgramElement.HasAttribute("name") ? diffgramElement.GetAttribute("name") : null;
            var str2 = diffgramElement.HasAttribute("prefix") ? diffgramElement.GetAttribute("prefix") : null;
            var str3 = diffgramElement.HasAttribute("ns") ? diffgramElement.GetAttribute("ns") : null;
            switch (current._nodeType)
            {
                case XmlNodeType.Element:
                    changeInfo._localName = str1 == null ? ( (XmlDiffViewElement)current )._localName : str1;
                    changeInfo._prefix = str2 == null ? ( (XmlDiffViewElement)current )._prefix : str2;
                    changeInfo._ns = str3 == null ? ( (XmlDiffViewElement)current )._ns : str3;
                    break;
                case XmlNodeType.Attribute:
                    var innerText = diffgramElement.InnerText;
                    if (str1 == string.Empty && str2 == string.Empty && innerText == string.Empty)
                        return;
                    changeInfo._localName = str1 == null ? ( (XmlDiffViewAttribute)current )._localName : str1;
                    changeInfo._prefix = str2 == null ? ( (XmlDiffViewAttribute)current )._prefix : str2;
                    changeInfo._ns = str3 == null ? ( (XmlDiffViewAttribute)current )._ns : str3;
                    changeInfo._value = diffgramElement.InnerText;
                    break;
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                    Debug.Assert(diffgramElement.FirstChild != null);
                    changeInfo._value = diffgramElement.InnerText;
                    break;
                case XmlNodeType.EntityReference:
                    Debug.Assert(str1 != null);
                    changeInfo._localName = str1;
                    break;
                case XmlNodeType.ProcessingInstruction:
                    if (str1 == null)
                    {
                        Debug.Assert(diffgramElement.FirstChild != null);
                        Debug.Assert(diffgramElement.FirstChild.NodeType == XmlNodeType.ProcessingInstruction);
                        changeInfo._localName = diffgramElement.FirstChild.Name;
                        changeInfo._value = diffgramElement.FirstChild.Value;
                        break;
                    }
                    changeInfo._localName = str1;
                    changeInfo._value = ( (XmlDiffViewCharData)current )._value;
                    break;
                case XmlNodeType.Comment:
                    Debug.Assert(diffgramElement.FirstChild != null);
                    Debug.Assert(diffgramElement.FirstChild.NodeType == XmlNodeType.Comment);
                    changeInfo._value = diffgramElement.FirstChild.Value;
                    break;
                case XmlNodeType.DocumentType:
                    changeInfo._localName = str1 == null ? ( (XmlDiffViewDocumentType)current )._name : str1;
                    changeInfo._prefix = !diffgramElement.HasAttribute("publicId") ? ( (XmlDiffViewDocumentType)current )._publicId : diffgramElement.GetAttribute("publicId");
                    changeInfo._ns = !diffgramElement.HasAttribute("systemId") ? ( (XmlDiffViewDocumentType)current )._systemId : diffgramElement.GetAttribute("systemId");
                    changeInfo._value = diffgramElement.FirstChild == null ? ( (XmlDiffViewDocumentType)current )._subset : diffgramElement.InnerText;
                    break;
                case XmlNodeType.XmlDeclaration:
                    Debug.Assert(diffgramElement.FirstChild != null);
                    changeInfo._value = diffgramElement.InnerText;
                    break;
                default:
                    Debug.Assert(false, "Invalid node type.");
                    break;
            }
            current._changeInfo = changeInfo;
            current._op = XmlDiffViewOperation.Change;
            var attribute = diffgramElement.GetAttribute("opid");
            if (attribute != string.Empty)
                current._opid = int.Parse(attribute);
            if (current._nodeType != XmlNodeType.Element || diffgramElement.FirstChild == null)
                return;
            this.ApplyDiffgram(diffgramElement, (XmlDiffViewParentNode)current);
        }

        private OperationDescriptor GetDescriptor(int opid)
        {
            var descriptor = (OperationDescriptor)this._descriptors[opid];
            if (descriptor == null)
                throw new Exception("Invalid operation id.");
            return descriptor;
        }

        private void AnnotateNode(
          XmlDiffViewNode node,
          XmlDiffViewOperation op,
          int opid,
          bool bSubtree)
        {
            node._op = op;
            node._opid = opid;
            if (node._nodeType == XmlNodeType.Element)
            {
                for (var diffViewAttribute = ( (XmlDiffViewElement)node )._attributes; diffViewAttribute != null; diffViewAttribute = (XmlDiffViewAttribute)diffViewAttribute._nextSibbling)
                {
                    diffViewAttribute._op = op;
                    diffViewAttribute._opid = opid;
                }
            }
            if (!bSubtree)
                return;
            for (var node1 = node.FirstChildNode; node1 != null; node1 = node1._nextSibbling)
                this.AnnotateNode(node1, op, opid, true);
        }

        public void GetHtml(TextWriter htmlOutput)
        {
            this._doc.DrawHtml(new XmlTextWriter(htmlOutput), 10);
        }

        private static void HtmlSetColor(XmlWriter pane, XmlDiffViewOperation op)
        {
            pane.WriteStartElement("font");
            pane.WriteAttributeString("style", XmlDiffView.HtmlBgColor[(int)op]);
            pane.WriteAttributeString("color", XmlDiffView.HtmlFgColor[(int)op]);
        }

        private static void HtmlResetColor(XmlWriter pane)
        {
            pane.WriteFullEndElement();
        }

        internal static void HtmlWriteString(XmlWriter pane, string str)
        {
            pane.WriteString(str);
        }

        internal static void HtmlWriteString(XmlWriter pane, XmlDiffViewOperation op, string str)
        {
            XmlDiffView.HtmlSetColor(pane, op);
            pane.WriteString(str);
            XmlDiffView.HtmlResetColor(pane);
        }

        internal static void HtmlWriteEmptyString(XmlWriter pane)
        {
            pane.WriteRaw("&nbsp;");
        }

        internal static void HtmlStartCell(XmlWriter writer, int indent)
        {
            writer.WriteStartElement("td");
            writer.WriteAttributeString("style", "padding-left: " + indent.ToString() + "pt;");
        }

        internal static void HtmlEndCell(XmlWriter writer)
        {
            writer.WriteFullEndElement();
        }

        internal static void HtmlBr(XmlWriter writer)
        {
            writer.WriteStartElement("br");
            writer.WriteEndElement();
        }

        internal static void HtmlStartRow(XmlWriter writer)
        {
            writer.WriteStartElement("tr");
        }

        internal static void HtmlEndRow(XmlWriter writer)
        {
            writer.WriteFullEndElement();
        }

        internal static string GetIndent(int charCount)
        {
            var length = charCount * 6;
            if (length <= XmlDiffView.Nbsp.Length)
                return XmlDiffView.Nbsp.Substring(0, length);
            var empty = string.Empty;
            for (; length > XmlDiffView.Nbsp.Length; length -= XmlDiffView.Nbsp.Length)
                empty += XmlDiffView.Nbsp;
            return empty + XmlDiffView.Nbsp.Substring(0, length);
        }

        internal static string NormalizeText(string text)
        {
            var charArray = text.ToCharArray();
            var length = 0;
            var index = 0;
            while (true)
            {
                for (; index >= charArray.Length || !XmlDiffView.IsWhitespace(text[index]); ++index)
                {
                    while (index < charArray.Length && !XmlDiffView.IsWhitespace(text[index]))
                        charArray[length++] = charArray[index++];
                    if (index < charArray.Length)
                    {
                        charArray[length++] = ' ';
                    }
                    else
                    {
                        if (index == 0)
                            return string.Empty;
                        if (XmlDiffView.IsWhitespace(charArray[index - 1]))
                            --length;
                        return new string(charArray, 0, length);
                    }
                }
                ++index;
            }
        }

        internal static bool IsWhitespace(char c)
        {
            return c == ' ' || c == '\t' || c == '\n' || c == '\r';
        }

        private struct LoadState
        {
            public XmlDiffViewNode _curLastChild;
            public XmlDiffViewAttribute _curLastAttribute;

            public void Reset()
            {
                this._curLastChild = null;
                this._curLastAttribute = null;
            }
        }
    }

    internal class Debug
    {
        internal static void Assert(bool v1, string v2)
        {
            throw new NotImplementedException();
        }

        internal static void Assert(bool v)
        {
            throw new NotImplementedException();
        }
    }
}
