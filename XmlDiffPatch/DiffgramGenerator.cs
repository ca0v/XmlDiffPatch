// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.DiffgramGenerator
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System;
using System.Collections;

namespace Microsoft.XmlDiffPatch
{
  internal class DiffgramGenerator
  {
    internal Hashtable _moveDescriptors = new Hashtable(8);
    private DiffgramGenerator.PrefixChange _prefixChangeDescr = (DiffgramGenerator.PrefixChange) null;
    private DiffgramGenerator.NamespaceChange _namespaceChangeDescr = (DiffgramGenerator.NamespaceChange) null;
    private bool _bBuildingAddTree = false;
    private DiffgramPosition _cachedDiffgramPosition = new DiffgramPosition((XmlDiffNode) null);
    internal const int MoveHashtableInitialSize = 8;
    private const int LogMultiplier = 4;
    private XmlDiff _xmlDiff;
    private bool _bChildOrderSignificant;
    private ulong _lastOperationID;
    private EditScript _editScript;
    private XmlDiffNode[] _sourceNodes;
    private XmlDiffNode[] _targetNodes;
    private int _curSourceIndex;
    private int _curTargetIndex;
    private DiffgramGenerator.PostponedEditScriptInfo _postponedEditScript;

    internal DiffgramGenerator(XmlDiff xmlDiff)
    {
      this._xmlDiff = xmlDiff;
      this._bChildOrderSignificant = !xmlDiff.IgnoreChildOrder;
      this._lastOperationID = 0UL;
    }

    internal Diffgram GenerateFromEditScript(EditScript editScript)
    {
      this._sourceNodes = this._xmlDiff._sourceNodes;
      this._targetNodes = this._xmlDiff._targetNodes;
      Diffgram diffgram = new Diffgram(this._xmlDiff);
      EditScriptMatch editScriptMatch = editScript as EditScriptMatch;
      if (editScript.Operation == EditScriptOperation.Match && editScriptMatch._firstSourceIndex + editScriptMatch._length == this._sourceNodes.Length && editScriptMatch._firstTargetIndex + editScriptMatch._length == this._targetNodes.Length)
      {
        --editScriptMatch._length;
        if (editScriptMatch._length == 0)
          editScript = editScriptMatch._nextEditScript;
      }
      this._curSourceIndex = this._sourceNodes.Length - 2;
      this._curTargetIndex = this._targetNodes.Length - 2;
      this._editScript = editScript;
      this.GenerateDiffgramMatch((DiffgramParentOperation) diffgram, 1, 1);
      this.AppendDescriptors(diffgram);
      return diffgram;
    }

    private void AppendDescriptors(Diffgram diffgram)
    {
      IDictionaryEnumerator enumerator = this._moveDescriptors.GetEnumerator();
      while (enumerator.MoveNext())
        diffgram.AddDescriptor((OperationDescriptor) new OperationDescrMove((ulong) enumerator.Value));
      for (DiffgramGenerator.NamespaceChange nsChange = this._namespaceChangeDescr; nsChange != null; nsChange = nsChange._next)
        diffgram.AddDescriptor((OperationDescriptor) new OperationDescrNamespaceChange(nsChange));
      for (DiffgramGenerator.PrefixChange prefixChange = this._prefixChangeDescr; prefixChange != null; prefixChange = prefixChange._next)
        diffgram.AddDescriptor((OperationDescriptor) new OperationDescrPrefixChange(prefixChange));
    }

    private void GenerateDiffgramMatch(
      DiffgramParentOperation parent,
      int sourceBorderIndex,
      int targetBorderIndex)
    {
      bool bNeedPosition = false;
      while (this._curSourceIndex >= sourceBorderIndex || this._curTargetIndex >= targetBorderIndex)
      {
        switch (this._editScript.Operation)
        {
          case EditScriptOperation.Match:
            this.OnMatch(parent, bNeedPosition);
            bNeedPosition = false;
            continue;
          case EditScriptOperation.Add:
            bNeedPosition = this.OnAdd(parent, sourceBorderIndex, targetBorderIndex);
            continue;
          case EditScriptOperation.Remove:
            if (this._curSourceIndex < sourceBorderIndex)
              return;
            this.OnRemove(parent);
            continue;
          case EditScriptOperation.ChangeNode:
            if (this._curSourceIndex < sourceBorderIndex)
              return;
            this.OnChange(parent);
            continue;
          case EditScriptOperation.EditScriptPostponed:
            if (this._curSourceIndex < sourceBorderIndex)
              return;
            this.OnEditScriptPostponed(parent, targetBorderIndex);
            continue;
          default:
            continue;
        }
      }
    }

    private void GenerateDiffgramAdd(
      DiffgramParentOperation parent,
      int sourceBorderIndex,
      int targetBorderIndex)
    {
      while (this._curTargetIndex >= targetBorderIndex)
      {
        switch (this._editScript.Operation)
        {
          case EditScriptOperation.Match:
            this.OnMatch(parent, false);
            continue;
          case EditScriptOperation.Add:
            this.OnAdd(parent, sourceBorderIndex, targetBorderIndex);
            continue;
          case EditScriptOperation.Remove:
            this.OnRemove(parent);
            continue;
          case EditScriptOperation.ChangeNode:
            this.OnChange(parent);
            continue;
          case EditScriptOperation.EditScriptPostponed:
            this.OnEditScriptPostponed(parent, targetBorderIndex);
            continue;
          default:
            continue;
        }
      }
    }

    private void GenerateDiffgramPostponed(
      DiffgramParentOperation parent,
      ref EditScript editScript,
      int sourceBorderIndex,
      int targetBorderIndex)
    {
      while (this._curSourceIndex >= sourceBorderIndex && editScript != null)
      {
        if (!(editScript is EditScriptPostponed editScriptPostponed))
        {
          this.GenerateDiffgramMatch(parent, sourceBorderIndex, targetBorderIndex);
          break;
        }
        int startSourceIndex = editScriptPostponed._startSourceIndex;
        int left = this._sourceNodes[editScriptPostponed._endSourceIndex].Left;
        DiffgramOperation diffOperation = editScriptPostponed._diffOperation;
        this._curSourceIndex = editScriptPostponed._startSourceIndex - 1;
        editScript = editScriptPostponed._nextEditScript;
        if (startSourceIndex > left)
          this.GenerateDiffgramPostponed((DiffgramParentOperation) diffOperation, ref editScript, left, targetBorderIndex);
        parent.InsertAtBeginning(diffOperation);
      }
    }

    private void OnMatch(DiffgramParentOperation parent, bool bNeedPosition)
    {
      EditScriptMatch editScript = this._editScript as EditScriptMatch;
      int index1 = editScript._firstTargetIndex + editScript._length - 1;
      int index2 = editScript._firstSourceIndex + editScript._length - 1;
      XmlDiffNode targetNode = this._targetNodes[index1];
      XmlDiffNode sourceNode = this._sourceNodes[index2];
      if (editScript._firstTargetIndex <= targetNode.Left && editScript._firstSourceIndex <= sourceNode.Left)
      {
        if (this._bBuildingAddTree)
        {
          ulong operationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
          parent.InsertAtBeginning((DiffgramOperation) new DiffgramCopy(sourceNode, true, operationId));
          this.PostponedRemoveSubtrees(sourceNode, operationId, sourceNode.Left, index2);
        }
        else if (sourceNode.NodeType == XmlDiffNodeType.Element)
        {
          DiffgramPosition diffgramPosition = this._cachedDiffgramPosition;
          diffgramPosition._sourceNode = sourceNode;
          this.GenerateChangeDiffgramForAttributes((DiffgramParentOperation) diffgramPosition, (XmlDiffElement) sourceNode, (XmlDiffElement) targetNode);
          if (diffgramPosition._firstChildOp != null || bNeedPosition)
          {
            parent.InsertAtBeginning((DiffgramOperation) diffgramPosition);
            this._cachedDiffgramPosition = new DiffgramPosition((XmlDiffNode) null);
            bNeedPosition = false;
          }
        }
        else if (bNeedPosition)
        {
          parent.InsertAtBeginning((DiffgramOperation) new DiffgramPosition(sourceNode));
          bNeedPosition = false;
        }
        else if (!this._bChildOrderSignificant && sourceNode.NodeType < XmlDiffNodeType.None)
        {
          switch (parent._firstChildOp)
          {
            case DiffgramAddNode _:
            case DiffgramAddSubtrees _:
            case DiffgramCopy _:
              parent.InsertAtBeginning((DiffgramOperation) new DiffgramPosition(sourceNode));
              break;
          }
        }
        this._curSourceIndex = sourceNode.Left - 1;
        this._curTargetIndex = targetNode.Left - 1;
        editScript._length -= index1 - targetNode.Left + 1;
        if (editScript._length > 0)
          return;
        this._editScript = this._editScript._nextEditScript;
      }
      else
      {
        --this._curSourceIndex;
        --this._curTargetIndex;
        --editScript._length;
        if (editScript._length <= 0)
          this._editScript = this._editScript._nextEditScript;
        if (this._bBuildingAddTree)
        {
          ulong operationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
          bool bSubtree = sourceNode.NodeType != XmlDiffNodeType.Element;
          DiffgramParentOperation parent1 = (DiffgramParentOperation) new DiffgramCopy(sourceNode, bSubtree, operationId);
          this.PostponedRemoveNode(sourceNode, bSubtree, operationId, index2, index2);
          this.GenerateDiffgramAdd(parent1, sourceNode.Left, targetNode.Left);
          parent.InsertAtBeginning((DiffgramOperation) parent1);
        }
        else
        {
          DiffgramParentOperation parent1 = (DiffgramParentOperation) new DiffgramPosition(sourceNode);
          this.GenerateDiffgramMatch(parent1, sourceNode.Left, targetNode.Left);
          if (parent1._firstChildOp == null)
            return;
          parent.InsertAtBeginning((DiffgramOperation) parent1);
        }
      }
    }

    private bool OnAdd(
      DiffgramParentOperation parent,
      int sourceBorderIndex,
      int targetBorderIndex)
    {
      EditScriptAdd editScript = this._editScript as EditScriptAdd;
      XmlDiffNode targetNode = this._targetNodes[editScript._endTargetIndex];
      if (editScript._startTargetIndex <= targetNode.Left && !targetNode._bSomeDescendantMatches)
      {
        switch (targetNode.NodeType)
        {
          case XmlDiffNodeType.XmlDeclaration:
          case XmlDiffNodeType.DocumentType:
          case XmlDiffNodeType.EntityReference:
            parent.InsertAtBeginning((DiffgramOperation) new DiffgramAddNode(targetNode, 0UL));
            break;
          case XmlDiffNodeType.ShrankNode:
            XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode) targetNode;
            if (xmlDiffShrankNode.MoveOperationId == 0UL)
              xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
            parent.InsertAtBeginning((DiffgramOperation) new DiffgramCopy((XmlDiffNode) xmlDiffShrankNode.MatchingShrankNode, true, xmlDiffShrankNode.MoveOperationId));
            break;
          default:
            if (!parent.MergeAddSubtreeAtBeginning(targetNode))
            {
              parent.InsertAtBeginning((DiffgramOperation) new DiffgramAddSubtrees(targetNode, 0UL, !this._xmlDiff.IgnoreChildOrder));
              break;
            }
            break;
        }
        this._curTargetIndex = targetNode.Left - 1;
        editScript._endTargetIndex = targetNode.Left - 1;
        if (editScript._startTargetIndex > editScript._endTargetIndex)
          this._editScript = this._editScript._nextEditScript;
      }
      else
      {
        DiffgramAddNode diffgramAddNode = new DiffgramAddNode(targetNode, 0UL);
        --this._curTargetIndex;
        --editScript._endTargetIndex;
        if (editScript._startTargetIndex > editScript._endTargetIndex)
          this._editScript = this._editScript._nextEditScript;
        if (this._bBuildingAddTree)
        {
          this.GenerateDiffgramAdd((DiffgramParentOperation) diffgramAddNode, sourceBorderIndex, targetNode.Left);
        }
        else
        {
          this._postponedEditScript.Reset();
          this._bBuildingAddTree = true;
          this.GenerateDiffgramAdd((DiffgramParentOperation) diffgramAddNode, sourceBorderIndex, targetNode.Left);
          this._bBuildingAddTree = false;
          if (this._postponedEditScript._firstES != null)
          {
            this._curSourceIndex = this._postponedEditScript._endSourceIndex;
            this._postponedEditScript._lastES._nextEditScript = this._editScript;
            this._editScript = (EditScript) this._postponedEditScript._firstES;
          }
        }
        if (targetNode.NodeType == XmlDiffNodeType.Element)
          this.GenerateAddDiffgramForAttributes((DiffgramParentOperation) diffgramAddNode, (XmlDiffElement) targetNode);
        parent.InsertAtBeginning((DiffgramOperation) diffgramAddNode);
      }
      return this._bChildOrderSignificant && !this._bBuildingAddTree;
    }

    private void OnRemove(DiffgramParentOperation parent)
    {
      EditScriptRemove editScript = this._editScript as EditScriptRemove;
      XmlDiffNode sourceNode = this._sourceNodes[editScript._endSourceIndex];
      if (editScript._startSourceIndex <= sourceNode.Left)
      {
        bool flag = sourceNode is XmlDiffShrankNode;
        if (sourceNode._bSomeDescendantMatches && !flag)
        {
          DiffgramOperation descendantMatches = this.GenerateDiffgramRemoveWhenDescendantMatches(sourceNode);
          if (this._bBuildingAddTree)
            this.PostponedOperation(descendantMatches, sourceNode.Left, editScript._endSourceIndex);
          else
            parent.InsertAtBeginning(descendantMatches);
        }
        else
        {
          ulong operationID = 0;
          if (flag)
          {
            XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode) sourceNode;
            if (xmlDiffShrankNode.MoveOperationId == 0UL)
              xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
            operationID = xmlDiffShrankNode.MoveOperationId;
          }
          if (this._bBuildingAddTree)
            this.PostponedRemoveSubtrees(sourceNode, operationID, sourceNode.Left, editScript._endSourceIndex);
          else if (operationID != 0UL || !parent.MergeRemoveSubtreeAtBeginning(sourceNode))
            parent.InsertAtBeginning((DiffgramOperation) new DiffgramRemoveSubtrees(sourceNode, operationID, !this._xmlDiff.IgnoreChildOrder));
        }
        this._curSourceIndex = sourceNode.Left - 1;
        editScript._endSourceIndex = sourceNode.Left - 1;
        if (editScript._startSourceIndex <= editScript._endSourceIndex)
          return;
        this._editScript = this._editScript._nextEditScript;
      }
      else
      {
        --this._curSourceIndex;
        --editScript._endSourceIndex;
        if (editScript._startSourceIndex > editScript._endSourceIndex)
          this._editScript = this._editScript._nextEditScript;
        bool bSubtree = sourceNode.NodeType != XmlDiffNodeType.Element;
        if (this._bBuildingAddTree)
        {
          this.PostponedRemoveNode(sourceNode, bSubtree, 0UL, editScript._endSourceIndex + 1, editScript._endSourceIndex + 1);
          this.GenerateDiffgramAdd(parent, sourceNode.Left, this._targetNodes[this._curTargetIndex].Left);
        }
        else
        {
          DiffgramRemoveNode diffgramRemoveNode = new DiffgramRemoveNode(sourceNode, bSubtree, 0UL);
          this.GenerateDiffgramMatch((DiffgramParentOperation) diffgramRemoveNode, sourceNode.Left, this._targetNodes[this._curTargetIndex].Left);
          parent.InsertAtBeginning((DiffgramOperation) diffgramRemoveNode);
        }
      }
    }

    private void OnChange(DiffgramParentOperation parent)
    {
      EditScriptChange editScript = this._editScript as EditScriptChange;
      XmlDiffNode sourceNode = this._sourceNodes[editScript._sourceIndex];
      XmlDiffNode targetNode = this._targetNodes[editScript._targetIndex];
      --this._curSourceIndex;
      --this._curTargetIndex;
      this._editScript = this._editScript._nextEditScript;
      DiffgramOperation newOp;
      if (this._bBuildingAddTree)
      {
        newOp = targetNode.NodeType != XmlDiffNodeType.Element ? (DiffgramOperation) new DiffgramAddSubtrees(targetNode, 0UL, !this._xmlDiff.IgnoreChildOrder) : (DiffgramOperation) new DiffgramAddNode(targetNode, 0UL);
        bool bSubtree = sourceNode.NodeType != XmlDiffNodeType.Element;
        this.PostponedRemoveNode(sourceNode, bSubtree, 0UL, editScript._sourceIndex, editScript._sourceIndex);
        if (sourceNode.Left < editScript._sourceIndex || targetNode.Left < editScript._targetIndex)
          this.GenerateDiffgramAdd((DiffgramParentOperation) newOp, sourceNode.Left, targetNode.Left);
        if (targetNode.NodeType == XmlDiffNodeType.Element)
          this.GenerateAddDiffgramForAttributes((DiffgramParentOperation) newOp, (XmlDiffElement) targetNode);
      }
      else
      {
        ulong operationID = 0;
        if (!this._xmlDiff.IgnoreNamespaces && sourceNode.NodeType == XmlDiffNodeType.Element)
        {
          XmlDiffElement xmlDiffElement1 = (XmlDiffElement) sourceNode;
          XmlDiffElement xmlDiffElement2 = (XmlDiffElement) targetNode;
          if (xmlDiffElement1.LocalName == xmlDiffElement2.LocalName)
            operationID = this.GetNamespaceChangeOpid(xmlDiffElement1.NamespaceURI, xmlDiffElement1.Prefix, xmlDiffElement2.NamespaceURI, xmlDiffElement2.Prefix);
        }
        if (sourceNode.NodeType == XmlDiffNodeType.Element)
        {
          newOp = !XmlDiff.IsChangeOperationOnAttributesOnly(editScript._changeOp) ? (DiffgramOperation) new DiffgramChangeNode(sourceNode, targetNode, XmlDiffOperation.ChangeElementName, operationID) : (DiffgramOperation) new DiffgramPosition(sourceNode);
          if (sourceNode.Left < editScript._sourceIndex || targetNode.Left < editScript._targetIndex)
            this.GenerateDiffgramMatch((DiffgramParentOperation) newOp, sourceNode.Left, targetNode.Left);
          this.GenerateChangeDiffgramForAttributes((DiffgramParentOperation) newOp, (XmlDiffElement) sourceNode, (XmlDiffElement) targetNode);
        }
        else
          newOp = (DiffgramOperation) new DiffgramChangeNode(sourceNode, targetNode, editScript._changeOp, operationID);
      }
      parent.InsertAtBeginning(newOp);
    }

    private void PostponedRemoveNode(
      XmlDiffNode sourceNode,
      bool bSubtree,
      ulong operationID,
      int startSourceIndex,
      int endSourceIndex)
    {
      this.PostponedOperation((DiffgramOperation) new DiffgramRemoveNode(sourceNode, bSubtree, operationID), startSourceIndex, endSourceIndex);
    }

    private void PostponedRemoveSubtrees(
      XmlDiffNode sourceNode,
      ulong operationID,
      int startSourceIndex,
      int endSourceIndex)
    {
      if (operationID == 0UL && this._postponedEditScript._firstES != null && (this._postponedEditScript._lastES._diffOperation is DiffgramRemoveSubtrees diffOperation && diffOperation.SetNewFirstNode(sourceNode)))
      {
        this._postponedEditScript._lastES._startSourceIndex = startSourceIndex;
        this._postponedEditScript._startSourceIndex = startSourceIndex;
      }
      else
        this.PostponedOperation((DiffgramOperation) new DiffgramRemoveSubtrees(sourceNode, operationID, !this._xmlDiff.IgnoreChildOrder), startSourceIndex, endSourceIndex);
    }

    private void PostponedOperation(DiffgramOperation op, int startSourceIndex, int endSourceIndex)
    {
      EditScriptPostponed editScriptPostponed = new EditScriptPostponed(op, startSourceIndex, endSourceIndex);
      if (this._postponedEditScript._firstES == null)
      {
        this._postponedEditScript._firstES = editScriptPostponed;
        this._postponedEditScript._lastES = editScriptPostponed;
        this._postponedEditScript._startSourceIndex = startSourceIndex;
        this._postponedEditScript._endSourceIndex = endSourceIndex;
      }
      else
      {
        this._postponedEditScript._lastES._nextEditScript = (EditScript) editScriptPostponed;
        this._postponedEditScript._lastES = editScriptPostponed;
        this._postponedEditScript._startSourceIndex = startSourceIndex;
      }
    }

    private ulong GenerateOperationID(XmlDiffDescriptorType descriptorType)
    {
      ulong num = ++this._lastOperationID;
      if (descriptorType == XmlDiffDescriptorType.Move)
        this._moveDescriptors.Add((object) num, (object) num);
      return num;
    }

    private ulong GetNamespaceChangeOpid(
      string oldNamespaceURI,
      string oldPrefix,
      string newNamespaceURI,
      string newPrefix)
    {
      ulong opid = 0;
      if (oldNamespaceURI != newNamespaceURI)
      {
        if (oldPrefix != newPrefix)
          return 0;
        for (DiffgramGenerator.NamespaceChange namespaceChange = this._namespaceChangeDescr; namespaceChange != null; namespaceChange = namespaceChange._next)
        {
          if (namespaceChange._oldNS == oldNamespaceURI && namespaceChange._prefix == oldPrefix && namespaceChange._newNS == newNamespaceURI)
            return namespaceChange._opid;
        }
        opid = this.GenerateOperationID(XmlDiffDescriptorType.NamespaceChange);
        this._namespaceChangeDescr = new DiffgramGenerator.NamespaceChange(oldPrefix, oldNamespaceURI, newNamespaceURI, opid, this._namespaceChangeDescr);
      }
      else if (!this._xmlDiff.IgnorePrefixes && oldPrefix != newPrefix)
      {
        for (DiffgramGenerator.PrefixChange prefixChange = this._prefixChangeDescr; prefixChange != null; prefixChange = prefixChange._next)
        {
          if (prefixChange._NS == oldNamespaceURI && prefixChange._oldPrefix == oldPrefix && prefixChange._newPrefix == newPrefix)
            return prefixChange._opid;
        }
        opid = this.GenerateOperationID(XmlDiffDescriptorType.PrefixChange);
        this._prefixChangeDescr = new DiffgramGenerator.PrefixChange(oldPrefix, newPrefix, oldNamespaceURI, opid, this._prefixChangeDescr);
      }
      return opid;
    }

    internal Diffgram GenerateEmptyDiffgram()
    {
      return new Diffgram(this._xmlDiff);
    }

    private void GenerateChangeDiffgramForAttributes(
      DiffgramParentOperation diffgramParent,
      XmlDiffElement sourceElement,
      XmlDiffElement targetElement)
    {
      XmlDiffAttributeOrNamespace attributeOrNamespace1 = sourceElement._attributes;
      XmlDiffAttributeOrNamespace attributeOrNamespace2 = targetElement._attributes;
      while (attributeOrNamespace1 != null && attributeOrNamespace2 != null)
      {
        ulong operationID = 0;
        if (attributeOrNamespace1.NodeType == attributeOrNamespace2.NodeType)
        {
          int num1;
          if (attributeOrNamespace1.NodeType == XmlDiffNodeType.Attribute)
          {
            if ((num1 = XmlDiffDocument.OrderStrings(attributeOrNamespace1.LocalName, attributeOrNamespace2.LocalName)) == 0)
            {
              if (this._xmlDiff.IgnoreNamespaces)
              {
                if (XmlDiffDocument.OrderStrings(attributeOrNamespace1.Value, attributeOrNamespace2.Value) == 0)
                  goto label_16;
              }
              else if (XmlDiffDocument.OrderStrings(attributeOrNamespace1.NamespaceURI, attributeOrNamespace2.NamespaceURI) == 0 && (this._xmlDiff.IgnorePrefixes || XmlDiffDocument.OrderStrings(attributeOrNamespace1.Prefix, attributeOrNamespace2.Prefix) == 0) && XmlDiffDocument.OrderStrings(attributeOrNamespace1.Value, attributeOrNamespace2.Value) == 0)
                goto label_16;
              diffgramParent.InsertAtBeginning((DiffgramOperation) new DiffgramChangeNode((XmlDiffNode) attributeOrNamespace1, (XmlDiffNode) attributeOrNamespace2, XmlDiffOperation.ChangeAttr, 0UL));
            }
            else
              goto label_17;
          }
          else if (this._xmlDiff.IgnorePrefixes)
          {
            if ((num1 = XmlDiffDocument.OrderStrings(attributeOrNamespace1.NamespaceURI, attributeOrNamespace2.NamespaceURI)) != 0)
              goto label_17;
          }
          else
          {
            int num2;
            ulong namespaceChangeOpid;
            if ((num2 = XmlDiffDocument.OrderStrings(attributeOrNamespace1.Prefix, attributeOrNamespace2.Prefix)) == 0)
            {
              if ((num2 = XmlDiffDocument.OrderStrings(attributeOrNamespace1.NamespaceURI, attributeOrNamespace2.NamespaceURI)) != 0)
                namespaceChangeOpid = this.GetNamespaceChangeOpid(attributeOrNamespace1.NamespaceURI, attributeOrNamespace1.Prefix, attributeOrNamespace2.NamespaceURI, attributeOrNamespace2.Prefix);
              else
                goto label_16;
            }
            else if ((num1 = XmlDiffDocument.OrderStrings(attributeOrNamespace1.NamespaceURI, attributeOrNamespace2.NamespaceURI)) == 0)
              namespaceChangeOpid = this.GetNamespaceChangeOpid(attributeOrNamespace1.NamespaceURI, attributeOrNamespace1.Prefix, attributeOrNamespace2.NamespaceURI, attributeOrNamespace2.Prefix);
            else
              goto label_17;
            if (!diffgramParent.MergeRemoveAttributeAtBeginning((XmlDiffNode) attributeOrNamespace1))
              diffgramParent.InsertAtBeginning((DiffgramOperation) new DiffgramRemoveNode((XmlDiffNode) attributeOrNamespace1, true, namespaceChangeOpid));
            attributeOrNamespace1 = (XmlDiffAttributeOrNamespace) attributeOrNamespace1._nextSibling;
            diffgramParent.InsertAtBeginning((DiffgramOperation) new DiffgramAddNode((XmlDiffNode) attributeOrNamespace2, namespaceChangeOpid));
            attributeOrNamespace2 = (XmlDiffAttributeOrNamespace) attributeOrNamespace2._nextSibling;
            continue;
          }
label_16:
          attributeOrNamespace1 = (XmlDiffAttributeOrNamespace) attributeOrNamespace1._nextSibling;
          attributeOrNamespace2 = (XmlDiffAttributeOrNamespace) attributeOrNamespace2._nextSibling;
          continue;
label_17:
          if (num1 != -1)
            goto label_24;
        }
        else if (attributeOrNamespace1.NodeType != XmlDiffNodeType.Namespace)
          goto label_24;
        if (!diffgramParent.MergeRemoveAttributeAtBeginning((XmlDiffNode) attributeOrNamespace1))
          diffgramParent.InsertAtBeginning((DiffgramOperation) new DiffgramRemoveNode((XmlDiffNode) attributeOrNamespace1, true, operationID));
        attributeOrNamespace1 = (XmlDiffAttributeOrNamespace) attributeOrNamespace1._nextSibling;
        continue;
label_24:
        diffgramParent.InsertAtBeginning((DiffgramOperation) new DiffgramAddNode((XmlDiffNode) attributeOrNamespace2, operationID));
        attributeOrNamespace2 = (XmlDiffAttributeOrNamespace) attributeOrNamespace2._nextSibling;
      }
      for (; attributeOrNamespace1 != null; attributeOrNamespace1 = (XmlDiffAttributeOrNamespace) attributeOrNamespace1._nextSibling)
      {
        if (!diffgramParent.MergeRemoveAttributeAtBeginning((XmlDiffNode) attributeOrNamespace1))
          diffgramParent.InsertAtBeginning((DiffgramOperation) new DiffgramRemoveNode((XmlDiffNode) attributeOrNamespace1, true, 0UL));
      }
      for (; attributeOrNamespace2 != null; attributeOrNamespace2 = (XmlDiffAttributeOrNamespace) attributeOrNamespace2._nextSibling)
        diffgramParent.InsertAtBeginning((DiffgramOperation) new DiffgramAddNode((XmlDiffNode) attributeOrNamespace2, 0UL));
    }

    private void GenerateAddDiffgramForAttributes(
      DiffgramParentOperation diffgramParent,
      XmlDiffElement targetElement)
    {
      for (XmlDiffAttributeOrNamespace attributeOrNamespace = targetElement._attributes; attributeOrNamespace != null; attributeOrNamespace = (XmlDiffAttributeOrNamespace) attributeOrNamespace._nextSibling)
        diffgramParent.InsertAtBeginning((DiffgramOperation) new DiffgramAddNode((XmlDiffNode) attributeOrNamespace, 0UL));
    }

    private DiffgramOperation GenerateDiffgramRemoveWhenDescendantMatches(
      XmlDiffNode sourceParent)
    {
      DiffgramParentOperation diffgramParentOperation = (DiffgramParentOperation) new DiffgramRemoveNode(sourceParent, false, 0UL);
      for (XmlDiffNode xmlDiffNode = ((XmlDiffParentNode) sourceParent)._firstChildNode; xmlDiffNode != null; xmlDiffNode = xmlDiffNode._nextSibling)
      {
        if (xmlDiffNode.NodeType == XmlDiffNodeType.ShrankNode)
        {
          XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode) xmlDiffNode;
          if (xmlDiffShrankNode.MoveOperationId == 0UL)
            xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
          diffgramParentOperation.InsertAtEnd((DiffgramOperation) new DiffgramRemoveSubtrees(xmlDiffNode, xmlDiffShrankNode.MoveOperationId, !this._xmlDiff.IgnoreChildOrder));
        }
        else if (xmlDiffNode.HasChildNodes && xmlDiffNode._bSomeDescendantMatches)
          diffgramParentOperation.InsertAtEnd(this.GenerateDiffgramRemoveWhenDescendantMatches(xmlDiffNode));
        else if (!diffgramParentOperation.MergeRemoveSubtreeAtEnd(xmlDiffNode))
          diffgramParentOperation.InsertAtEnd((DiffgramOperation) new DiffgramRemoveSubtrees(xmlDiffNode, 0UL, !this._xmlDiff.IgnoreChildOrder));
      }
      return (DiffgramOperation) diffgramParentOperation;
    }

    private DiffgramOperation GenerateDiffgramAddWhenDescendantMatches(
      XmlDiffNode targetParent)
    {
      DiffgramParentOperation diffgramParentOperation = (DiffgramParentOperation) new DiffgramAddNode(targetParent, 0UL);
      if (targetParent.NodeType == XmlDiffNodeType.Element)
      {
        for (XmlDiffAttributeOrNamespace attributeOrNamespace = ((XmlDiffElement) targetParent)._attributes; attributeOrNamespace != null; attributeOrNamespace = (XmlDiffAttributeOrNamespace) attributeOrNamespace._nextSibling)
          diffgramParentOperation.InsertAtEnd((DiffgramOperation) new DiffgramAddNode((XmlDiffNode) attributeOrNamespace, 0UL));
      }
      for (XmlDiffNode xmlDiffNode = ((XmlDiffParentNode) targetParent)._firstChildNode; xmlDiffNode != null; xmlDiffNode = xmlDiffNode._nextSibling)
      {
        if (xmlDiffNode.NodeType == XmlDiffNodeType.ShrankNode)
        {
          XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode) xmlDiffNode;
          if (xmlDiffShrankNode.MoveOperationId == 0UL)
            xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
          diffgramParentOperation.InsertAtEnd((DiffgramOperation) new DiffgramCopy((XmlDiffNode) xmlDiffShrankNode.MatchingShrankNode, true, xmlDiffShrankNode.MoveOperationId));
        }
        else if (xmlDiffNode.HasChildNodes && xmlDiffNode._bSomeDescendantMatches)
          diffgramParentOperation.InsertAtEnd(this.GenerateDiffgramAddWhenDescendantMatches(xmlDiffNode));
        else if (!diffgramParentOperation.MergeAddSubtreeAtEnd(xmlDiffNode))
          diffgramParentOperation.InsertAtEnd((DiffgramOperation) new DiffgramAddSubtrees(xmlDiffNode, 0UL, !this._xmlDiff.IgnoreChildOrder));
      }
      return (DiffgramOperation) diffgramParentOperation;
    }

    private void OnEditScriptPostponed(DiffgramParentOperation parent, int targetBorderIndex)
    {
      EditScriptPostponed editScript = (EditScriptPostponed) this._editScript;
      DiffgramOperation diffOperation = editScript._diffOperation;
      int startSourceIndex = editScript._startSourceIndex;
      int left = this._sourceNodes[editScript._endSourceIndex].Left;
      this._curSourceIndex = editScript._startSourceIndex - 1;
      this._editScript = editScript._nextEditScript;
      if (startSourceIndex > left)
        this.GenerateDiffgramPostponed((DiffgramParentOperation) diffOperation, ref this._editScript, left, targetBorderIndex);
      parent.InsertAtBeginning(diffOperation);
    }

    internal Diffgram GenerateFromWalkTree()
    {
      Diffgram diffgram = new Diffgram(this._xmlDiff);
      this.WalkTreeGenerateDiffgramMatch((DiffgramParentOperation) diffgram, (XmlDiffParentNode) this._xmlDiff._sourceDoc, (XmlDiffParentNode) this._xmlDiff._targetDoc);
      this.AppendDescriptors(diffgram);
      return diffgram;
    }

    private void WalkTreeGenerateDiffgramMatch(
      DiffgramParentOperation diffParent,
      XmlDiffParentNode sourceParent,
      XmlDiffParentNode targetParent)
    {
      XmlDiffNode xmlDiffNode1 = sourceParent.FirstChildNode;
      XmlDiffNode xmlDiffNode2 = targetParent.FirstChildNode;
      XmlDiffNode needPositionSourceNode = (XmlDiffNode) null;
      while (xmlDiffNode1 != null || xmlDiffNode2 != null)
      {
        if (xmlDiffNode1 != null)
        {
          if (xmlDiffNode2 != null)
          {
            XmlDiffOperation op = xmlDiffNode1.GetDiffOperation(xmlDiffNode2, this._xmlDiff);
            if (op == XmlDiffOperation.Match)
            {
              this.WalkTreeOnMatchNode(diffParent, xmlDiffNode1, xmlDiffNode2, ref needPositionSourceNode);
            }
            else
            {
              int num1 = (int) (4.0 * Math.Log((double) ((xmlDiffNode1._parent.ChildNodesCount + xmlDiffNode2._parent.ChildNodesCount) / 2)) + 1.0);
              XmlDiffNode xmlDiffNode3 = xmlDiffNode2;
              XmlDiffNode xmlDiffNode4 = xmlDiffNode1;
              XmlDiffOperation xmlDiffOperation1 = op;
              XmlDiffOperation xmlDiffOperation2 = op;
              int nodesCount1 = xmlDiffNode2.NodesCount;
              int nodesCount2 = xmlDiffNode1.NodesCount;
              XmlDiffNode xmlDiffNode5 = xmlDiffNode1._nextSibling;
              XmlDiffNode xmlDiffNode6 = xmlDiffNode2._nextSibling;
              for (int index = 0; index < num1 && (xmlDiffNode6 != null || xmlDiffNode5 != null); ++index)
              {
                if (xmlDiffNode6 != null && xmlDiffOperation1 != XmlDiffOperation.Match)
                {
                  XmlDiffOperation diffOperation = xmlDiffNode1.GetDiffOperation(xmlDiffNode6, this._xmlDiff);
                  if (MinimalTreeDistanceAlgo.OperationCost[(int) diffOperation] < MinimalTreeDistanceAlgo.OperationCost[(int) xmlDiffOperation1])
                  {
                    xmlDiffOperation1 = diffOperation;
                    xmlDiffNode3 = xmlDiffNode6;
                  }
                  else
                  {
                    nodesCount1 += xmlDiffNode6.NodesCount;
                    xmlDiffNode6 = xmlDiffNode6._nextSibling;
                  }
                }
                if (xmlDiffNode5 != null && xmlDiffOperation2 != XmlDiffOperation.Match)
                {
                  XmlDiffOperation diffOperation = xmlDiffNode2.GetDiffOperation(xmlDiffNode5, this._xmlDiff);
                  if (MinimalTreeDistanceAlgo.OperationCost[(int) diffOperation] < MinimalTreeDistanceAlgo.OperationCost[(int) xmlDiffOperation2])
                  {
                    xmlDiffOperation2 = diffOperation;
                    xmlDiffNode4 = xmlDiffNode5;
                  }
                  else
                  {
                    nodesCount2 += xmlDiffNode5.NodesCount;
                    xmlDiffNode5 = xmlDiffNode5._nextSibling;
                  }
                }
                if (xmlDiffOperation1 != XmlDiffOperation.Match && xmlDiffOperation2 != XmlDiffOperation.Match)
                {
                  if (this._xmlDiff.IgnoreChildOrder)
                  {
                    if (xmlDiffNode6 != null && XmlDiffDocument.OrderChildren(xmlDiffNode1, xmlDiffNode6) < 0)
                      xmlDiffNode6 = (XmlDiffNode) null;
                    if (xmlDiffNode5 != null && XmlDiffDocument.OrderChildren(xmlDiffNode2, xmlDiffNode5) < 0)
                      xmlDiffNode5 = (XmlDiffNode) null;
                  }
                }
                else
                  break;
              }
              if (xmlDiffOperation1 == XmlDiffOperation.Match)
              {
                if (xmlDiffOperation2 != XmlDiffOperation.Match || nodesCount1 < nodesCount2)
                {
                  for (; xmlDiffNode2 != xmlDiffNode3; xmlDiffNode2 = xmlDiffNode2._nextSibling)
                  {
                    this.WalkTreeOnAddNode(diffParent, xmlDiffNode2, needPositionSourceNode);
                    needPositionSourceNode = (XmlDiffNode) null;
                  }
                  this.WalkTreeOnMatchNode(diffParent, xmlDiffNode1, xmlDiffNode2, ref needPositionSourceNode);
                  goto label_44;
                }
              }
              else if (xmlDiffOperation2 == XmlDiffOperation.Match)
              {
                for (; xmlDiffNode1 != xmlDiffNode4; xmlDiffNode1 = xmlDiffNode1._nextSibling)
                  this.WalkTreeOnRemoveNode(diffParent, xmlDiffNode1);
                needPositionSourceNode = (XmlDiffNode) null;
                this.WalkTreeOnMatchNode(diffParent, xmlDiffNode1, xmlDiffNode2, ref needPositionSourceNode);
                goto label_44;
              }
              else
              {
                int num2 = MinimalTreeDistanceAlgo.OperationCost[(int) xmlDiffOperation1];
                int num3 = MinimalTreeDistanceAlgo.OperationCost[(int) xmlDiffOperation2];
                if (num2 < num3 || num2 == num3 && nodesCount1 < nodesCount2)
                {
                  for (; xmlDiffNode2 != xmlDiffNode3; xmlDiffNode2 = xmlDiffNode2._nextSibling)
                  {
                    this.WalkTreeOnAddNode(diffParent, xmlDiffNode2, needPositionSourceNode);
                    needPositionSourceNode = (XmlDiffNode) null;
                  }
                  op = xmlDiffOperation1;
                }
                else
                {
                  for (; xmlDiffNode1 != xmlDiffNode4; xmlDiffNode1 = xmlDiffNode1._nextSibling)
                    this.WalkTreeOnRemoveNode(diffParent, xmlDiffNode1);
                  op = xmlDiffOperation2;
                }
              }
              switch (op)
              {
                case XmlDiffOperation.ChangeElementName:
                  this.WalkTreeOnChangeElement(diffParent, (XmlDiffElement) xmlDiffNode1, (XmlDiffElement) xmlDiffNode2, op);
                  needPositionSourceNode = (XmlDiffNode) null;
                  break;
                case XmlDiffOperation.ChangeElementAttr1:
                case XmlDiffOperation.ChangeElementAttr2:
                case XmlDiffOperation.ChangeElementAttr3:
                case XmlDiffOperation.ChangeElementNameAndAttr1:
                case XmlDiffOperation.ChangeElementNameAndAttr2:
                case XmlDiffOperation.ChangeElementNameAndAttr3:
                  if (this.GoForElementChange((XmlDiffElement) xmlDiffNode1, (XmlDiffElement) xmlDiffNode2))
                  {
                    this.WalkTreeOnChangeElement(diffParent, (XmlDiffElement) xmlDiffNode1, (XmlDiffElement) xmlDiffNode2, op);
                    needPositionSourceNode = (XmlDiffNode) null;
                    break;
                  }
                  goto case XmlDiffOperation.Undefined;
                case XmlDiffOperation.ChangePI:
                case XmlDiffOperation.ChangeER:
                case XmlDiffOperation.ChangeCharacterData:
                case XmlDiffOperation.ChangeXmlDeclaration:
                case XmlDiffOperation.ChangeDTD:
                  diffParent.InsertAtEnd((DiffgramOperation) new DiffgramChangeNode(xmlDiffNode1, xmlDiffNode2, op, 0UL));
                  needPositionSourceNode = (XmlDiffNode) null;
                  break;
                case XmlDiffOperation.Undefined:
                  this.WalkTreeOnAddNode(diffParent, xmlDiffNode2, needPositionSourceNode);
                  needPositionSourceNode = (XmlDiffNode) null;
                  xmlDiffNode2 = xmlDiffNode2._nextSibling;
                  continue;
              }
            }
label_44:
            xmlDiffNode1 = xmlDiffNode1._nextSibling;
            xmlDiffNode2 = xmlDiffNode2._nextSibling;
          }
          else
          {
            do
            {
              this.WalkTreeOnRemoveNode(diffParent, xmlDiffNode1);
              xmlDiffNode1 = xmlDiffNode1._nextSibling;
            }
            while (xmlDiffNode1 != null);
          }
        }
        else
        {
          for (; xmlDiffNode2 != null; xmlDiffNode2 = xmlDiffNode2._nextSibling)
          {
            this.WalkTreeOnAddNode(diffParent, xmlDiffNode2, needPositionSourceNode);
            needPositionSourceNode = (XmlDiffNode) null;
          }
        }
      }
    }

    private bool GoForElementChange(XmlDiffElement sourceElement, XmlDiffElement targetElement)
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      int num4 = 0;
      bool flag = sourceElement.LocalName != targetElement.LocalName;
      XmlDiffAttributeOrNamespace node1 = sourceElement._attributes;
      XmlDiffAttributeOrNamespace node2 = targetElement._attributes;
      while (node1 != null && node2 != null)
      {
        if (node1.LocalName == node2.LocalName)
        {
          if ((this._xmlDiff.IgnorePrefixes || this._xmlDiff.IgnoreNamespaces || node1.Prefix == node2.Prefix) && (this._xmlDiff.IgnoreNamespaces || node1.NamespaceURI == node2.NamespaceURI))
          {
            if (node1.Value == node2.Value)
              ++num1;
            else
              ++num4;
          }
          else
            ++num4;
          node1 = (XmlDiffAttributeOrNamespace) node1._nextSibling;
          node2 = (XmlDiffAttributeOrNamespace) node2._nextSibling;
        }
        else if (XmlDiffDocument.OrderAttributesOrNamespaces(node1, node2) < 0)
        {
          ++num3;
          node1 = (XmlDiffAttributeOrNamespace) node1._nextSibling;
        }
        else
        {
          ++num2;
          node2 = (XmlDiffAttributeOrNamespace) node2._nextSibling;
        }
      }
      for (; node1 != null; node1 = (XmlDiffAttributeOrNamespace) node1._nextSibling)
        ++num3;
      for (; node2 != null; node2 = (XmlDiffAttributeOrNamespace) node2._nextSibling)
        ++num2;
      return flag ? num3 + num2 + num4 <= num1 : num3 + num4 == 0 || num2 + num4 == 0 || (num3 + num2 == 0 || num3 + num2 + num4 <= num1 * 3) || sourceElement._nextSibling == null;
    }

    private void WalkTreeOnRemoveNode(DiffgramParentOperation diffParent, XmlDiffNode sourceNode)
    {
      bool flag = sourceNode is XmlDiffShrankNode;
      if (sourceNode._bSomeDescendantMatches && !flag)
      {
        DiffgramOperation descendantMatches = this.GenerateDiffgramRemoveWhenDescendantMatches(sourceNode);
        diffParent.InsertAtEnd(descendantMatches);
      }
      else
      {
        ulong operationID = 0;
        if (flag)
        {
          XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode) sourceNode;
          if (xmlDiffShrankNode.MoveOperationId == 0UL)
            xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
          operationID = xmlDiffShrankNode.MoveOperationId;
        }
        if (operationID == 0UL && diffParent.MergeRemoveSubtreeAtEnd(sourceNode))
          return;
        diffParent.InsertAtEnd((DiffgramOperation) new DiffgramRemoveSubtrees(sourceNode, operationID, !this._xmlDiff.IgnoreChildOrder));
      }
    }

    private void WalkTreeOnAddNode(
      DiffgramParentOperation diffParent,
      XmlDiffNode targetNode,
      XmlDiffNode sourcePositionNode)
    {
      bool flag = targetNode is XmlDiffShrankNode;
      if (this._bChildOrderSignificant)
      {
        if (sourcePositionNode != null)
          diffParent.InsertAtEnd((DiffgramOperation) new DiffgramPosition(sourcePositionNode));
      }
      else if (diffParent._firstChildOp == null && diffParent is Diffgram)
        diffParent.InsertAtEnd((DiffgramOperation) new DiffgramPosition(sourcePositionNode));
      if (targetNode._bSomeDescendantMatches && !flag)
      {
        DiffgramOperation descendantMatches = this.GenerateDiffgramAddWhenDescendantMatches(targetNode);
        diffParent.InsertAtEnd(descendantMatches);
      }
      else if (flag)
      {
        XmlDiffShrankNode xmlDiffShrankNode = (XmlDiffShrankNode) targetNode;
        if (xmlDiffShrankNode.MoveOperationId == 0UL)
          xmlDiffShrankNode.MoveOperationId = this.GenerateOperationID(XmlDiffDescriptorType.Move);
        ulong moveOperationId = xmlDiffShrankNode.MoveOperationId;
        diffParent.InsertAtEnd((DiffgramOperation) new DiffgramCopy((XmlDiffNode) xmlDiffShrankNode.MatchingShrankNode, true, moveOperationId));
      }
      else
      {
        switch (targetNode.NodeType)
        {
          case XmlDiffNodeType.XmlDeclaration:
          case XmlDiffNodeType.DocumentType:
          case XmlDiffNodeType.EntityReference:
            diffParent.InsertAtEnd((DiffgramOperation) new DiffgramAddNode(targetNode, 0UL));
            break;
          default:
            if (diffParent.MergeAddSubtreeAtEnd(targetNode))
              break;
            diffParent.InsertAtEnd((DiffgramOperation) new DiffgramAddSubtrees(targetNode, 0UL, !this._xmlDiff.IgnoreChildOrder));
            break;
        }
      }
    }

    private void WalkTreeOnChangeNode(
      DiffgramParentOperation diffParent,
      XmlDiffNode sourceNode,
      XmlDiffNode targetNode,
      XmlDiffOperation op)
    {
      DiffgramChangeNode diffgramChangeNode = new DiffgramChangeNode(sourceNode, targetNode, op, 0UL);
      if (sourceNode.HasChildNodes || targetNode.HasChildNodes)
        this.WalkTreeGenerateDiffgramMatch((DiffgramParentOperation) diffgramChangeNode, (XmlDiffParentNode) sourceNode, (XmlDiffParentNode) targetNode);
      diffParent.InsertAtEnd((DiffgramOperation) diffgramChangeNode);
    }

    private void WalkTreeOnChangeElement(
      DiffgramParentOperation diffParent,
      XmlDiffElement sourceElement,
      XmlDiffElement targetElement,
      XmlDiffOperation op)
    {
      DiffgramParentOperation diffgramParentOperation;
      if (XmlDiff.IsChangeOperationOnAttributesOnly(op))
      {
        diffgramParentOperation = (DiffgramParentOperation) new DiffgramPosition((XmlDiffNode) sourceElement);
      }
      else
      {
        ulong operationID = 0;
        if (!this._xmlDiff.IgnoreNamespaces && sourceElement.LocalName == targetElement.LocalName)
          operationID = this.GetNamespaceChangeOpid(sourceElement.NamespaceURI, sourceElement.Prefix, targetElement.NamespaceURI, targetElement.Prefix);
        diffgramParentOperation = (DiffgramParentOperation) new DiffgramChangeNode((XmlDiffNode) sourceElement, (XmlDiffNode) targetElement, XmlDiffOperation.ChangeElementName, operationID);
      }
      this.GenerateChangeDiffgramForAttributes(diffgramParentOperation, sourceElement, targetElement);
      if (sourceElement.HasChildNodes || targetElement.HasChildNodes)
        this.WalkTreeGenerateDiffgramMatch(diffgramParentOperation, (XmlDiffParentNode) sourceElement, (XmlDiffParentNode) targetElement);
      diffParent.InsertAtEnd((DiffgramOperation) diffgramParentOperation);
    }

    private void WalkTreeOnMatchNode(
      DiffgramParentOperation diffParent,
      XmlDiffNode sourceNode,
      XmlDiffNode targetNode,
      ref XmlDiffNode needPositionSourceNode)
    {
      if (sourceNode.HasChildNodes || targetNode.HasChildNodes)
      {
        DiffgramPosition diffgramPosition = new DiffgramPosition(sourceNode);
        this.WalkTreeGenerateDiffgramMatch((DiffgramParentOperation) diffgramPosition, (XmlDiffParentNode) sourceNode, (XmlDiffParentNode) targetNode);
        diffParent.InsertAtEnd((DiffgramOperation) diffgramPosition);
        needPositionSourceNode = (XmlDiffNode) null;
      }
      else if (sourceNode.NodeType == XmlDiffNodeType.ShrankNode)
        needPositionSourceNode = ((XmlDiffShrankNode) sourceNode)._lastNode;
      else
        needPositionSourceNode = sourceNode;
    }

    internal struct PostponedEditScriptInfo
    {
      internal EditScriptPostponed _firstES;
      internal EditScriptPostponed _lastES;
      internal int _startSourceIndex;
      internal int _endSourceIndex;

      internal void Reset()
      {
        this._firstES = (EditScriptPostponed) null;
        this._lastES = (EditScriptPostponed) null;
        this._startSourceIndex = 0;
        this._endSourceIndex = 0;
      }
    }

    internal class PrefixChange
    {
      internal string _oldPrefix;
      internal string _newPrefix;
      internal string _NS;
      internal ulong _opid;
      internal DiffgramGenerator.PrefixChange _next;

      internal PrefixChange(
        string oldPrefix,
        string newPrefix,
        string ns,
        ulong opid,
        DiffgramGenerator.PrefixChange next)
      {
        this._oldPrefix = oldPrefix;
        this._newPrefix = newPrefix;
        this._NS = ns;
        this._opid = opid;
        this._next = next;
      }
    }

    internal class NamespaceChange
    {
      internal string _prefix;
      internal string _oldNS;
      internal string _newNS;
      internal ulong _opid;
      internal DiffgramGenerator.NamespaceChange _next;

      internal NamespaceChange(
        string prefix,
        string oldNamespace,
        string newNamespace,
        ulong opid,
        DiffgramGenerator.NamespaceChange next)
      {
        this._prefix = prefix;
        this._oldNS = oldNamespace;
        this._newNS = newNamespace;
        this._opid = opid;
        this._next = next;
      }
    }
  }
}
