// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramAddSubtrees
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class DiffgramAddSubtrees : DiffgramOperation
  {
    internal XmlDiffNode _firstTargetNode;
    internal XmlDiffNode _lastTargetNode;
    private bool _bSorted;
    private bool _bNeedNamespaces;

    internal DiffgramAddSubtrees(XmlDiffNode subtreeRoot, ulong operationID, bool bSorted)
      : base(operationID)
    {
      this._firstTargetNode = subtreeRoot;
      this._lastTargetNode = subtreeRoot;
      this._bSorted = bSorted;
      this._bNeedNamespaces = subtreeRoot.NodeType == XmlDiffNodeType.Element;
    }

    internal override void WriteTo(XmlWriter xmlWriter, XmlDiff xmlDiff)
    {
      if (!this._bSorted)
        this.Sort();
      xmlWriter.WriteStartElement("xd", "add", "http://schemas.microsoft.com/xmltools/2002/xmldiff");
      if (this._operationID != 0UL)
        xmlWriter.WriteAttributeString("opid", this._operationID.ToString());
      if (this._bNeedNamespaces)
      {
        Hashtable hashtable = new Hashtable();
        for (XmlDiffParentNode parent = this._firstTargetNode._parent; parent != null; parent = parent._parent)
        {
          if (parent._bDefinesNamespaces)
          {
            for (XmlDiffAttributeOrNamespace attributeOrNamespace = ((XmlDiffElement) parent)._attributes; attributeOrNamespace != null && attributeOrNamespace.NodeType == XmlDiffNodeType.Namespace; attributeOrNamespace = (XmlDiffAttributeOrNamespace) attributeOrNamespace._nextSibling)
            {
              if (hashtable[(object) attributeOrNamespace.Prefix] == null)
              {
                if (attributeOrNamespace.Prefix == string.Empty)
                  xmlWriter.WriteAttributeString("xmlns", "http://www.w3.org/2000/xmlns/", attributeOrNamespace.NamespaceURI);
                else
                  xmlWriter.WriteAttributeString("xmlns", attributeOrNamespace.Prefix, "http://www.w3.org/2000/xmlns/", attributeOrNamespace.NamespaceURI);
                hashtable[(object) attributeOrNamespace.Prefix] = (object) attributeOrNamespace.Prefix;
              }
            }
          }
        }
      }
      XmlDiffNode xmlDiffNode = this._firstTargetNode;
      while (true)
      {
        xmlDiffNode.WriteTo(xmlWriter);
        if (xmlDiffNode != this._lastTargetNode)
          xmlDiffNode = xmlDiffNode._nextSibling;
        else
          break;
      }
      xmlWriter.WriteEndElement();
    }

    private void Sort()
    {
      XmlDiffNode firstPreviousSibbling = (XmlDiffNode) null;
      XmlDiff.SortNodesByPosition(ref this._firstTargetNode, ref this._lastTargetNode, ref firstPreviousSibbling);
      this._bSorted = true;
    }

    internal bool SetNewFirstNode(XmlDiffNode targetNode)
    {
      if (this._operationID != 0UL || targetNode._nextSibling != this._firstTargetNode || targetNode.NodeType == XmlDiffNodeType.Text && this._firstTargetNode.NodeType == XmlDiffNodeType.Text || (!targetNode.CanMerge || !this._firstTargetNode.CanMerge))
        return false;
      this._firstTargetNode = targetNode;
      if (targetNode.NodeType == XmlDiffNodeType.Element)
        this._bNeedNamespaces = true;
      return true;
    }

    internal bool SetNewLastNode(XmlDiffNode targetNode)
    {
      if (this._operationID != 0UL || this._lastTargetNode._nextSibling != targetNode || targetNode.NodeType == XmlDiffNodeType.Text && this._lastTargetNode.NodeType == XmlDiffNodeType.Text || (!targetNode.CanMerge || !this._lastTargetNode.CanMerge))
        return false;
      this._lastTargetNode = targetNode;
      if (targetNode.NodeType == XmlDiffNodeType.Element)
        this._bNeedNamespaces = true;
      return true;
    }
  }
}
