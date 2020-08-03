// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiff
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System;
using System.Collections;
using System.IO;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  public class XmlDiff
  {
    private bool _bIgnoreChildOrder = false;
    private bool _bIgnoreComments = false;
    private bool _bIgnorePI = false;
    private bool _bIgnoreWhitespace = false;
    private bool _bIgnoreNamespaces = false;
    private bool _bIgnorePrefixes = false;
    private bool _bIgnoreXmlDecl = false;
    private bool _bIgnoreDtd = false;
    private XmlDiffAlgorithm _algorithm = XmlDiffAlgorithm.Auto;
    internal XmlDiffDocument _sourceDoc = (XmlDiffDocument) null;
    internal XmlDiffDocument _targetDoc = (XmlDiffDocument) null;
    internal XmlDiffNode[] _sourceNodes = (XmlDiffNode[]) null;
    internal XmlDiffNode[] _targetNodes = (XmlDiffNode[]) null;
    internal TriStateBool _fragments = TriStateBool.DontKnown;
    public XmlDiffPerf _xmlDiffPerf = new XmlDiffPerf();
    private const int MininumNodesForQuicksort = 5;
    private const int MaxTotalNodesCountForTreeDistance = 256;
    public const string NamespaceUri = "http://schemas.microsoft.com/xmltools/2002/xmldiff";
    internal const string Prefix = "xd";
    internal const string XmlnsNamespaceUri = "http://www.w3.org/2000/xmlns/";

    public XmlDiff()
    {
    }

    public XmlDiff(XmlDiffOptions options)
      : this()
    {
      this.Options = options;
    }

    public bool IgnoreChildOrder
    {
      get
      {
        return this._bIgnoreChildOrder;
      }
      set
      {
        this._bIgnoreChildOrder = value;
      }
    }

    public bool IgnoreComments
    {
      get
      {
        return this._bIgnoreComments;
      }
      set
      {
        this._bIgnoreComments = value;
      }
    }

    public bool IgnorePI
    {
      get
      {
        return this._bIgnorePI;
      }
      set
      {
        this._bIgnorePI = value;
      }
    }

    public bool IgnoreWhitespace
    {
      get
      {
        return this._bIgnoreWhitespace;
      }
      set
      {
        this._bIgnoreWhitespace = value;
      }
    }

    public bool IgnoreNamespaces
    {
      get
      {
        return this._bIgnoreNamespaces;
      }
      set
      {
        this._bIgnoreNamespaces = value;
      }
    }

    public bool IgnorePrefixes
    {
      get
      {
        return this._bIgnorePrefixes;
      }
      set
      {
        this._bIgnorePrefixes = value;
      }
    }

    public bool IgnoreXmlDecl
    {
      get
      {
        return this._bIgnoreXmlDecl;
      }
      set
      {
        this._bIgnoreXmlDecl = value;
      }
    }

    public bool IgnoreDtd
    {
      get
      {
        return this._bIgnoreDtd;
      }
      set
      {
        this._bIgnoreDtd = value;
      }
    }

    public XmlDiffOptions Options
    {
      set
      {
        this.IgnoreChildOrder = (value & XmlDiffOptions.IgnoreChildOrder) > XmlDiffOptions.None;
        this.IgnoreComments = (value & XmlDiffOptions.IgnoreComments) > XmlDiffOptions.None;
        this.IgnorePI = (value & XmlDiffOptions.IgnorePI) > XmlDiffOptions.None;
        this.IgnoreWhitespace = (value & XmlDiffOptions.IgnoreWhitespace) > XmlDiffOptions.None;
        this.IgnoreNamespaces = (value & XmlDiffOptions.IgnoreNamespaces) > XmlDiffOptions.None;
        this.IgnorePrefixes = (value & XmlDiffOptions.IgnorePrefixes) > XmlDiffOptions.None;
        this.IgnoreXmlDecl = (value & XmlDiffOptions.IgnoreXmlDecl) > XmlDiffOptions.None;
        this.IgnoreDtd = (value & XmlDiffOptions.IgnoreDtd) > XmlDiffOptions.None;
      }
    }

    public XmlDiffAlgorithm Algorithm
    {
      get
      {
        return this._algorithm;
      }
      set
      {
        this._algorithm = value;
      }
    }

    public bool Compare(string sourceFile, string changedFile, bool bFragments)
    {
      return this.Compare(sourceFile, changedFile, bFragments, (XmlWriter) null);
    }

    public bool Compare(
      string sourceFile,
      string changedFile,
      bool bFragments,
      XmlWriter diffgramWriter)
    {
      if (sourceFile == null)
        throw new ArgumentNullException(nameof (sourceFile));
      if (changedFile == null)
        throw new ArgumentNullException(nameof (changedFile));
            var sourceReader = (XmlReader) null;
            var changedReader = (XmlReader) null;
      try
      {
        this._fragments = bFragments ? TriStateBool.Yes : TriStateBool.No;
        if (bFragments)
          this.OpenFragments(sourceFile, changedFile, ref sourceReader, ref changedReader);
        else
          this.OpenDocuments(sourceFile, changedFile, ref sourceReader, ref changedReader);
        return this.Compare(sourceReader, changedReader, diffgramWriter);
      }
      finally
      {
        sourceReader?.Close();
        changedReader?.Close();
      }
    }

    private void OpenDocuments(
      string sourceFile,
      string changedFile,
      ref XmlReader sourceReader,
      ref XmlReader changedReader)
    {
      sourceReader = (XmlReader) new XmlTextReader(sourceFile)
      {
        XmlResolver = (XmlResolver) null
      };
      changedReader = (XmlReader) new XmlTextReader(changedFile)
      {
        XmlResolver = (XmlResolver) null
      };
    }

    private void OpenFragments(
      string sourceFile,
      string changedFile,
      ref XmlReader sourceReader,
      ref XmlReader changedReader)
    {
            var fileStream1 = (FileStream) null;
            var fileStream2 = (FileStream) null;
      try
      {
                var xmlNameTable = (XmlNameTable) new NameTable();
                var context1 = new XmlParserContext(xmlNameTable, new XmlNamespaceManager(xmlNameTable), string.Empty, XmlSpace.Default);
                var context2 = new XmlParserContext(xmlNameTable, new XmlNamespaceManager(xmlNameTable), string.Empty, XmlSpace.Default);
        fileStream1 = new FileStream(sourceFile, FileMode.Open, FileAccess.Read);
        fileStream2 = new FileStream(changedFile, FileMode.Open, FileAccess.Read);
        sourceReader = (XmlReader) new XmlTextReader((Stream) fileStream1, XmlNodeType.Element, context1)
        {
          XmlResolver = (XmlResolver) null
        };
        changedReader = (XmlReader) new XmlTextReader((Stream) fileStream2, XmlNodeType.Element, context2)
        {
          XmlResolver = (XmlResolver) null
        };
      }
      catch
      {
        fileStream1?.Close();
        fileStream2?.Close();
        throw;
      }
    }

    public bool Compare(XmlReader sourceReader, XmlReader changedReader)
    {
      return this.Compare(sourceReader, changedReader, (XmlWriter) null);
    }

    public bool Compare(XmlReader sourceReader, XmlReader changedReader, XmlWriter diffgramWriter)
    {
      if (sourceReader == null)
        throw new ArgumentNullException(nameof (sourceReader));
      if (changedReader == null)
        throw new ArgumentNullException(nameof (changedReader));
      try
      {
                var xmlHash = new XmlHash(this);
        this._xmlDiffPerf.Clean();
        var tickCount = Environment.TickCount;
        this._sourceDoc = new XmlDiffDocument(this);
        this._sourceDoc.Load(sourceReader, xmlHash);
        this._targetDoc = new XmlDiffDocument(this);
        this._targetDoc.Load(changedReader, xmlHash);
        if (this._fragments == TriStateBool.DontKnown)
          this._fragments = this._sourceDoc.IsFragment || this._targetDoc.IsFragment ? TriStateBool.Yes : TriStateBool.No;
        this._xmlDiffPerf._loadTime = Environment.TickCount - tickCount;
        return this.Diff(diffgramWriter);
      }
      finally
      {
        this._sourceDoc = (XmlDiffDocument) null;
        this._targetDoc = (XmlDiffDocument) null;
      }
    }

    public bool Compare(XmlNode sourceNode, XmlNode changedNode)
    {
      return this.Compare(sourceNode, changedNode, (XmlWriter) null);
    }

    public bool Compare(XmlNode sourceNode, XmlNode changedNode, XmlWriter diffgramWriter)
    {
      if (sourceNode == null)
        throw new ArgumentNullException(nameof (sourceNode));
      if (changedNode == null)
        throw new ArgumentNullException(nameof (changedNode));
      try
      {
                var xmlHash = new XmlHash(this);
        this._xmlDiffPerf.Clean();
        var tickCount = Environment.TickCount;
        this._sourceDoc = new XmlDiffDocument(this);
        this._sourceDoc.Load(sourceNode, xmlHash);
        this._targetDoc = new XmlDiffDocument(this);
        this._targetDoc.Load(changedNode, xmlHash);
        this._fragments = sourceNode.NodeType != XmlNodeType.Document || changedNode.NodeType != XmlNodeType.Document ? TriStateBool.Yes : TriStateBool.No;
        this._xmlDiffPerf._loadTime = Environment.TickCount - tickCount;
        return this.Diff(diffgramWriter);
      }
      finally
      {
        this._sourceDoc = (XmlDiffDocument) null;
        this._targetDoc = (XmlDiffDocument) null;
      }
    }

    private bool Diff(XmlWriter diffgramWriter)
    {
      try
      {
        var tickCount1 = Environment.TickCount;
        if (this.IdenticalSubtrees((XmlDiffNode) this._sourceDoc, (XmlDiffNode) this._targetDoc))
        {
          if (diffgramWriter != null)
          {
            new DiffgramGenerator(this).GenerateEmptyDiffgram().WriteTo(diffgramWriter);
            diffgramWriter.Flush();
          }
          this._xmlDiffPerf._identicalOrNoDiffWriterTime = Environment.TickCount - tickCount1;
          return true;
        }
        if (diffgramWriter == null)
        {
          this._xmlDiffPerf._identicalOrNoDiffWriterTime = Environment.TickCount - tickCount1;
          return false;
        }
        this._xmlDiffPerf._identicalOrNoDiffWriterTime = Environment.TickCount - tickCount1;
        var tickCount2 = Environment.TickCount;
        this.MatchIdenticalSubtrees();
        this._xmlDiffPerf._matchTime = Environment.TickCount - tickCount2;
                var diffgram = (Diffgram) null;
        switch (this._algorithm)
        {
          case XmlDiffAlgorithm.Auto:
            diffgram = this._sourceDoc.NodesCount + this._targetDoc.NodesCount > 256 ? this.WalkTreeAlgorithm() : this.ZhangShashaAlgorithm();
            break;
          case XmlDiffAlgorithm.Fast:
            diffgram = this.WalkTreeAlgorithm();
            break;
          case XmlDiffAlgorithm.Precise:
            diffgram = this.ZhangShashaAlgorithm();
            break;
        }
        var tickCount3 = Environment.TickCount;
        diffgram.WriteTo(diffgramWriter);
        diffgramWriter.Flush();
        this._xmlDiffPerf._diffgramSaveTime = Environment.TickCount - tickCount3;
      }
      finally
      {
        this._sourceNodes = (XmlDiffNode[]) null;
        this._targetNodes = (XmlDiffNode[]) null;
      }
      return false;
    }

    private Diffgram ZhangShashaAlgorithm()
    {
      var tickCount1 = Environment.TickCount;
      this.PreprocessTree(this._sourceDoc, ref this._sourceNodes);
      this.PreprocessTree(this._targetDoc, ref this._targetNodes);
      this._xmlDiffPerf._preprocessTime = Environment.TickCount - tickCount1;
      var tickCount2 = Environment.TickCount;
            var minimalDistance = new MinimalTreeDistanceAlgo(this).FindMinimalDistance();
      this._xmlDiffPerf._treeDistanceTime = Environment.TickCount - tickCount2;
      var tickCount3 = Environment.TickCount;
            var fromEditScript = new DiffgramGenerator(this).GenerateFromEditScript(minimalDistance);
      this._xmlDiffPerf._diffgramGenerationTime = Environment.TickCount - tickCount3;
      return fromEditScript;
    }

    private void PreprocessTree(XmlDiffDocument doc, ref XmlDiffNode[] postOrderArray)
    {
      postOrderArray = new XmlDiffNode[(int) checked ((uint) unchecked (doc.NodesCount + 1))];
      postOrderArray[0] = (XmlDiffNode) null;
      var currentIndex = 1;
      this.PreprocessNode((XmlDiffNode) doc, ref postOrderArray, ref currentIndex);
      doc._bKeyRoot = true;
    }

    private void PreprocessNode(
      XmlDiffNode node,
      ref XmlDiffNode[] postOrderArray,
      ref int currentIndex)
    {
      if (node.HasChildNodes)
      {
                var node1 = node.FirstChildNode;
        node1._bKeyRoot = false;
        while (true)
        {
          this.PreprocessNode(node1, ref postOrderArray, ref currentIndex);
          node1 = node1._nextSibling;
          if (node1 != null)
            node1._bKeyRoot = true;
          else
            break;
        }
        node.Left = node.FirstChildNode.Left;
      }
      else
      {
        node.Left = currentIndex;
        node.NodesCount = 1;
      }
      postOrderArray[currentIndex++] = node;
    }

    private void MatchIdenticalSubtrees()
    {
            var hashtable1 = new Hashtable(16);
            var hashtable2 = new Hashtable(16);
            var queue1 = new Queue(16);
            var queue2 = new Queue(16);
      queue1.Enqueue((object) this._sourceDoc);
      queue2.Enqueue((object) this._targetDoc);
      while (queue1.Count > 0 || queue2.Count > 0)
      {
        foreach (XmlDiffParentNode xmlDiffParentNode in queue1)
        {
          xmlDiffParentNode._bExpanded = true;
          if (xmlDiffParentNode.HasChildNodes)
          {
            for (var node = xmlDiffParentNode._firstChildNode; node != null; node = node._nextSibling)
              this.AddNodeToHashTable(hashtable1, node);
          }
        }
        var count1 = queue2.Count;
        for (var index = 0; index < count1; ++index)
        {
                    var xmlDiffParentNode = (XmlDiffParentNode) queue2.Dequeue();
          xmlDiffParentNode._bExpanded = true;
          if (xmlDiffParentNode.HasChildNodes)
          {
                        var xmlDiffNode1 = xmlDiffParentNode._firstChildNode;
            while (xmlDiffNode1 != null)
            {
                            var xmlDiffNode2 = (XmlDiffNode) null;
                            var nodeListHead = (XmlDiffNodeListHead) hashtable1[(object) xmlDiffNode1.HashValue];
              if (nodeListHead != null)
                xmlDiffNode2 = this.HTFindAndRemoveMatchingNode(hashtable1, nodeListHead, xmlDiffNode1);
              if (xmlDiffNode2 == null || xmlDiffNode1.NodeType < XmlDiffNodeType.None)
              {
                if (xmlDiffNode1.HasChildNodes)
                  queue2.Enqueue((object) xmlDiffNode1);
                else
                  xmlDiffNode1._bExpanded = true;
                this.AddNodeToHashTable(hashtable2, xmlDiffNode1);
                xmlDiffNode1 = xmlDiffNode1._nextSibling;
              }
              else
              {
                this.HTRemoveAncestors(hashtable1, xmlDiffNode2);
                this.HTRemoveDescendants(hashtable1, xmlDiffNode2);
                this.HTRemoveAncestors(hashtable2, xmlDiffNode1);
                                var firstTargetNode = xmlDiffNode1;
                                var lastSourceNode = xmlDiffNode2;
                                var lastTargetNode = firstTargetNode;
                xmlDiffNode1 = xmlDiffNode1._nextSibling;
                for (var nextSibling = xmlDiffNode2._nextSibling; xmlDiffNode1 != null && nextSibling != null && (nextSibling.NodeType != XmlDiffNodeType.ShrankNode && (this.IdenticalSubtrees(nextSibling, xmlDiffNode1) && hashtable1.Contains((object) nextSibling.HashValue))); xmlDiffNode1 = xmlDiffNode1._nextSibling)
                {
                  this.HTRemoveNode(hashtable1, nextSibling);
                  this.HTRemoveDescendants(hashtable1, nextSibling);
                  lastSourceNode = nextSibling;
                  nextSibling = nextSibling._nextSibling;
                  lastTargetNode = xmlDiffNode1;
                }
                if (xmlDiffNode2 != lastSourceNode || xmlDiffNode2.NodeType != XmlDiffNodeType.Element)
                {
                  this.ShrinkNodeInterval(xmlDiffNode2, lastSourceNode, firstTargetNode, lastTargetNode);
                }
                else
                {
                                    var xmlDiffElement = (XmlDiffElement) xmlDiffNode2;
                  if (xmlDiffElement.FirstChildNode != null || xmlDiffElement._attributes != null)
                    this.ShrinkNodeInterval(xmlDiffNode2, lastSourceNode, firstTargetNode, lastTargetNode);
                }
              }
            }
          }
        }
        var count2 = queue1.Count;
        for (var index = 0; index < count2; ++index)
        {
                    var xmlDiffParentNode = (XmlDiffParentNode) queue1.Dequeue();
          if (xmlDiffParentNode.HasChildNodes)
          {
                        var xmlDiffNode1 = xmlDiffParentNode._firstChildNode;
            while (xmlDiffNode1 != null)
            {
              if (xmlDiffNode1 is XmlDiffShrankNode || !this.NodeInHashTable(hashtable1, xmlDiffNode1))
              {
                xmlDiffNode1 = xmlDiffNode1._nextSibling;
              }
              else
              {
                                var xmlDiffNode2 = (XmlDiffNode) null;
                                var nodeListHead = (XmlDiffNodeListHead) hashtable2[(object) xmlDiffNode1.HashValue];
                if (nodeListHead != null)
                  xmlDiffNode2 = this.HTFindAndRemoveMatchingNode(hashtable2, nodeListHead, xmlDiffNode1);
                if (xmlDiffNode2 == null || xmlDiffNode1.NodeType < XmlDiffNodeType.None)
                {
                  if (xmlDiffNode1.HasChildNodes)
                    queue1.Enqueue((object) xmlDiffNode1);
                  else
                    xmlDiffNode1._bExpanded = true;
                  xmlDiffNode1 = xmlDiffNode1._nextSibling;
                }
                else
                {
                  this.HTRemoveAncestors(hashtable2, xmlDiffNode2);
                  this.HTRemoveDescendants(hashtable2, xmlDiffNode2);
                  this.HTRemoveNode(hashtable1, xmlDiffNode1);
                  this.HTRemoveAncestors(hashtable1, xmlDiffNode1);
                                    var firstSourceNode = xmlDiffNode1;
                                    var lastSourceNode = firstSourceNode;
                                    var lastTargetNode = xmlDiffNode2;
                  xmlDiffNode1 = xmlDiffNode1._nextSibling;
                  for (var nextSibling = xmlDiffNode2._nextSibling; xmlDiffNode1 != null && nextSibling != null && (nextSibling.NodeType != XmlDiffNodeType.ShrankNode && (this.IdenticalSubtrees(xmlDiffNode1, nextSibling) && hashtable1.Contains((object) xmlDiffNode1.HashValue))) && hashtable2.Contains((object) nextSibling.HashValue); nextSibling = nextSibling._nextSibling)
                  {
                    this.HTRemoveNode(hashtable1, xmlDiffNode1);
                    this.HTRemoveDescendants(hashtable1, xmlDiffNode1);
                    this.HTRemoveNode(hashtable2, nextSibling);
                    this.HTRemoveDescendants(hashtable2, nextSibling);
                    lastSourceNode = xmlDiffNode1;
                    xmlDiffNode1 = xmlDiffNode1._nextSibling;
                    lastTargetNode = nextSibling;
                  }
                  if (firstSourceNode != lastSourceNode || firstSourceNode.NodeType != XmlDiffNodeType.Element)
                  {
                    this.ShrinkNodeInterval(firstSourceNode, lastSourceNode, xmlDiffNode2, lastTargetNode);
                  }
                  else
                  {
                                        var xmlDiffElement = (XmlDiffElement) firstSourceNode;
                    if (xmlDiffElement.FirstChildNode != null || xmlDiffElement._attributes != null)
                      this.ShrinkNodeInterval(firstSourceNode, lastSourceNode, xmlDiffNode2, lastTargetNode);
                  }
                }
              }
            }
          }
        }
      }
    }

    private void AddNodeToHashTable(Hashtable hashtable, XmlDiffNode node)
    {
      var hashValue = node.HashValue;
            var diffNodeListHead = (XmlDiffNodeListHead) hashtable[(object) hashValue];
      if (diffNodeListHead == null)
      {
        hashtable[(object) hashValue] = (object) new XmlDiffNodeListHead(new XmlDiffNodeListMember(node, (XmlDiffNodeListMember) null));
      }
      else
      {
                var diffNodeListMember = new XmlDiffNodeListMember(node, (XmlDiffNodeListMember) null);
        diffNodeListHead._last._next = diffNodeListMember;
        diffNodeListHead._last = diffNodeListMember;
      }
    }

    private bool HTRemoveNode(Hashtable hashtable, XmlDiffNode node)
    {
            var diffNodeListHead = (XmlDiffNodeListHead) hashtable[(object) node.HashValue];
      if (diffNodeListHead == null)
        return false;
            var diffNodeListMember = diffNodeListHead._first;
      if (diffNodeListMember._node == node)
      {
        if (diffNodeListMember._next == null)
          hashtable.Remove((object) node.HashValue);
        else
          diffNodeListHead._first = diffNodeListMember._next;
      }
      else
      {
        if (diffNodeListMember._next == null)
          return false;
        while (diffNodeListMember._next._node != node)
        {
          diffNodeListMember = diffNodeListMember._next;
          if (diffNodeListMember._next == null)
            return false;
        }
        diffNodeListMember._next = diffNodeListMember._next._next;
        if (diffNodeListMember._next == null)
          diffNodeListHead._last = diffNodeListMember;
      }
      return true;
    }

    private bool NodeInHashTable(Hashtable hashtable, XmlDiffNode node)
    {
            var diffNodeListHead = (XmlDiffNodeListHead) hashtable[(object) node.HashValue];
      if (diffNodeListHead == null)
        return false;
      for (var diffNodeListMember = diffNodeListHead._first; diffNodeListMember != null; diffNodeListMember = diffNodeListMember._next)
      {
        if (diffNodeListMember._node == node)
          return true;
      }
      return false;
    }

    private void ShrinkNodeInterval(
      XmlDiffNode firstSourceNode,
      XmlDiffNode lastSourceNode,
      XmlDiffNode firstTargetNode,
      XmlDiffNode lastTargetNode)
    {
            var firstPreviousSibbling1 = (XmlDiffNode) null;
            var firstPreviousSibbling2 = (XmlDiffNode) null;
      if (this.IgnoreChildOrder && firstSourceNode != lastSourceNode)
      {
        XmlDiff.SortNodesByPosition(ref firstSourceNode, ref lastSourceNode, ref firstPreviousSibbling1);
        XmlDiff.SortNodesByPosition(ref firstTargetNode, ref lastTargetNode, ref firstPreviousSibbling2);
      }
            var xmlDiffShrankNode1 = this.ReplaceNodeIntervalWithShrankNode(firstSourceNode, lastSourceNode, firstPreviousSibbling1);
            var xmlDiffShrankNode2 = this.ReplaceNodeIntervalWithShrankNode(firstTargetNode, lastTargetNode, firstPreviousSibbling2);
      xmlDiffShrankNode1.MatchingShrankNode = xmlDiffShrankNode2;
      xmlDiffShrankNode2.MatchingShrankNode = xmlDiffShrankNode1;
    }

    private XmlDiffShrankNode ReplaceNodeIntervalWithShrankNode(
      XmlDiffNode firstNode,
      XmlDiffNode lastNode,
      XmlDiffNode previousSibling)
    {
            var xmlDiffShrankNode = new XmlDiffShrankNode(firstNode, lastNode);
            var parent = firstNode._parent;
      if (previousSibling == null && firstNode != parent._firstChildNode)
      {
        previousSibling = parent._firstChildNode;
        while (previousSibling._nextSibling != firstNode)
          previousSibling = previousSibling._nextSibling;
      }
      if (previousSibling == null)
      {
        xmlDiffShrankNode._nextSibling = parent._firstChildNode;
        parent._firstChildNode = (XmlDiffNode) xmlDiffShrankNode;
      }
      else
      {
        xmlDiffShrankNode._nextSibling = previousSibling._nextSibling;
        previousSibling._nextSibling = (XmlDiffNode) xmlDiffShrankNode;
      }
      xmlDiffShrankNode._parent = parent;
      var num1 = 0;
      XmlDiffNode nextSibling;
      do
      {
        nextSibling = xmlDiffShrankNode._nextSibling;
        num1 += nextSibling.NodesCount;
        xmlDiffShrankNode._nextSibling = xmlDiffShrankNode._nextSibling._nextSibling;
      }
      while (nextSibling != lastNode);
      if (num1 > 1)
      {
        var num2 = num1 - 1;
        for (; parent != null; parent = parent._parent)
          parent.NodesCount -= num2;
      }
      return xmlDiffShrankNode;
    }

    private XmlDiffNode HTFindAndRemoveMatchingNode(
      Hashtable hashtable,
      XmlDiffNodeListHead nodeListHead,
      XmlDiffNode nodeToMatch)
    {
            var first = nodeListHead._first;
            var node = first._node;
      if (this.IdenticalSubtrees(node, nodeToMatch))
      {
        if (first._next == null)
          hashtable.Remove((object) node.HashValue);
        else
          nodeListHead._first = first._next;
        return node;
      }
      while (first._next != null)
      {
        if (this.IdenticalSubtrees(first._node, nodeToMatch))
        {
          first._next = first._next._next;
          if (first._next == null)
            nodeListHead._last = first;
          return node;
        }
      }
      return (XmlDiffNode) null;
    }

    private void HTRemoveAncestors(Hashtable hashtable, XmlDiffNode node)
    {
      for (var parent = (XmlDiffNode) node._parent; parent != null && this.HTRemoveNode(hashtable, parent); parent = (XmlDiffNode) parent._parent)
        parent._bSomeDescendantMatches = true;
    }

    private void HTRemoveDescendants(Hashtable hashtable, XmlDiffNode parent)
    {
      if (!parent._bExpanded || !parent.HasChildNodes)
        return;
            var node = parent.FirstChildNode;
      while (true)
      {
        for (; !node._bExpanded || !node.HasChildNodes; node = node._nextSibling)
        {
          this.HTRemoveNode(hashtable, node);
          for (; node._nextSibling == null; node = (XmlDiffNode) node._parent)
          {
            if (node._parent == parent)
              return;
          }
        }
        node = ((XmlDiffParentNode) node)._firstChildNode;
      }
    }

    private void RemoveDescendantsFromHashTable(Hashtable hashtable, XmlDiffNode parentNode)
    {
    }

    internal static void SortNodesByPosition(
      ref XmlDiffNode firstNode,
      ref XmlDiffNode lastNode,
      ref XmlDiffNode firstPreviousSibbling)
    {
            var parent = firstNode._parent;
      if (firstPreviousSibbling == null && firstNode != parent._firstChildNode)
      {
        firstPreviousSibbling = parent._firstChildNode;
        while (firstPreviousSibbling._nextSibling != firstNode)
          firstPreviousSibbling = firstPreviousSibbling._nextSibling;
      }
            var nextSibling = lastNode._nextSibling;
      lastNode._nextSibling = (XmlDiffNode) null;
      var count = 0;
      for (var xmlDiffNode = firstNode; xmlDiffNode != null; xmlDiffNode = xmlDiffNode._nextSibling)
        ++count;
      if (count >= 5)
        XmlDiff.QuickSortNodes(ref firstNode, ref lastNode, count, firstPreviousSibbling, nextSibling);
      else
        XmlDiff.SlowSortNodes(ref firstNode, ref lastNode, firstPreviousSibbling, nextSibling);
    }

    private static void SlowSortNodes(
      ref XmlDiffNode firstNode,
      ref XmlDiffNode lastNode,
      XmlDiffNode firstPreviousSibbling,
      XmlDiffNode lastNextSibling)
    {
            var xmlDiffNode1 = firstNode;
            var xmlDiffNode2 = firstNode;
            var xmlDiffNode3 = firstNode._nextSibling;
      xmlDiffNode2._nextSibling = (XmlDiffNode) null;
      while (xmlDiffNode3 != null)
      {
                var xmlDiffNode4 = xmlDiffNode1;
        if (xmlDiffNode3.Position < xmlDiffNode1.Position)
        {
                    var nextSibling = xmlDiffNode3._nextSibling;
          xmlDiffNode3._nextSibling = xmlDiffNode1;
          xmlDiffNode1 = xmlDiffNode3;
          xmlDiffNode3 = nextSibling;
        }
        else
        {
          while (xmlDiffNode4._nextSibling != null && xmlDiffNode3.Position > xmlDiffNode4._nextSibling.Position)
            xmlDiffNode4 = xmlDiffNode4._nextSibling;
                    var nextSibling = xmlDiffNode3._nextSibling;
          if (xmlDiffNode4._nextSibling == null)
            xmlDiffNode2 = xmlDiffNode3;
          xmlDiffNode3._nextSibling = xmlDiffNode4._nextSibling;
          xmlDiffNode4._nextSibling = xmlDiffNode3;
          xmlDiffNode3 = nextSibling;
        }
      }
      if (firstPreviousSibbling == null)
        firstNode._parent._firstChildNode = xmlDiffNode1;
      else
        firstPreviousSibbling._nextSibling = xmlDiffNode1;
      xmlDiffNode2._nextSibling = lastNextSibling;
      firstNode = xmlDiffNode1;
      lastNode = xmlDiffNode2;
    }

    private static void QuickSortNodes(
      ref XmlDiffNode firstNode,
      ref XmlDiffNode lastNode,
      int count,
      XmlDiffNode firstPreviousSibbling,
      XmlDiffNode lastNextSibling)
    {
      var sortArray = new XmlDiffNode[(int) checked ((uint) count)];
            var xmlDiffNode = firstNode;
      var index1 = 0;
      while (index1 < count)
      {
        sortArray[index1] = xmlDiffNode;
        ++index1;
        xmlDiffNode = xmlDiffNode._nextSibling;
      }
      XmlDiff.QuickSortNodesRecursion(ref sortArray, 0, count - 1);
      for (var index2 = 0; index2 < count - 1; ++index2)
        sortArray[index2]._nextSibling = sortArray[index2 + 1];
      if (firstPreviousSibbling == null)
        firstNode._parent._firstChildNode = sortArray[0];
      else
        firstPreviousSibbling._nextSibling = sortArray[0];
      sortArray[count - 1]._nextSibling = lastNextSibling;
      firstNode = sortArray[0];
      lastNode = sortArray[count - 1];
    }

    private static void QuickSortNodesRecursion(
      ref XmlDiffNode[] sortArray,
      int firstIndex,
      int lastIndex)
    {
      var position = sortArray[(firstIndex + lastIndex) / 2].Position;
      var firstIndex1 = firstIndex;
      var lastIndex1 = lastIndex;
      while (firstIndex1 < lastIndex1)
      {
        while (sortArray[firstIndex1].Position < position)
          ++firstIndex1;
        while (sortArray[lastIndex1].Position > position)
          --lastIndex1;
        if (firstIndex1 < lastIndex1)
        {
                    var xmlDiffNode = sortArray[firstIndex1];
          sortArray[firstIndex1] = sortArray[lastIndex1];
          sortArray[lastIndex1] = xmlDiffNode;
          ++firstIndex1;
          --lastIndex1;
        }
        else if (firstIndex1 == lastIndex1)
        {
          ++firstIndex1;
          --lastIndex1;
        }
      }
      if (firstIndex < lastIndex1)
        XmlDiff.QuickSortNodesRecursion(ref sortArray, firstIndex, lastIndex1);
      if (firstIndex1 >= lastIndex)
        return;
      XmlDiff.QuickSortNodesRecursion(ref sortArray, firstIndex1, lastIndex);
    }

    private bool IdenticalSubtrees(XmlDiffNode node1, XmlDiffNode node2)
    {
      return (long) node1.HashValue == (long) node2.HashValue && this.CompareSubtrees(node1, node2);
    }

    private bool CompareSubtrees(XmlDiffNode node1, XmlDiffNode node2)
    {
      if (!node1.IsSameAs(node2, this))
        return false;
      if (!node1.HasChildNodes)
        return true;
            var node1_1 = node1.FirstChildNode;
      XmlDiffNode node2_1;
      for (node2_1 = node2.FirstChildNode; node1_1 != null && node2_1 != null; node2_1 = node2_1._nextSibling)
      {
        if (!this.CompareSubtrees(node1_1, node2_1))
          return false;
        node1_1 = node1_1._nextSibling;
      }
      return node1_1 == node2_1;
    }

    internal static bool IsChangeOperation(XmlDiffOperation op)
    {
      return op >= XmlDiffOperation.ChangeElementName && op <= XmlDiffOperation.ChangeDTD;
    }

    internal static bool IsChangeOperationOnAttributesOnly(XmlDiffOperation op)
    {
      return op >= XmlDiffOperation.ChangeElementAttr1 && op <= XmlDiffOperation.ChangeElementAttr3;
    }

    public static XmlDiffOptions ParseOptions(string options)
    {
      if (options == null)
        throw new ArgumentNullException(nameof (options));
      if (options == XmlDiffOptions.None.ToString())
        return XmlDiffOptions.None;
            var xmlDiffOptions = XmlDiffOptions.None;
      int num;
      for (var startIndex = 0; startIndex < options.Length; startIndex = num + 1)
      {
        num = options.IndexOf(' ', startIndex);
        if (num == -1)
          num = options.Length;
        switch (options.Substring(startIndex, num - startIndex))
        {
          case "IgnoreChildOrder":
            xmlDiffOptions |= XmlDiffOptions.IgnoreChildOrder;
            break;
          case "IgnoreComments":
            xmlDiffOptions |= XmlDiffOptions.IgnoreComments;
            break;
          case "IgnoreNamespaces":
            xmlDiffOptions |= XmlDiffOptions.IgnoreNamespaces;
            break;
          case "IgnorePI":
            xmlDiffOptions |= XmlDiffOptions.IgnorePI;
            break;
          case "IgnorePrefixes":
            xmlDiffOptions |= XmlDiffOptions.IgnorePrefixes;
            break;
          case "IgnoreWhitespace":
            xmlDiffOptions |= XmlDiffOptions.IgnoreWhitespace;
            break;
          case "IgnoreXmlDecl":
            xmlDiffOptions |= XmlDiffOptions.IgnoreXmlDecl;
            break;
          case "IgnoreDtd":
            xmlDiffOptions |= XmlDiffOptions.IgnoreDtd;
            break;
          default:
            throw new ArgumentException(nameof (options));
        }
      }
      return xmlDiffOptions;
    }

    internal string GetXmlDiffOptionsString()
    {
      var str = string.Empty;
      if (this._bIgnoreChildOrder)
        str = str + XmlDiffOptions.IgnoreChildOrder.ToString() + " ";
      if (this._bIgnoreComments)
        str = str + XmlDiffOptions.IgnoreComments.ToString() + " ";
      if (this._bIgnoreNamespaces)
        str = str + XmlDiffOptions.IgnoreNamespaces.ToString() + " ";
      if (this._bIgnorePI)
        str = str + XmlDiffOptions.IgnorePI.ToString() + " ";
      if (this._bIgnorePrefixes)
        str = str + XmlDiffOptions.IgnorePrefixes.ToString() + " ";
      if (this._bIgnoreWhitespace)
        str = str + XmlDiffOptions.IgnoreWhitespace.ToString() + " ";
      if (this._bIgnoreXmlDecl)
        str = str + XmlDiffOptions.IgnoreXmlDecl.ToString() + " ";
      if (this._bIgnoreDtd)
        str = str + XmlDiffOptions.IgnoreDtd.ToString() + " ";
      if (str == string.Empty)
        str = XmlDiffOptions.None.ToString();
      str.Trim();
      return str;
    }

    public static bool VerifySource(XmlNode node, ulong hashValue, XmlDiffOptions options)
    {
      if (node == null)
        throw new ArgumentNullException(nameof (node));
      var hash = new XmlHash().ComputeHash(node, options);
      return (long) hashValue == (long) hash;
    }

    internal static string NormalizeText(string text)
    {
      var charArray = text.ToCharArray();
      var length = 0;
      var index = 0;
      while (true)
      {
        for (; index >= charArray.Length || !XmlDiff.IsWhitespace(text[index]); ++index)
        {
          while (index < charArray.Length && !XmlDiff.IsWhitespace(text[index]))
            charArray[length++] = charArray[index++];
          if (index < charArray.Length)
          {
            charArray[length++] = ' ';
          }
          else
          {
            if (index == 0)
              return string.Empty;
            if (XmlDiff.IsWhitespace(charArray[index - 1]))
              --length;
            return new string(charArray, 0, length);
          }
        }
        ++index;
      }
    }

    internal static string NormalizeXmlDeclaration(string value)
    {
      value = value.Replace('\'', '"');
      return XmlDiff.NormalizeText(value);
    }

    internal static bool IsWhitespace(char c)
    {
      return c == ' ' || c == '\t' || c == '\n' || c == '\r';
    }

    private Diffgram WalkTreeAlgorithm()
    {
      return new DiffgramGenerator(this).GenerateFromWalkTree();
    }

    private class XmlDiffNodeListMember
    {
      internal XmlDiffNode _node;
      internal XmlDiffNodeListMember _next;

      internal XmlDiffNodeListMember(XmlDiffNode node, XmlDiffNodeListMember next)
      {
        this._node = node;
        this._next = next;
      }
    }

    private class XmlDiffNodeListHead
    {
      internal XmlDiffNodeListMember _first;
      internal XmlDiffNodeListMember _last;

      internal XmlDiffNodeListHead(XmlDiffNodeListMember firstMember)
      {
        this._first = firstMember;
        this._last = firstMember;
      }
    }
  }
}
