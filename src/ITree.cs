#region Copyright (c) 2017 Atif Aziz, Adrian Guerra
//
// Portions Copyright (c) 2013 Ivan Nikulin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
#endregion

namespace High5
{
    using System.Collections.Generic;

    public interface ITree<TNode, TElement, TText>
        where TElement : TNode
        where TText : TNode
    {
        bool TryGetParentNode(TNode node, out TNode parentNode);
        IEnumerable<TNode> GetChildNodes(TNode node);
        bool TryGetElement(TNode node, out TElement element);
        string GetTagName(TElement element);
        string GetNamespaceUri(TElement element);
        TNode GetTemplateContent(TElement element);
        int GetAttributeCount(TElement element);
        int ListAttributes(TElement element, (string Namespace, string Name, string Value)[] attributes, int offset, int count);
        bool TryGetText(TNode node, out TText text);
        string GetTextContent(TText text);
        bool TryGetCommentData(TNode node, out string data);
        bool TryGetDocumentTypeName(TNode node, out string name);
    }
}
