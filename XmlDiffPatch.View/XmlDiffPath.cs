// Decompiled with JetBrains decompiler
// Type: Microsoft.XmlDiffPatch.XmlDiffPath
// Assembly: XmlDiffPatch.View, Version=1.0.1493.40755, Culture=neutral, PublicKeyToken=null
// MVID: 0D4C313F-7E60-4DB8-9CA5-4749E3A923DE
// Assembly location: C:\development\Trunk\UnitTests\NugetRepository\NUnit3\packages\XMLDiffPatch.1.0.8.28\lib\net\XmlDiffPatch.View.dll

using System;
using System.Diagnostics;

namespace Microsoft.XmlDiffPatch
{
  internal class XmlDiffPath
  {
    private static char[] Delimites = new char[3]
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

    internal static XmlDiffPathNodeList SelectNodes(
      XmlDiffViewParentNode rootNode,
      XmlDiffViewParentNode currentParentNode,
      string xmlDiffPathExpr)
    {
      switch (xmlDiffPathExpr[0])
      {
        case '*':
          if (xmlDiffPathExpr.Length == 1)
            return XmlDiffPath.SelectAllChildren(currentParentNode);
          XmlDiffPath.OnInvalidExpression(xmlDiffPathExpr);
          return (XmlDiffPathNodeList) null;
        case '/':
          return XmlDiffPath.SelectAbsoluteNodes(rootNode, xmlDiffPathExpr);
        case '@':
          if (xmlDiffPathExpr.Length < 2)
            XmlDiffPath.OnInvalidExpression(xmlDiffPathExpr);
          return xmlDiffPathExpr[1] == '*' ? XmlDiffPath.SelectAllAttributes((XmlDiffViewElement) currentParentNode) : XmlDiffPath.SelectAttributes((XmlDiffViewElement) currentParentNode, xmlDiffPathExpr);
        default:
          return XmlDiffPath.SelectChildNodes(currentParentNode, xmlDiffPathExpr, 0);
      }
    }

    private static XmlDiffPathNodeList SelectAbsoluteNodes(
      XmlDiffViewParentNode rootNode,
      string path)
    {
      Debug.Assert(path[0] == '/');
      int pos = 1;
      XmlDiffViewNode node = (XmlDiffViewNode) rootNode;
      int startPos;
      while (true)
      {
        startPos = pos;
        int num = XmlDiffPath.ReadPosition(path, ref pos);
        if (pos == path.Length || path[pos] == '/')
        {
          if (node.FirstChildNode == null)
            XmlDiffPath.OnNoMatchingNode(path);
          XmlDiffViewParentNode diffViewParentNode = (XmlDiffViewParentNode) node;
          if (num <= 0 || num > diffViewParentNode._sourceChildNodesCount)
            XmlDiffPath.OnNoMatchingNode(path);
          node = diffViewParentNode.GetSourceChildNode(num - 1);
          if (pos != path.Length)
            ++pos;
          else
            break;
        }
        else if (path[pos] != '-' && path[pos] != '|')
          XmlDiffPath.OnInvalidExpression(path);
        else
          goto label_10;
      }
      XmlDiffPathNodeList diffPathNodeList = (XmlDiffPathNodeList) new XmlDiffPathSingleNodeList();
      diffPathNodeList.AddNode(node);
      return diffPathNodeList;
label_10:
      if (node.FirstChildNode == null)
        XmlDiffPath.OnNoMatchingNode(path);
      return XmlDiffPath.SelectChildNodes((XmlDiffViewParentNode) node, path, startPos);
    }

    private static XmlDiffPathNodeList SelectAllAttributes(
      XmlDiffViewElement parentElement)
    {
      if (parentElement._attributes == null)
      {
        XmlDiffPath.OnNoMatchingNode("@*");
        return (XmlDiffPathNodeList) null;
      }
      if (parentElement._attributes._nextSibbling == null)
      {
        XmlDiffPathNodeList diffPathNodeList = (XmlDiffPathNodeList) new XmlDiffPathSingleNodeList();
        diffPathNodeList.AddNode((XmlDiffViewNode) parentElement._attributes);
        return diffPathNodeList;
      }
      XmlDiffPathNodeList diffPathNodeList1 = (XmlDiffPathNodeList) new XmlDiffPathMultiNodeList();
      XmlDiffViewAttribute attributes = parentElement._attributes;
      while (attributes != null)
        diffPathNodeList1.AddNode((XmlDiffViewNode) attributes);
      return diffPathNodeList1;
    }

    private static XmlDiffPathNodeList SelectAttributes(
      XmlDiffViewElement parentElement,
      string path)
    {
      Debug.Assert(path[0] == '@');
      int pos = 1;
      XmlDiffPathNodeList diffPathNodeList = (XmlDiffPathNodeList) null;
      while (true)
      {
        string name = XmlDiffPath.ReadAttrName(path, ref pos);
        if (diffPathNodeList == null)
          diffPathNodeList = pos != path.Length ? (XmlDiffPathNodeList) new XmlDiffPathMultiNodeList() : (XmlDiffPathNodeList) new XmlDiffPathSingleNodeList();
        XmlDiffViewAttribute attribute = parentElement.GetAttribute(name);
        if (attribute == null)
          XmlDiffPath.OnNoMatchingNode(path);
        diffPathNodeList.AddNode((XmlDiffViewNode) attribute);
        if (pos != path.Length)
        {
          if (path[pos] == '|')
          {
            int index = pos + 1;
            if (path[index] != '@')
              XmlDiffPath.OnInvalidExpression(path);
            pos = index + 1;
          }
          else
            XmlDiffPath.OnInvalidExpression(path);
        }
        else
          break;
      }
      return diffPathNodeList;
    }

    private static XmlDiffPathNodeList SelectAllChildren(
      XmlDiffViewParentNode parentNode)
    {
      if (parentNode._childNodes == null)
      {
        XmlDiffPath.OnNoMatchingNode("*");
        return (XmlDiffPathNodeList) null;
      }
      if (parentNode._childNodes._nextSibbling == null)
      {
        XmlDiffPathNodeList diffPathNodeList = (XmlDiffPathNodeList) new XmlDiffPathSingleNodeList();
        diffPathNodeList.AddNode(parentNode._childNodes);
        return diffPathNodeList;
      }
      XmlDiffPathNodeList diffPathNodeList1 = (XmlDiffPathNodeList) new XmlDiffPathMultiNodeList();
      for (XmlDiffViewNode node = parentNode._childNodes; node != null; node = node._nextSibbling)
        diffPathNodeList1.AddNode(node);
      return diffPathNodeList1;
    }

    private static XmlDiffPathNodeList SelectChildNodes(
      XmlDiffViewParentNode parentNode,
      string path,
      int startPos)
    {
      int pos = startPos;
      XmlDiffPathNodeList diffPathNodeList;
      while (true)
      {
        int num1;
        do
        {
          num1 = XmlDiffPath.ReadPosition(path, ref pos);
          diffPathNodeList = pos != path.Length ? (XmlDiffPathNodeList) new XmlDiffPathMultiNodeList() : (XmlDiffPathNodeList) new XmlDiffPathSingleNodeList();
          if (num1 <= 0 || num1 > parentNode._sourceChildNodesCount)
            XmlDiffPath.OnNoMatchingNode(path);
          diffPathNodeList.AddNode(parentNode.GetSourceChildNode(num1 - 1));
          if (pos != path.Length)
          {
            if (path[pos] == '|')
              ++pos;
          }
          else
            goto label_15;
        }
        while (path[pos] != '-');
        ++pos;
        int num2 = XmlDiffPath.ReadPosition(path, ref pos);
        if (num2 <= 0 || num2 > parentNode._sourceChildNodesCount)
          XmlDiffPath.OnNoMatchingNode(path);
        while (num1 < num2)
        {
          ++num1;
          diffPathNodeList.AddNode(parentNode.GetSourceChildNode(num1 - 1));
        }
        if (pos != path.Length)
        {
          if (path[pos] == '|')
            ++pos;
          else
            XmlDiffPath.OnInvalidExpression(path);
        }
        else
          break;
      }
label_15:
      return diffPathNodeList;
    }

    private static int ReadPosition(string str, ref int pos)
    {
      int num1 = str.IndexOfAny(XmlDiffPath.Delimites, pos);
      if (num1 < 0)
        num1 = str.Length;
      int num2 = int.Parse(str.Substring(pos, num1 - pos));
      pos = num1;
      return num2;
    }

    private static string ReadAttrName(string str, ref int pos)
    {
      int num = str.IndexOf('|', pos);
      if (num < 0)
        num = str.Length;
      string str1 = str.Substring(pos, num - pos);
      pos = num;
      return str1;
    }

    private static void OnInvalidExpression(string path)
    {
      throw new Exception("Invalid XmlDiffPath expression: " + path);
    }

    private static void OnNoMatchingNode(string path)
    {
      throw new Exception("No matching node:" + path);
    }
  }
}
