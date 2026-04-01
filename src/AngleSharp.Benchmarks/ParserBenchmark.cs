using System;
using System.Linq;
using AngleSharp.Html.Parser;
using AngleSharp.Text;
using BenchmarkDotNet.Attributes;

#if NETFRAMEWORK
using CsQuery;
using CsQuery.ExtensionMethods.Internal;
using CsQuery.HtmlParser;
#endif

using HtmlAgilityPack;

namespace AngleSharp.Benchmarks
{
    [MemoryDiagnoser, ShortRunJob]
    public class ParserBenchmark
    {
        private static readonly HtmlParser angleSharpParser = new();
        private static readonly UrlTests _websites = CreateWebsites();
        private UrlTest[] _sources;

        private static UrlTests CreateWebsites()
        {
            var websites = new UrlTests(".html", true);
            websites.Include(
                "https://www.amazon.com",
                "https://html.spec.whatwg.org/",
                "https://www.youtube.com",
                "https://www.weibo.com",
                "https://www.yahoo.com",
                "https://www.google.com",
                "https://www.linkedin.com",
                "https://www.pinterest.com",
                "https://news.google.com",
                "https://www.baidu.com",
                "https://www.ebay.com",
                "https://www.msn.com",
                "https://www.nbc.com",
                "https://www.qq.com",
                "https://www.florian-rappl.de",
                "https://www.stackoverflow.com",
                "https://www.html5rocks.com/en",
                "https://www.taobao.com",
                "https://www.huffingtonpost.com",
                "https://www.wordpress.org",
                "https://www.myspace.com",
                "https://www.flickr.com",
                "https://www.godaddy.com",
                "https://www.reddit.com",
                "https://www.nytimes.com",
                "https://www.pcmag.com",
                "https://www.sitepoint.com",
                "https://html5test.com",
                "https://www.spiegel.de",
                "https://www.tmall.com",
                "https://www.sohu.com",
                "https://www.vk.com",
                "https://www.wordpress.com",
                "https://www.bing.com",
                "https://www.tumblr.com",
                "https://www.ask.com",
                "https://www.mail.ru",
                "https://www.w3.org/TR/html5/single-page.html",
                "https://www.360.cn",
                "https://www.163.com",
                "https://www.neobux.com",
                "https://www.aliexpress.com",
                "https://www.netflix.com",
                "http://en.wikipedia.org/wiki/South_African_labour_law").GetAwaiter().GetResult();
            return websites;
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _sources = _websites.Tests.Where(t => t != null).ToArray();
        }

#if NETFRAMEWORK
        [Benchmark]
        public void CsQuery()
        {
            foreach (var test in _sources)
            {
                var factory = new ElementFactory(DomIndexProviders.Simple);
                using var stream = test.Source.ToStream();
                factory.Parse(stream, System.Text.Encoding.UTF8);
            }
        }
#endif

        [Benchmark]
        public void HTMLAgilityPack()
        {
            foreach (var test in _sources)
            {
                var document = new HtmlDocument();
                document.LoadHtml(test.Source);
            }
        }

        [Benchmark]
        public void AngleSharp()
        {
            foreach (var test in _sources)
            {
                angleSharpParser.ParseDocument(test.Source);
            }
        }

        [Benchmark]
        public void ArrayPool()
        {
            foreach (var test in _sources)
            {
                using var _ = angleSharpParser.ParseDocument(test.Source.AsMemory());
            }
        }
    }
}
