﻿// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlPatchError
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlPatchError
  {
    internal const string InvalidPathDescriptor = "Invalid XDL diffgram. '{0}' is an invalid path descriptor.";
    internal const string NoMatchingNode = "Invalid XDL diffgram. No node matches the path descriptor '{0}'.";
    internal const string ExpectingDiffgramElement = "Invalid XDL diffgram. Expecting xd:xmldiff as a root element with namespace URI 'http://schemas.microsoft.com/xmltools/2002/xmldiff'.";
    internal const string MissingSrcDocAttribute = "Invalid XDL diffgram. Missing srcDocHash attribute on the xd:xmldiff element.";
    internal const string MissingOptionsAttribute = "Invalid XDL diffgram. Missing options attribute on the xd:xmldiff element.";
    internal const string InvalidSrcDocAttribute = "Invalid XDL diffgram. The srcDocHash attribute has an invalid value.";
    internal const string InvalidOptionsAttribute = "Invalid XDL diffgram. The options attribute has an invalid value.";
    internal const string SrcDocMismatch = "The XDL diffgram is not applicable to this XML document; the srcDocHash value does not match.";
    internal const string MoreThanOneNodeMatched = "Invalid XDL diffgram; more than one node matches the '{0}' path descriptor on the xd:node or xd:change element.";
    internal const string XmlDeclMismatch = "The diffgram is not applicable to this XML document; cannot add a new xml declaration.";
    internal const string InternalErrorMoreThanOneNodeInList = "Internal Error. XmlDiffPathSingleNodeList can contain one node only.";
    internal const string InternalErrorMoreThanOneNodeLeft = "Internal Error. {0} nodes left after patch, expecting 1.";

    internal static void Error(string message)
    {
      throw new Exception(message);
    }

    internal static void Error(string message, string arg1)
    {
      XmlPatchError.Error(string.Format(message, (object) arg1));
    }
  }
}
