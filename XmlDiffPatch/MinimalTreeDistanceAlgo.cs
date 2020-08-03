// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.MinimalTreeDistanceAlgo
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
    internal class MinimalTreeDistanceAlgo
    {
        private static readonly EditScriptEmpty EmptyEditScript = new EditScriptEmpty();
        internal static readonly int[] OperationCost = new int[16]
        {
      0,
      4,
      4,
      1,
      1,
      2,
      3,
      2,
      3,
      4,
      4,
      4,
      4,
      4,
      4,
      1073741823
        };
        private readonly XmlDiff _xmlDiff;
        private XmlDiffNode[] _sourceNodes;
        private XmlDiffNode[] _targetNodes;
        private Distance[,] _treeDist;
        private Distance[,] _forestDist;

        internal MinimalTreeDistanceAlgo(XmlDiff xmlDiff)
        {
            this._xmlDiff = xmlDiff;
        }

        internal EditScript FindMinimalDistance()
        {
            EditScript es = null;
            try
            {
                this._sourceNodes = this._xmlDiff._sourceNodes;
                this._targetNodes = this._xmlDiff._targetNodes;
                this._treeDist = new Distance[(int)checked((uint)this._sourceNodes.Length), (int)checked((uint)this._targetNodes.Length)];
                this._forestDist = new Distance[(int)checked((uint)this._sourceNodes.Length), (int)checked((uint)this._targetNodes.Length)];
                for (var sourcePos = 1; sourcePos < this._sourceNodes.Length; ++sourcePos)
                {
                    if (this._sourceNodes[sourcePos].IsKeyRoot)
                    {
                        for (var targetPos = 1; targetPos < this._targetNodes.Length; ++targetPos)
                        {
                            if (this._targetNodes[targetPos].IsKeyRoot)
                                this.ComputeTreeDistance(sourcePos, targetPos);
                        }
                    }
                }
                es = this._treeDist[this._sourceNodes.Length - 1, this._targetNodes.Length - 1]._editScript;
            }
            finally
            {
                this._forestDist = null;
                this._treeDist = null;
                this._sourceNodes = null;
                this._targetNodes = null;
            }
            return MinimalTreeDistanceAlgo.NormalizeScript(es);
        }

        private void ComputeTreeDistance(int sourcePos, int targetPos)
        {
            var left1 = this._sourceNodes[sourcePos].Left;
            var left2 = this._targetNodes[targetPos].Left;
            var editScriptAddOpened = new EditScriptAddOpened(left2, EmptyEditScript);
            var scriptRemoveOpened = new EditScriptRemoveOpened(left1, EmptyEditScript);
            this._forestDist[left1 - 1, left2 - 1]._cost = 0;
            this._forestDist[left1 - 1, left2 - 1]._editScript = EmptyEditScript;
            for (var index = left1; index <= sourcePos; ++index)
            {
                this._forestDist[index, left2 - 1]._cost = ( index - left1 + 1 ) * MinimalTreeDistanceAlgo.OperationCost[2];
                this._forestDist[index, left2 - 1]._editScript = scriptRemoveOpened;
            }
            for (var index = left2; index <= targetPos; ++index)
            {
                this._forestDist[left1 - 1, index]._cost = ( index - left2 + 1 ) * MinimalTreeDistanceAlgo.OperationCost[1];
                this._forestDist[left1 - 1, index]._editScript = editScriptAddOpened;
            }
            for (var index1 = left1; index1 <= sourcePos; ++index1)
            {
                for (var index2 = left2; index2 <= targetPos; ++index2)
                {
                    var left3 = this._sourceNodes[index1].Left;
                    var left4 = this._targetNodes[index2].Left;
                    var cost1 = this._forestDist[index1 - 1, index2]._cost + MinimalTreeDistanceAlgo.OperationCost[2];
                    var cost2 = this._forestDist[index1, index2 - 1]._cost + MinimalTreeDistanceAlgo.OperationCost[1];
                    if (left3 == left1 && left4 == left2)
                    {
                        var diffOperation = this._sourceNodes[index1].GetDiffOperation(this._targetNodes[index2], this._xmlDiff);
                        if (diffOperation == XmlDiffOperation.Match)
                        {
                            this.OpNodesMatch(index1, index2);
                        }
                        else
                        {
                            var cost3 = this._forestDist[index1 - 1, index2 - 1]._cost + MinimalTreeDistanceAlgo.OperationCost[(int)diffOperation];
                            if (cost3 < cost2)
                            {
                                if (cost3 < cost1)
                                    this.OpChange(index1, index2, diffOperation, cost3);
                                else
                                    this.OpRemove(index1, index2, cost1);
                            }
                            else if (cost2 < cost1)
                                this.OpAdd(index1, index2, cost2);
                            else
                                this.OpRemove(index1, index2, cost1);
                        }
                        this._treeDist[index1, index2]._cost = this._forestDist[index1, index2]._cost;
                        this._treeDist[index1, index2]._editScript = this._forestDist[index1, index2]._editScript.GetClosedScript(index1, index2);
                    }
                    else
                    {
                        var m = left3 - 1;
                        var n = left4 - 1;
                        if (m < left1 - 1)
                            m = left1 - 1;
                        if (n < left2 - 1)
                            n = left2 - 1;
                        var num = this._forestDist[m, n]._cost + this._treeDist[index1, index2]._cost;
                        if (num < cost2)
                        {
                            if (num < cost1)
                            {
                                if (this._treeDist[index1, index2]._editScript == MinimalTreeDistanceAlgo.EmptyEditScript)
                                    this.OpCopyScript(index1, index2, m, n);
                                else
                                    this.OpConcatScripts(index1, index2, m, n);
                            }
                            else
                                this.OpRemove(index1, index2, cost1);
                        }
                        else if (cost2 < cost1)
                            this.OpAdd(index1, index2, cost2);
                        else
                            this.OpRemove(index1, index2, cost1);
                    }
                }
            }
        }

        private void OpChange(int i, int j, XmlDiffOperation changeOp, int cost)
        {
            this._forestDist[i, j]._editScript = new EditScriptChange(i, j, changeOp, this._forestDist[i - 1, j - 1]._editScript.GetClosedScript(i - 1, j - 1));
            this._forestDist[i, j]._cost = cost;
        }

        private void OpAdd(int i, int j, int cost)
        {
            var editScriptAddOpened = this._forestDist[i, j - 1]._editScript as EditScriptAddOpened;
            if (!( editScriptAddOpened is EditScriptAddOpened ))
                editScriptAddOpened = new EditScriptAddOpened(j, this._forestDist[i, j - 1]._editScript.GetClosedScript(i, j - 1));
            this._forestDist[i, j]._editScript = editScriptAddOpened;
            this._forestDist[i, j]._cost = cost;
        }

        private void OpRemove(int i, int j, int cost)
        {
            var scriptRemoveOpened = this._forestDist[i - 1, j]._editScript as EditScriptRemoveOpened;
            if (!( scriptRemoveOpened is EditScriptRemoveOpened  ))
                scriptRemoveOpened = new EditScriptRemoveOpened(i, this._forestDist[i - 1, j]._editScript.GetClosedScript(i - 1, j));
            this._forestDist[i, j]._editScript = scriptRemoveOpened;
            this._forestDist[i, j]._cost = cost;
        }

        private void OpNodesMatch(int i, int j)
        {
            var scriptMatchOpened = this._forestDist[i - 1, j - 1]._editScript as EditScriptMatchOpened;
            if (!( scriptMatchOpened is EditScriptMatchOpened ))
                scriptMatchOpened = new EditScriptMatchOpened(i, j, this._forestDist[i - 1, j - 1]._editScript.GetClosedScript(i - 1, j - 1));
            this._forestDist[i, j]._editScript = scriptMatchOpened;
            this._forestDist[i, j]._cost = this._forestDist[i - 1, j - 1]._cost;
        }

        private void OpCopyScript(int i, int j, int m, int n)
        {
            this._forestDist[i, j]._cost = this._forestDist[m, n]._cost;
            this._forestDist[i, j]._editScript = this._forestDist[m, n]._editScript.GetClosedScript(m, n);
        }

        private void OpConcatScripts(int i, int j, int m, int n)
        {
            this._forestDist[i, j]._editScript = new EditScriptReference(this._treeDist[i, j]._editScript, this._forestDist[m, n]._editScript.GetClosedScript(m, n));
            this._forestDist[i, j]._cost = this._treeDist[i, j]._cost + this._forestDist[m, n]._cost;
        }

        private static EditScript NormalizeScript(EditScript es)
        {
            var editScript1 = es;
            var editScript2 = es;
            EditScript editScript3 = null;
            while (editScript2 != MinimalTreeDistanceAlgo.EmptyEditScript)
            {
                if (editScript2.Operation != EditScriptOperation.EditScriptReference)
                {
                    editScript3 = editScript2;
                    editScript2 = editScript2._nextEditScript;
                }
                else
                {
                    var editScriptReference = editScript2 as EditScriptReference;
                    var editScript4 = editScriptReference._editScriptReference;
                    while (editScript4.Next != MinimalTreeDistanceAlgo.EmptyEditScript)
                        editScript4 = editScript4._nextEditScript;
                    editScript4._nextEditScript = editScript2._nextEditScript;
                    editScript2 = editScriptReference._editScriptReference;
                    if (editScript3 == null)
                        editScript1 = editScript2;
                    else
                        editScript3._nextEditScript = editScript2;
                }
            }
            if (editScript3 != null)
                editScript3._nextEditScript = null;
            else
                editScript1 = null;
            return editScript1;
        }

        private struct Distance
        {
            internal int _cost;
            internal EditScript _editScript;
        }
    }
}
