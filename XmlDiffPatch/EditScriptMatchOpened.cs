// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.EditScriptMatchOpened
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
  internal class EditScriptMatchOpened : EditScriptOpened
  {
    internal int _startSourceIndex;
    internal int _startTargetIndex;

    internal EditScriptMatchOpened(int startSourceIndex, int startTargetIndex, EditScript next)
      : base(next)
    {
      this._startSourceIndex = startSourceIndex;
      this._startTargetIndex = startTargetIndex;
    }

    internal override EditScriptOperation Operation
    {
      get
      {
        return EditScriptOperation.OpenedMatch;
      }
    }

    internal override EditScript GetClosedScript(
      int currentSourceIndex,
      int currentTargetIndex)
    {
      return (EditScript) new EditScriptMatch(this._startSourceIndex, this._startTargetIndex, currentSourceIndex - this._startSourceIndex + 1, this._nextEditScript);
    }
  }
}
