#region Copyright (c) 2017 Atif Aziz
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

namespace High5.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Xunit;
    using static HtmlNodeFactory;

    public class HtmlTreeTests
    {
        static HtmlTree<HtmlNode> DefaultHtmlTree => default;

        [Fact]
        public void DefaultNodeIsNull() =>
            Assert.Null(default(HtmlTree<HtmlNode>).Node);

        [Fact]
        public void DefaultIsEmpty() =>
            Assert.True(DefaultHtmlTree.IsEmpty);

        [Fact]
        public void DefaultHasChildNodesIsFalse() =>
            Assert.False(DefaultHtmlTree.HasChildNodes);

        [Fact]
        public void DefaultChildNodeCountIsZero() =>
            Assert.Equal(0, DefaultHtmlTree.ChildNodeCount);

        [Fact]
        public void DefaultChildNodesIsEmpty() =>
            Assert.Empty(DefaultHtmlTree.ChildNodes);

        [Fact]
        public void DefaultFirstChildIsNull() =>
            Assert.Null(DefaultHtmlTree.FirstChild);

        [Fact]
        public void DefaultLastChildIsNull() =>
            Assert.Null(DefaultHtmlTree.LastChild);

        [Fact]
        public void DefaultHasParentIsFalse() =>
            Assert.False(DefaultHtmlTree.HasParent);

        [Fact]
        public void DefaultParentIsNull() =>
            Assert.Null(DefaultHtmlTree.Parent);

        [Fact]
        public void DefaultHashCodeIsZero() =>
            Assert.Equal(0, DefaultHtmlTree.GetHashCode());

        [Fact]
        public void DefaultEqualsSelf()
        {
            Assert.True(DefaultHtmlTree.Equals(DefaultHtmlTree));
            Assert.True(DefaultHtmlTree.Equals((object) DefaultHtmlTree));
            Assert.True(DefaultHtmlTree == default);
            Assert.False(DefaultHtmlTree != default);
        }

        [Fact]
        public void DefaultNotEqualsNull() =>
            Assert.False(DefaultHtmlTree.Equals(null));

        [Fact]
        public void DefaultNotEqualsOther() =>
            Assert.False(DefaultHtmlTree.Equals(new object()));

        [Fact]
        public void DefaultDescendantsIsEmpty() =>
            Assert.Empty(DefaultHtmlTree.DescendantNodes());

        [Fact]
        public void DefaultToStringReturnsEmptyString() =>
            Assert.Equal(string.Empty, DefaultHtmlTree.ToString());

        [Fact]
        public void AsBaseNodeReturnsTheSameNode()
        {
            var tree = HtmlTree.Create(DocumentFragment(Element("div")));
            var div = tree.FirstChild.Value;
            var baseNode = div.AsBaseNode();
            Assert.Same(div.Node, baseNode.Node);
            Assert.Same(tree.Node, baseNode.Parent.Value.Node);
        }

        [Fact]
        public void CastReturnsTheSameNodeTyped()
        {
            HtmlElement htmlElement;
            var tree = HtmlTree.Create(DocumentFragment(htmlElement = Element("div")));
            var treeElement = tree.FirstChild.Value.Cast<HtmlElement>();
            Assert.Same(htmlElement, treeElement.Node);
            Assert.Same(tree.Node, treeElement.Parent.Value.Node);
        }

        [Fact]
        public void AsWithWrongType() =>
            Assert.Equal(default, HtmlTree.Create(Element("div")).As<HtmlDocument>());

        [Fact]
        public void CastWithWrongTypeThrowsInvalidCastException() =>
            Assert.Throws<InvalidCastException>(() =>
                HtmlTree.Create(Element("div")).Cast<HtmlDocument>());

        [Fact]
        public void TryAsReturnsTheSameNodeTyped()
        {
            HtmlElement htmlElement;
            var tree = HtmlTree.Create(DocumentFragment(htmlElement = Element("div")));
            Assert.True(tree.FirstChild.Value.TryAs<HtmlElement>(out var treeElement));
            Assert.Same(htmlElement, treeElement.Node);
            Assert.Same(tree.Node, treeElement.Parent.Value.Node);
        }

        [Fact]
        public void TryAsWithWrongTypeReturnsFalse() =>
            Assert.False(HtmlTree.Create(Element("div")).TryAs<HtmlDocument>(out var _));

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void MatchReturnsResultBasedOnNodeType(int cases)
        {
            var element = HtmlTree.Create(Element("div"));

            var matches = new Func<HtmlTree<HtmlNode>, HtmlTree<HtmlElement>>[]
            {
                /* 0 */ null,
                /* 1 */ null,

                node => node.Match((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlElement> e)  => e,
                                   _                          => throw new InvalidOperationException()),

                node => node.Match((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlElement> e)  => e,
                                   (HtmlTree<HtmlText> _)     => throw new InvalidOperationException(),
                                   _                          => throw new InvalidOperationException()),

                node => node.Match((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlElement> e)  => e,
                                   (HtmlTree<HtmlText> _)     => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlComment> _)  => throw new InvalidOperationException(),
                                   _                          => throw new InvalidOperationException()),
            };

            Assert.InRange(cases, 2, 4);
            var result = matches[cases](element.AsBaseNode());
            Assert.Equal(element, result);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void MatchWithMismatchReturnsDefaultCase(int cases)
        {
            var tree = HtmlTree.Create(DocumentFragment(Element("div")));
            var mismatch = new object();

            var matches = new Func<HtmlTree<HtmlNode>, object>[]
            {
                /* 0 */ null,
                /* 1 */ null,

                node => node.Match((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlElement> e)  => throw new InvalidOperationException(),
                                   _                          => mismatch),

                node => node.Match((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlElement> e)  => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlText> _)     => throw new InvalidOperationException(),
                                   _                          => mismatch),

                node => node.Match((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlElement> e)  => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlText> _)     => throw new InvalidOperationException(),
                                   (HtmlTree<HtmlComment> _)  => throw new InvalidOperationException(),
                                   _                          => mismatch),
            };

            Assert.InRange(cases, 2, 4);
            var result = matches[cases](tree.AsBaseNode());
            Assert.Equal(mismatch, result);
        }

        [Theory]
        [InlineData(2, 1)]
        [InlineData(2, 2)]
        [InlineData(3, 1)]
        [InlineData(3, 2)]
        [InlineData(3, 3)]
        [InlineData(4, 1)]
        [InlineData(4, 2)]
        [InlineData(4, 3)]
        [InlineData(4, 4)]
        public void ChooseReturnsResultsBasedOnNodeTypesWhileSkippingMismatches(int cases, int position)
        {
            HtmlElement p1, p2, p3, hr;
            var tree =
                HtmlTree.Create(
                    DocumentFragment(
                        p1 = Element("p", "foo"),
                        Comment("comment"),
                        p2 = Element("p", "bar"),
                        p3 = Element("p", "baz"),
                        hr = Element("hr"),
                        Text("qux")));

            var matches = new[]
            {
                /* 0 */ null,
                /* 1 */ null,

                new Func<IEnumerable<HtmlTree<HtmlNode>>, IEnumerable<HtmlTree<HtmlElement>>>[]
                {
                    nodes => nodes.Choose((HtmlTree<HtmlElement>  e) => e,
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException()),

                    nodes => nodes.Choose((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlElement>  e) => e),
                },
                new Func<IEnumerable<HtmlTree<HtmlNode>>, IEnumerable<HtmlTree<HtmlElement>>>[]
                {
                    nodes => nodes.Choose((HtmlTree<HtmlElement>  e) => e,
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException()),

                    nodes => nodes.Choose((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlElement>  e) => e,
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException()),

                    nodes => nodes.Choose((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlElement>  e) => e),
                },
                new Func<IEnumerable<HtmlTree<HtmlNode>>, IEnumerable<HtmlTree<HtmlElement>>>[]
                {
                    nodes => nodes.Choose((HtmlTree<HtmlElement>  e) => e,
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException()),

                    nodes => nodes.Choose((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlElement>  e) => e,
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException()),

                    nodes => nodes.Choose((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlElement>  e) => e,
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException()),

                    nodes => nodes.Choose((HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlDocument> _) => throw new InvalidOperationException(),
                                         (HtmlTree<HtmlElement>  e) => e),
                }
            };

            Assert.InRange(cases, 2, 4);
            Assert.InRange(position, 1, cases);

            var result =
                from e in matches[cases][position - 1](tree.DescendantNodes())
                select e.Node;

            Assert.Equal(new[] { p1, p2, p3, hr }, result);
        }

        [Fact]
        public void AllRelationsAreReflectedCorrectlyThroughTree()
        {
            HtmlElement html, head, body, p;
            var doc =
                Document(
                    html = Element("html",
                        head = Element("head",
                            Element("title", "Example")),
                        body = Element("body",
                            Element("h1", Attributes(("class", "main")),
                                "Heading"),
                            Comment("content start"),
                            p = Element("p",
                                "Lorem ipsum dolor sit amet, consectetur adipiscing elit."),
                            Element("p",
                                "The quick brown fox jumps over the lazy dog."),
                            Comment("content end"))));

            var tree = HtmlTree.Create(doc);
            Assert.Same(doc, tree.Node);
            Assert.False(tree.HasParent);
            Assert.True(tree.HasChildNodes);
            Assert.Equal(1, tree.ChildNodeCount);
            Assert.True(tree.FirstChild.Equals(tree.LastChild));

            var treeHtml = tree.ChildNodes.First();
            Assert.Same(html, treeHtml.Node);
            Assert.True(tree.Equals(treeHtml.Parent));
            Assert.Equal(2, treeHtml.ChildNodeCount);
            Assert.False(treeHtml.FirstChild.Equals(treeHtml.LastChild));

            var treeHead = treeHtml.ChildNodes.First();
            Assert.True(treeHead.Equals(treeHtml.FirstChild));
            Assert.Same(head, treeHead.Node);
            Assert.True(treeHead.Parent == treeHtml);
            Assert.Equal(1, treeHead.ChildNodeCount);

            var treeBody = treeHtml.ChildNodes.Last();
            Assert.True(treeBody.Equals(treeHtml.LastChild));
            Assert.Same(body, treeBody.Node);
            Assert.True(treeBody.Parent == treeHtml);
            Assert.Equal(5, treeBody.ChildNodeCount);

            Assert.True(treeHead.NextSibling == treeBody);
            Assert.Null(treeHead.PreviousSibling);

            Assert.True(treeBody.PreviousSibling == treeHead);
            Assert.Null(treeBody.NextSibling);

            Assert.Equal(doc.DescendantNodes(),
                         from d in tree.DescendantNodes()
                         select d.Node);

            Assert.Equal(doc.DescendantNodesAndSelf(),
                         from d in tree.DescendantNodesAndSelf()
                         select d.Node);

            Assert.Equal(html.Elements(),
                         from e in treeHtml.Elements()
                         select e.Node);

            Assert.Equal(html.Descendants(),
                         from e in treeHtml.Descendants()
                         select e.Node);

            Assert.Equal(treeBody.ChildNodes.Skip(1),
                         treeBody.FirstChild?.NodesAfterSelf());

            Assert.Equal(treeBody.ChildNodes.Skip(1).Elements(),
                         treeBody.FirstChild?.ElementsAfterSelf());

            Assert.Equal(treeBody.ChildNodes.Skip(2),
                         treeBody.FirstChild?.NextSibling?.NodesAfterSelf());

            Assert.Equal(treeBody.ChildNodes.Skip(2).Elements(),
                         treeBody.FirstChild?.NextSibling?.ElementsAfterSelf());

            Assert.Empty(treeBody.ChildNodes.Last().NodesAfterSelf());
            Assert.Empty(treeBody.Elements().Last().ElementsAfterSelf());

            Assert.Equal(treeBody.ChildNodes.Take(treeBody.ChildNodeCount - 1),
                         treeBody.LastChild?.NodesBeforeSelf());

            Assert.Equal(treeBody.ChildNodes.Take(treeBody.ChildNodeCount - 1).Elements(),
                         treeBody.LastChild?.ElementsBeforeSelf());

            Assert.Equal(treeBody.ChildNodes.Take(treeBody.ChildNodeCount - 2),
                         treeBody.LastChild?.PreviousSibling?.NodesBeforeSelf());

            Assert.Equal(treeBody.ChildNodes.Take(treeBody.ChildNodeCount - 2).Elements(),
                         treeBody.LastChild?.PreviousSibling?.ElementsBeforeSelf());

            Assert.Empty(treeBody.ChildNodes.First().NodesBeforeSelf());
            Assert.Empty(treeBody.Elements().First().ElementsBeforeSelf());

            Assert.Equal(new HtmlNode[] { body, html, doc },
                         tree.DescendantNodes()
                             .Elements()
                             .Single(e => e.Node == p)
                             .AncestorNodes()
                             .Select(a => a.Node));

            var treePara = HtmlTree.Create(p);
            Assert.True(treePara.Equals(treePara.ChildNodes.Single().Parent));
        }

        [Fact]
        public void ToStringEncodesTextNodes()
        {
            var text = Text("\"2 < 3\" & \"3 > 2\"");
            const string escaped = "\"2 &lt; 3\" &amp; \"3 &gt; 2\"";

            Assert.Equal($"<p>{escaped}</p>",
                HtmlTree.Create(Element("p", text)).ToString());

            Assert.Equal(escaped,
                HtmlTree.Create(DocumentFragment(text)).ToString());
        }

        [Theory]
        [InlineData("script"   , "document.writeline('2 < 3 & 3 > 2');")]
        [InlineData("xmp"      , "<strong>This is bold text</strong> while <em>this is emphasized text.</em>")]
        [InlineData("style"    , "div.article > a[href] {} /* < & > */")]
        [InlineData("iframe"   , "<p>Your browser <em>does not</em> support inline frames.</p>")]
        [InlineData("noembed"  , "<img src='poster.gif' alt='Poster'>")]
        [InlineData("noframes" , "<p>Your browser <em>does not</em> support frames.</p>")]
        [InlineData("noscript" , "<p>Your browser <em>does not</em> support scripting.</p>")]
        [InlineData("plaintext", "<h1>Heading</h1><p>!Lorem ipsum dolor sit amet</p>")]
        public void ToStringDoesNotEncodeTextOfSomeElements(string tag, string text)
        {
            var tree = HtmlTree.Create(Element(tag, text));
            Assert.Equal($"<{tag}>{text}</{tag}>", tree.ToString());
            Assert.Equal(text, tree.FirstChild.ToString());
        }
    }
}
