// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramOperation
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Text;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
    internal abstract class DiffgramOperation
    {
        internal DiffgramOperation _nextSiblingOp;
        protected ulong _operationID;

        internal DiffgramOperation(ulong operationID)
        {
            this._nextSiblingOp = (DiffgramOperation)null;
            this._operationID = operationID;
        }

        internal abstract void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff);

        internal static string GetRelativeAddressOfNodeset(XmlDiffNode firstNode, XmlDiffNode lastNode)
        {
            int num = -1;
            bool flag = false;
            StringBuilder stringBuilder = new StringBuilder();
            XmlDiffNode xmlDiffNode = firstNode;
            while (true)
            {
                if (xmlDiffNode.Position != num + 1)
                {
                    if (flag)
                    {
                        stringBuilder.Append(num);
                        flag = false;
                        stringBuilder.Append('|');
                    }
                    stringBuilder.Append(xmlDiffNode.Position);
                    if (xmlDiffNode != lastNode)
                    {
                        if (xmlDiffNode._nextSibling.Position == xmlDiffNode.Position + 1)
                        {
                            stringBuilder.Append("-");
                            flag = true;
                        }
                        else
                            stringBuilder.Append('|');
                    }
                }
                if (xmlDiffNode != lastNode)
                {
                    num = xmlDiffNode.Position;
                    xmlDiffNode = xmlDiffNode._nextSibling;
                }
                else
                    break;
            }
            if (flag)
                stringBuilder.Append(lastNode.Position);
            return stringBuilder.ToString();
        }

        internal static void GetAddressOfAttributeInterval(
          AttributeInterval interval,
          XmlWriter xmlWriter)
        {
            if (interval._next == null)
            {
                if (interval._firstAttr == interval._lastAttr)
                {
                    xmlWriter.WriteAttributeString("match", interval._firstAttr.GetRelativeAddress());
                    return;
                }
                if (interval._firstAttr._parent._firstChildNode == interval._firstAttr && (interval._lastAttr._nextSibling == null || interval._lastAttr._nextSibling.NodeType != XmlDiffNodeType.Attribute))
                {
                    xmlWriter.WriteAttributeString("match", "@*");
                    return;
                }
            }
            string str1 = string.Empty;
            string str2;
            while (true)
            {
                XmlDiffAttribute xmlDiffAttribute = interval._firstAttr;
                while (true)
                {
                    str2 = str1 + xmlDiffAttribute.GetRelativeAddress();
                    if (xmlDiffAttribute != interval._lastAttr)
                    {
                        str1 = str2 + "|";
                        xmlDiffAttribute = (XmlDiffAttribute)xmlDiffAttribute._nextSibling;
                    }
                    else
                        break;
                }
                interval = interval._next;
                if (interval != null)
                    str1 = str2 + "|";
                else
                    break;
            }
            xmlWriter.WriteAttributeString("match", str2);
        }

        internal static void WriteAbsoluteMatchAttribute(XmlDiffNode node, XmlWriter xmlWriter)
        {
            if (node is XmlDiffAttribute attr && attr.NamespaceURI != string.Empty)
            {
                DiffgramOperation.WriteNamespaceDefinition(attr, xmlWriter);
            }

            xmlWriter.WriteAttributeString("match", node.GetAbsoluteAddress());
        }

        private static void WriteNamespaceDefinition(XmlDiffAttribute attr, XmlWriter xmlWriter)
        {
            if (attr.Prefix != string.Empty)
                xmlWriter.WriteAttributeString("xmlns", attr.Prefix, "http://www.w3.org/2000/xmlns/", attr.NamespaceURI);
            else
                xmlWriter.WriteAttributeString(string.Empty, "xmlns", "http://www.w3.org/2000/xmlns/", attr.NamespaceURI);
        }
    }
}
