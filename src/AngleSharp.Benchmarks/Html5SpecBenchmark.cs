using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnostics.dotTrace;

namespace AngleSharp.Benchmarks
{
    [MemoryDiagnoser, ShortRunJob]
    public class HtmlParseBenchmark
    {
        private static readonly HtmlParser _parser = new();
        private static readonly UrlTests _websites = CreateWebsites();
        private string _combined;

        private static UrlTests CreateWebsites()
        {
            var websites = new UrlTests(".html", true);
            websites.Include(
                //"https://www.amazon.com",
                //"https://www.reddit.com",
                "https://www.w3.org/TR/html5/single-page.html",
                "https://en.wikipedia.org/wiki/South_African_labour_law",
                //"https://www.time.com",
                "https://github.com/trending",
                "https://html.spec.whatwg.org/",
                "https://developer.mozilla.org/en-US/docs/Web/HTML",
                "https://en.wikipedia.org/wiki/Cascading_Style_Sheets"
                //"https://www.reuters.com/"
            ).GetAwaiter().GetResult();
            return websites;
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _combined = String.Concat(_websites.Tests.Select(t => t.Source));
        }

        [Benchmark]
        public IDocument Parse()
        {
            return _parser.ParseDocument(_combined);
        }
    }

    [MemoryDiagnoser, MediumRunJob]
    public class HtmlSelectorBenchmark
    {
        private static readonly HtmlParser _parser = new();
        private static readonly UrlTests _websites = CreateWebsites();
        private IDocument _document;

        private static UrlTests CreateWebsites()
        {
            var websites = new UrlTests(".html", true);
            websites.Include(
                //"https://www.amazon.com",
                //"https://www.reddit.com",
                "https://www.w3.org/TR/html5/single-page.html",
                "https://en.wikipedia.org/wiki/South_African_labour_law",
                //"https://www.time.com",
                "https://github.com/trending",
                "https://html.spec.whatwg.org/",
                "https://developer.mozilla.org/en-US/docs/Web/HTML",
                "https://en.wikipedia.org/wiki/Cascading_Style_Sheets"
                //"https://www.reuters.com/"
            ).GetAwaiter().GetResult();
            return websites;
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            var combined = String.Concat(_websites.Tests.Select(t => t.Source));
            _document = _parser.ParseDocument(combined);
        }

        [Benchmark]
        public IHtmlCollection<IElement> Div() => _document.QuerySelectorAll("div");

        [Benchmark]
        public IHtmlCollection<IElement> DivP() => _document.QuerySelectorAll("div p");

        [Benchmark]
        public IHtmlCollection<IElement> AHref() => _document.QuerySelectorAll("a[href]");

        [Benchmark]
        public IHtmlCollection<IElement> ClassNote() => _document.QuerySelectorAll(".note");

        [Benchmark]
        public IHtmlCollection<IElement> Headings() => _document.QuerySelectorAll("h1, h2, h3, h4, h5, h6");

        [Benchmark]
        public IHtmlCollection<IElement> DivChildP() => _document.QuerySelectorAll("div > p");

        [Benchmark]
        public IHtmlCollection<IElement> DivClass() => _document.QuerySelectorAll("div[class]");

        [Benchmark]
        public IHtmlCollection<IElement> PFirstChild() => _document.QuerySelectorAll("p:first-child");

        [Benchmark]
        public IHtmlCollection<IElement> UlLiA() => _document.QuerySelectorAll("ul > li > a");
    }
}
