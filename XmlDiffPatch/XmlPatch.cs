// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlPatch
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System;
using System.IO;
using System.Text;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    public class XmlPatch
    {
        private XmlNode _sourceRootNode;
        private bool _ignoreChildOrder;

        public void Patch(XmlDocument sourceDoc, XmlReader diffgram)
        {
            if (sourceDoc == null)
                throw new ArgumentNullException(nameof(sourceDoc));
            if (diffgram == null)
                throw new ArgumentNullException(nameof(diffgram));
            XmlNode sourceNode = (XmlNode)sourceDoc;
            this.Patch(ref sourceNode, diffgram);
        }

        public void Patch(string sourceFile, Stream outputStream, XmlReader diffgram)
        {
            if (sourceFile == null)
                throw new ArgumentNullException(nameof(sourceFile));
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));
            if (diffgram == null)
                throw new ArgumentException(nameof(diffgram));
            XmlDocument diffDoc = new XmlDocument();
            diffDoc.Load(diffgram);
            if (diffDoc.DocumentElement.GetAttribute("fragments") == "yes")
            {
                NameTable nameTable = new NameTable();
                this.Patch((XmlReader)new XmlTextReader((Stream)new FileStream(sourceFile, FileMode.Open, FileAccess.Read), XmlNodeType.Element, new XmlParserContext((XmlNameTable)nameTable, new XmlNamespaceManager((XmlNameTable)nameTable), string.Empty, XmlSpace.Default)), outputStream, diffDoc);
            }
            else
                this.Patch((XmlReader)new XmlTextReader(sourceFile), outputStream, diffDoc);
        }

        public void Patch(XmlReader sourceReader, Stream outputStream, XmlReader diffgram)
        {
            if (sourceReader == null)
                throw new ArgumentNullException(nameof(sourceReader));
            if (outputStream == null)
                throw new ArgumentNullException(nameof(outputStream));
            if (diffgram == null)
                throw new ArgumentException(nameof(diffgram));
            XmlDocument diffDoc = new XmlDocument();
            diffDoc.Load(diffgram);
            this.Patch(sourceReader, outputStream, diffDoc);
        }

        private void Patch(XmlReader sourceReader, Stream outputStream, XmlDocument diffDoc)
        {
            bool flag = diffDoc.DocumentElement.GetAttribute("fragments") == "yes";
            Encoding encoding = (Encoding)null;
            if (flag)
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlDocumentFragment documentFragment = xmlDocument.CreateDocumentFragment();
                XmlNode newChild;
                while ((newChild = xmlDocument.ReadNode(sourceReader)) != null)
                {
                    switch (newChild.NodeType)
                    {
                        case XmlNodeType.Whitespace:
                            if (encoding == null)
                            {
                                switch (sourceReader)
                                {
                                    case XmlTextReader _:
                                        encoding = ((XmlTextReader)sourceReader).Encoding;
                                        continue;
                                    case XmlValidatingReader _:
                                        encoding = ((XmlValidatingReader)sourceReader).Encoding;
                                        continue;
                                    default:
                                        encoding = Encoding.Unicode;
                                        continue;
                                }
                            }
                            else
                                continue;
                        case XmlNodeType.XmlDeclaration:
                            documentFragment.InnerXml = newChild.OuterXml;
                            goto case XmlNodeType.Whitespace;
                        default:
                            documentFragment.AppendChild(newChild);
                            goto case XmlNodeType.Whitespace;
                    }
                }
                XmlNode sourceNode = (XmlNode)documentFragment;
                this.Patch(ref sourceNode, diffDoc);
                if (documentFragment.FirstChild != null && documentFragment.FirstChild.NodeType == XmlNodeType.XmlDeclaration)
                    encoding = Encoding.GetEncoding(((XmlDeclaration)sourceNode.FirstChild).Encoding);
                XmlTextWriter xmlTextWriter = new XmlTextWriter(outputStream, encoding);
                documentFragment.WriteTo((XmlWriter)xmlTextWriter);
                xmlTextWriter.Flush();
            }
            else
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.XmlResolver = (XmlResolver)null;
                xmlDocument.Load(sourceReader);
                XmlNode sourceNode = (XmlNode)xmlDocument;
                this.Patch(ref sourceNode, diffDoc);
                xmlDocument.Save(outputStream);
            }
        }

        public void Patch(ref XmlNode sourceNode, XmlReader diffgram)
        {
            if (sourceNode == null)
                throw new ArgumentNullException(nameof(sourceNode));
            if (diffgram == null)
                throw new ArgumentNullException(nameof(diffgram));
            XmlDocument diffDoc = new XmlDocument();
            diffDoc.Load(diffgram);
            this.Patch(ref sourceNode, diffDoc);
        }

        private void Patch(ref XmlNode sourceNode, XmlDocument diffDoc)
        {
            XmlElement documentElement = diffDoc.DocumentElement;
            if (documentElement.LocalName != "xmldiff" || documentElement.NamespaceURI != "http://schemas.microsoft.com/xmltools/2002/xmldiff")
                XmlPatchError.Error("Invalid XDL diffgram. Expecting xd:xmldiff as a root element with namespace URI 'http://schemas.microsoft.com/xmltools/2002/xmldiff'.");
            XmlNamedNodeMap attributes = (XmlNamedNodeMap)documentElement.Attributes;
            XmlAttribute namedItem1 = (XmlAttribute)attributes.GetNamedItem("srcDocHash");
            if (namedItem1 == null)
                XmlPatchError.Error("Invalid XDL diffgram. Missing srcDocHash attribute on the xd:xmldiff element.");
            ulong hashValue = 0;
            try
            {
                hashValue = ulong.Parse(namedItem1.Value);
            }
            catch
            {
                XmlPatchError.Error("Invalid XDL diffgram. The srcDocHash attribute has an invalid value.");
            }
            XmlAttribute namedItem2 = (XmlAttribute)attributes.GetNamedItem("options");
            if (namedItem2 == null)
                XmlPatchError.Error("Invalid XDL diffgram. Missing options attribute on the xd:xmldiff element.");
            XmlDiffOptions options = XmlDiffOptions.None;
            try
            {
                options = XmlDiff.ParseOptions(namedItem2.Value);
            }
            catch
            {
                XmlPatchError.Error("Invalid XDL diffgram. The options attribute has an invalid value.");
            }
            this._ignoreChildOrder = (options & XmlDiffOptions.IgnoreChildOrder) != XmlDiffOptions.None;
            if (!XmlDiff.VerifySource(sourceNode, hashValue, options))
                XmlPatchError.Error("The XDL diffgram is not applicable to this XML document; the srcDocHash value does not match.");
            if (sourceNode.NodeType == XmlNodeType.Document)
            {
                Microsoft.XmlDiffPatch.Patch patch = this.CreatePatch(sourceNode, documentElement);
                XmlDocument xmlDocument = (XmlDocument)sourceNode;
                XmlElement element = xmlDocument.CreateElement("tempRoot");
                XmlNode nextSibling;
                for (XmlNode xmlNode = xmlDocument.FirstChild; xmlNode != null; xmlNode = nextSibling)
                {
                    nextSibling = xmlNode.NextSibling;
                    if (xmlNode.NodeType != XmlNodeType.XmlDeclaration && xmlNode.NodeType != XmlNodeType.DocumentType)
                    {
                        xmlDocument.RemoveChild(xmlNode);
                        element.AppendChild(xmlNode);
                    }
                }
                xmlDocument.AppendChild((XmlNode)element);
                XmlNode currentPosition = (XmlNode)null;
                patch.Apply((XmlNode)element, ref currentPosition);
                if (sourceNode.NodeType != XmlNodeType.Document)
                    return;
                xmlDocument.RemoveChild((XmlNode)element);
                XmlNode firstChild;
                while ((firstChild = element.FirstChild) != null)
                {
                    element.RemoveChild(firstChild);
                    xmlDocument.AppendChild(firstChild);
                }
            }
            else if (sourceNode.NodeType == XmlNodeType.DocumentFragment)
            {
                Microsoft.XmlDiffPatch.Patch patch = this.CreatePatch(sourceNode, documentElement);
                XmlNode currentPosition = (XmlNode)null;
                patch.Apply(sourceNode, ref currentPosition);
            }
            else
            {
                XmlDocumentFragment documentFragment = sourceNode.OwnerDocument.CreateDocumentFragment();
                XmlNode parentNode = sourceNode.ParentNode;
                XmlNode previousSibling = sourceNode.PreviousSibling;
                parentNode?.RemoveChild(sourceNode);
                if (sourceNode.NodeType != XmlNodeType.XmlDeclaration)
                    documentFragment.AppendChild(sourceNode);
                else
                    documentFragment.InnerXml = sourceNode.OuterXml;
                Microsoft.XmlDiffPatch.Patch patch = this.CreatePatch((XmlNode)documentFragment, documentElement);
                XmlNode currentPosition = (XmlNode)null;
                patch.Apply((XmlNode)documentFragment, ref currentPosition);
                XmlNodeList childNodes = documentFragment.ChildNodes;
                if (childNodes.Count != 1)
                    XmlPatchError.Error("Internal Error. {0} nodes left after patch, expecting 1.", childNodes.Count.ToString());
                sourceNode = childNodes.Item(0);
                documentFragment.RemoveAll();
                parentNode?.InsertAfter(sourceNode, previousSibling);
            }
        }

        private Microsoft.XmlDiffPatch.Patch CreatePatch(
          XmlNode sourceNode,
          XmlElement diffgramElement)
        {
            Microsoft.XmlDiffPatch.Patch patch = new Microsoft.XmlDiffPatch.Patch(sourceNode);
            this._sourceRootNode = sourceNode;
            this.CreatePatchForChildren(sourceNode, diffgramElement, (XmlPatchParentOperation)patch);
            return patch;
        }

        private void CreatePatchForChildren(
          XmlNode sourceParent,
          XmlElement diffgramParent,
          XmlPatchParentOperation patchParent)
        {
            XmlPatchOperation child = (XmlPatchOperation)null;
            XmlNode xmlNode1 = diffgramParent.FirstChild;
            while (xmlNode1 != null)
            {
                if (xmlNode1.NodeType != XmlNodeType.Element)
                {
                    xmlNode1 = xmlNode1.NextSibling;
                }
                else
                {
                    XmlElement diffgramParent1 = (XmlElement)xmlNode1;
                    XmlNodeList xmlNodeList = (XmlNodeList)null;
                    string attribute1 = diffgramParent1.GetAttribute("match");
                    if (attribute1 != string.Empty)
                    {
                        xmlNodeList = PathDescriptorParser.SelectNodes(this._sourceRootNode, sourceParent, attribute1);
                        if (xmlNodeList.Count == 0)
                            XmlPatchError.Error("Invalid XDL diffgram. No node matches the path descriptor '{0}'.", attribute1);
                    }
                    XmlPatchOperation newChild = (XmlPatchOperation)null;
                    switch (diffgramParent1.LocalName)
                    {
                        case "node":
                            if (xmlNodeList.Count != 1)
                                XmlPatchError.Error("Invalid XDL diffgram; more than one node matches the '{0}' path descriptor on the xd:node or xd:change element.", attribute1);
                            XmlNode xmlNode2 = xmlNodeList.Item(0);
                            if (this._sourceRootNode.NodeType != XmlNodeType.Document || xmlNode2.NodeType != XmlNodeType.XmlDeclaration && xmlNode2.NodeType != XmlNodeType.DocumentType)
                            {
                                newChild = (XmlPatchOperation)new PatchSetPosition(xmlNode2);
                                this.CreatePatchForChildren(xmlNode2, diffgramParent1, (XmlPatchParentOperation)newChild);
                                break;
                            }
                            break;
                        case "add":
                            if (attribute1 != string.Empty)
                            {
                                bool bSubtree = diffgramParent1.GetAttribute("subtree") != "no";
                                newChild = (XmlPatchOperation)new PatchCopy(xmlNodeList, bSubtree);
                                if (!bSubtree)
                                {
                                    this.CreatePatchForChildren(sourceParent, diffgramParent1, (XmlPatchParentOperation)newChild);
                                    break;
                                }
                                break;
                            }
                            string attribute2 = diffgramParent1.GetAttribute("type");
                            if (attribute2 != string.Empty)
                            {
                                XmlNodeType nodeType = (XmlNodeType)int.Parse(attribute2);
                                bool flag = nodeType == XmlNodeType.Element;
                                if (nodeType != XmlNodeType.DocumentType)
                                {
                                    newChild = (XmlPatchOperation)new PatchAddNode(nodeType, diffgramParent1.GetAttribute("name"), diffgramParent1.GetAttribute("ns"), diffgramParent1.GetAttribute("prefix"), flag ? string.Empty : diffgramParent1.InnerText, this._ignoreChildOrder);
                                    if (flag)
                                    {
                                        this.CreatePatchForChildren(sourceParent, diffgramParent1, (XmlPatchParentOperation)newChild);
                                        break;
                                    }
                                    break;
                                }
                                newChild = (XmlPatchOperation)new PatchAddNode(nodeType, diffgramParent1.GetAttribute("name"), diffgramParent1.GetAttribute("systemId"), diffgramParent1.GetAttribute("publicId"), diffgramParent1.InnerText, this._ignoreChildOrder);
                                break;
                            }
                            newChild = (XmlPatchOperation)new PatchAddXmlFragment(diffgramParent1.ChildNodes);
                            break;
                        case "remove":
                            bool bSubtree1 = diffgramParent1.GetAttribute("subtree") != "no";
                            newChild = (XmlPatchOperation)new PatchRemove(xmlNodeList, bSubtree1);
                            if (!bSubtree1)
                            {
                                this.CreatePatchForChildren(xmlNodeList.Item(0), diffgramParent1, (XmlPatchParentOperation)newChild);
                                break;
                            }
                            break;
                        case "change":
                            if (xmlNodeList.Count != 1)
                                XmlPatchError.Error("Invalid XDL diffgram; more than one node matches the '{0}' path descriptor on the xd:node or xd:change element.", attribute1);
                            XmlNode xmlNode3 = xmlNodeList.Item(0);
                            newChild = xmlNode3.NodeType == XmlNodeType.DocumentType ? (XmlPatchOperation)new PatchChange(xmlNode3, diffgramParent1.HasAttribute("name") ? diffgramParent1.GetAttribute("name") : (string)null, diffgramParent1.HasAttribute("systemId") ? diffgramParent1.GetAttribute("systemId") : (string)null, diffgramParent1.HasAttribute("publicId") ? diffgramParent1.GetAttribute("publicId") : (string)null, diffgramParent1.IsEmpty ? (XmlNode)null : (XmlNode)diffgramParent1) : (XmlPatchOperation)new PatchChange(xmlNode3, diffgramParent1.HasAttribute("name") ? diffgramParent1.GetAttribute("name") : (string)null, diffgramParent1.HasAttribute("ns") ? diffgramParent1.GetAttribute("ns") : (string)null, diffgramParent1.HasAttribute("prefix") ? diffgramParent1.GetAttribute("prefix") : (string)null, xmlNode3.NodeType == XmlNodeType.Element ? (XmlNode)null : (XmlNode)diffgramParent1);
                            if (xmlNode3.NodeType == XmlNodeType.Element)
                            {
                                this.CreatePatchForChildren(xmlNode3, diffgramParent1, (XmlPatchParentOperation)newChild);
                                break;
                            }
                            break;
                        case "descriptor":
                            return;
                    }
                    if (newChild != null)
                    {
                        patchParent.InsertChildAfter(child, newChild);
                        child = newChild;
                    }
                    xmlNode1 = xmlNode1.NextSibling;
                }
            }
        }
    }
}
