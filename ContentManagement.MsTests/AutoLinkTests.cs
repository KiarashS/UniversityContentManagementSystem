﻿using ContentManagement.Common.WebToolkit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Ganss.Text.Tests
{
    [TestClass]
    public class AutoLinkTests
    {
        public static string Linkify(string url, string text = null)
        {
            return string.Format(@"<a href=""{0}"">{1}</a>", url, text ?? url);
        }

        [TestMethod]
        public void SimpleTest()
        {
            var s = "Check out http://www.example.com/bla?x=y#test. Over.";
            var a = new AutoLink().Link(s);
            Assert.AreEqual(@"Check out <a href=""http://www.example.com/bla?x=y#test"">http://www.example.com/bla?x=y#test</a>. Over.", a);
        }

        [TestMethod]
        public void ParensTest()
        {
            var url = "http://en.wikipedia.org/wiki/Link_(film)";
            var text = "Check it out at Wikipedia ({0}).";

            var a = new AutoLink().Link(string.Format(text, url));
            Assert.AreEqual(string.Format(text, Linkify(url)), a);

            text = "This film is huge [{0}].";
            a = new AutoLink().Link(string.Format(text, url));
            Assert.AreEqual(string.Format(text, Linkify(url)), a);
        }

        [TestMethod]
        public void PunctuationTest()
        {
            var url = "http://a.b/c?d#e";
            var text = "Check out {0}{1}{1}{1}";
            foreach (var p in new[] { '.', ';', '!', '?', ':', ',' })
            {
                var a = new AutoLink().Link(string.Format(text, url, p));
                Assert.AreEqual(string.Format(text, Linkify(url), p), a);
            }
        }

        [TestMethod]
        public void MailTest()
        {
            var uri = "mailto:hallo@example.com";
            var text = "His email is {0}.";
            var a = new AutoLink().Link(string.Format(text, uri));
            Assert.AreEqual(string.Format(text, Linkify(uri)), a);
        }

        [TestMethod]
        public void WwwTest()
        {
            var url = "www.example.com";
            var text = "Check it out at {0}.";

            var a = new AutoLink().Link(string.Format(text, url));
            Assert.AreEqual(string.Format(text, Linkify("http://" + url, url)), a);
        }

        [TestMethod]
        public void OtherSchemesTest()
        {
            var url = "gopher://bla";
            var text = "Check it out on Gopher: {0}.";

            var a = new AutoLink().Link(string.Format(text, url));
            Assert.AreEqual(string.Format(text, url), a);

            var b = new AutoLink(AutoLink.DefaultSchemes.Concat(new[] { "gopher://" })).Link(string.Format(text, url));
            Assert.AreEqual(string.Format(text, Linkify(url)), b);
        }

        [TestMethod]
        public void MultipleLinksTest()
        {
            var urls = new[] { "https://www.google.com/", "mailto:xyz@example.com", "ftp://test:test@example.com/hallo.ballo" };
            var text = "Click here: {0}! Mail me at {1}. Download at {2}.";
            var a = new AutoLink().Link(string.Format(text, urls));
            Assert.AreEqual(string.Format(text, urls.Select(u => Linkify(u)).ToArray()), a);
        }

        [TestMethod]
        public void CustomReplaceTest()
        {
            var url = "http://www.example.com";
            var text = "Check it out: {0}!";
            Func<string, string> r = u => string.Format(@"<a class=""button"" href=""{0}"">Click here</a>", u);
            var a = new AutoLink().Link(string.Format(text, url), r);
            Assert.AreEqual(string.Format(text, r(url)), a);
        }
    }
}
