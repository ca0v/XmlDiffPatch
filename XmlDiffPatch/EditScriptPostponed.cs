﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.EditScriptPostponed
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

namespace Microsoft.XmlDiffPatch
{
  internal class EditScriptPostponed : EditScript
  {
    internal DiffgramOperation _diffOperation;
    internal int _startSourceIndex;
    internal int _endSourceIndex;

    internal EditScriptPostponed(
      DiffgramOperation diffOperation,
      int startSourceIndex,
      int endSourceIndex)
      : base((EditScript) null)
    {
      this._diffOperation = diffOperation;
      this._startSourceIndex = startSourceIndex;
      this._endSourceIndex = endSourceIndex;
    }

    internal override EditScriptOperation Operation
    {
      get
      {
        return EditScriptOperation.EditScriptPostponed;
      }
    }
  }
}
