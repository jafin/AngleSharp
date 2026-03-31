using System;
using System.Collections.Generic;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace AngleSharp.Benchmarks
{
    [MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams), ShortRunJob]
    public class HtmlParseBenchmark
    {
        private static readonly HtmlParser _parser = new();

        [ParamsSource(nameof(GetSources))]
        public UrlTest UrlTest { get; set; }

        public IEnumerable<UrlTest> GetSources()
        {
            var websites = new UrlTests(".html", true);
            websites.Include(
                "https://www.amazon.com",
                "https://www.reddit.com",
                "https://www.w3.org/TR/html5/single-page.html",
                "https://en.wikipedia.org/wiki/South_African_labour_law",
                "https://www.time.com"
            ).GetAwaiter().GetResult();
            return websites.Tests;
        }

        [Benchmark]
        public IDocument Parse()
        {
            return _parser.ParseDocument(UrlTest.Source);
        }
    }

    [MemoryDiagnoser, GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByParams), ShortRunJob]
    public class HtmlSelectorBenchmark
    {
        private static readonly HtmlParser _parser = new();
        private IDocument _document;

        [ParamsSource(nameof(GetSources))]
        public UrlTest UrlTest { get; set; }

        public IEnumerable<UrlTest> GetSources()
        {
            var websites = new UrlTests(".html", true);
            websites.Include(
                "https://www.amazon.com",
                "https://www.reddit.com",
                "https://www.w3.org/TR/html5/single-page.html",
                "https://en.wikipedia.org/wiki/South_African_labour_law",
                "https://www.time.com"
            ).GetAwaiter().GetResult();
            return websites.Tests;
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            _document = _parser.ParseDocument(UrlTest.Source);
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
    }
}
