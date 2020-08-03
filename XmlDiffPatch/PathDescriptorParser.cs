// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.PathDescriptorParser
// Assembly: XmlDiffPatch, Version=1.0.8.28, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2B32D548-922A-4A84-B9AD-FF8E573DAC90
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.dll

using System.Collections;
using System.Xml;

namespace Microsoft.XmlDiffPatch
{
  internal class PathDescriptorParser
  {
    private static char[] Delimiters = new char[3]
    {
      '|',
      '-',
      '/'
    };
    private static char[] MultiNodesDelimiters = new char[2]
    {
      '|',
      '-'
    };

    internal static XmlNodeList SelectNodes(
      XmlNode rootNode,
      XmlNode currentParentNode,
      string pathDescriptor)
    {
      switch (pathDescriptor[0])
      {
        case '*':
          if (pathDescriptor.Length == 1)
            return PathDescriptorParser.SelectAllChildren(currentParentNode);
          PathDescriptorParser.OnInvalidExpression(pathDescriptor);
          return (XmlNodeList) null;
        case '/':
          return PathDescriptorParser.SelectAbsoluteNodes(rootNode, pathDescriptor);
        case '@':
          if (pathDescriptor.Length < 2)
            PathDescriptorParser.OnInvalidExpression(pathDescriptor);
          return pathDescriptor[1] == '*' ? PathDescriptorParser.SelectAllAttributes(currentParentNode) : PathDescriptorParser.SelectAttributes(currentParentNode, pathDescriptor);
        default:
          return PathDescriptorParser.SelectChildNodes(currentParentNode, pathDescriptor, 0);
      }
    }

    private static XmlNodeList SelectAbsoluteNodes(XmlNode rootNode, string path)
    {
      var pos = 1;
            var xmlNode = rootNode;
      int startPos;
      while (true)
      {
        startPos = pos;
                var childNodes = xmlNode.ChildNodes;
        var num = PathDescriptorParser.ReadPosition(path, ref pos);
        if (pos == path.Length || path[pos] == '/')
        {
          if (childNodes.Count == 0 || num < 0 || num > childNodes.Count)
            PathDescriptorParser.OnNoMatchingNode(path);
          xmlNode = childNodes.Item(num - 1);
          if (pos != path.Length)
            ++pos;
          else
            break;
        }
        else if (path[pos] != '-' && path[pos] != '|')
          PathDescriptorParser.OnInvalidExpression(path);
        else
          goto label_8;
      }
            var xmlPatchNodeList = (XmlPatchNodeList) new SingleNodeList();
      xmlPatchNodeList.AddNode(xmlNode);
      return (XmlNodeList) xmlPatchNodeList;
label_8:
      return PathDescriptorParser.SelectChildNodes(xmlNode, path, startPos);
    }

    private static XmlNodeList SelectAllAttributes(XmlNode parentNode)
    {
            var attributes = parentNode.Attributes;
      if (attributes.Count == 0)
      {
        PathDescriptorParser.OnNoMatchingNode("@*");
        return (XmlNodeList) null;
      }
      if (attributes.Count == 1)
      {
                var xmlPatchNodeList = (XmlPatchNodeList) new SingleNodeList();
        xmlPatchNodeList.AddNode(attributes.Item(0));
        return (XmlNodeList) xmlPatchNodeList;
      }
            var enumerator = attributes.GetEnumerator();
            var xmlPatchNodeList1 = (XmlPatchNodeList) new MultiNodeList();
      while (enumerator.MoveNext())
        xmlPatchNodeList1.AddNode((XmlNode) enumerator.Current);
      return (XmlNodeList) xmlPatchNodeList1;
    }

    private static XmlNodeList SelectAttributes(XmlNode parentNode, string path)
    {
      var pos = 1;
            var attributes = parentNode.Attributes;
            var xmlPatchNodeList = (XmlPatchNodeList) null;
      while (true)
      {
        var name = PathDescriptorParser.ReadAttrName(path, ref pos);
        if (xmlPatchNodeList == null)
          xmlPatchNodeList = pos != path.Length ? (XmlPatchNodeList) new MultiNodeList() : (XmlPatchNodeList) new SingleNodeList();
                var namedItem = attributes.GetNamedItem(name);
        if (namedItem == null)
          PathDescriptorParser.OnNoMatchingNode(path);
        xmlPatchNodeList.AddNode(namedItem);
        if (pos != path.Length)
        {
          if (path[pos] == '|')
          {
            ++pos;
            if (path[pos] != '@')
              PathDescriptorParser.OnInvalidExpression(path);
            ++pos;
          }
          else
            PathDescriptorParser.OnInvalidExpression(path);
        }
        else
          break;
      }
      return (XmlNodeList) xmlPatchNodeList;
    }

    private static XmlNodeList SelectAllChildren(XmlNode parentNode)
    {
            var childNodes = parentNode.ChildNodes;
      if (childNodes.Count == 0)
      {
        PathDescriptorParser.OnNoMatchingNode("*");
        return (XmlNodeList) null;
      }
      if (childNodes.Count == 1)
      {
                var xmlPatchNodeList = (XmlPatchNodeList) new SingleNodeList();
        xmlPatchNodeList.AddNode(childNodes.Item(0));
        return (XmlNodeList) xmlPatchNodeList;
      }
            var enumerator = childNodes.GetEnumerator();
            var xmlPatchNodeList1 = (XmlPatchNodeList) new MultiNodeList();
      while (enumerator.MoveNext())
        xmlPatchNodeList1.AddNode((XmlNode) enumerator.Current);
      return (XmlNodeList) xmlPatchNodeList1;
    }

    private static XmlNodeList SelectChildNodes(
      XmlNode parentNode,
      string path,
      int startPos)
    {
      var pos = startPos;
            var childNodes = parentNode.ChildNodes;
      var num1 = PathDescriptorParser.ReadPosition(path, ref pos);
            var xmlPatchNodeList = pos != path.Length ? (XmlPatchNodeList) new MultiNodeList() : (XmlPatchNodeList) new SingleNodeList();
      while (true)
      {
        if (num1 <= 0 || num1 > childNodes.Count)
          PathDescriptorParser.OnNoMatchingNode(path);
                var node = childNodes.Item(num1 - 1);
        xmlPatchNodeList.AddNode(node);
        if (pos != path.Length)
        {
          if (path[pos] == '|')
            ++pos;
          else if (path[pos] == '-')
          {
            ++pos;
            var num2 = PathDescriptorParser.ReadPosition(path, ref pos);
            if (num2 <= 0 || num2 > childNodes.Count)
              PathDescriptorParser.OnNoMatchingNode(path);
            while (num1 < num2)
            {
              ++num1;
              node = node.NextSibling;
              xmlPatchNodeList.AddNode(node);
            }
            if (pos != path.Length)
            {
              if (path[pos] == '|')
                ++pos;
              else
                PathDescriptorParser.OnInvalidExpression(path);
            }
            else
              break;
          }
          num1 = PathDescriptorParser.ReadPosition(path, ref pos);
        }
        else
          break;
      }
      return (XmlNodeList) xmlPatchNodeList;
    }

    private static int ReadPosition(string str, ref int pos)
    {
      var num1 = str.IndexOfAny(PathDescriptorParser.Delimiters, pos);
      if (num1 < 0)
        num1 = str.Length;
      var num2 = int.Parse(str.Substring(pos, num1 - pos));
      pos = num1;
      return num2;
    }

    private static string ReadAttrName(string str, ref int pos)
    {
      var num = str.IndexOf('|', pos);
      if (num < 0)
        num = str.Length;
      var str1 = str.Substring(pos, num - pos);
      pos = num;
      return str1;
    }

    private static void OnInvalidExpression(string path)
    {
      XmlPatchError.Error("Invalid XDL diffgram. '{0}' is an invalid path descriptor.", path);
    }

    private static void OnNoMatchingNode(string path)
    {
      XmlPatchError.Error("Invalid XDL diffgram. No node matches the path descriptor '{0}'.", path);
    }
  }
}
