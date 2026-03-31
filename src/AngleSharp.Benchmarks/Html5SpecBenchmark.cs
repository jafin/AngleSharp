using System;
using System.Collections.Generic;
using System.Net.Http;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using BenchmarkDotNet.Attributes;

namespace AngleSharp.Benchmarks
{
    static class Html5SpecData
    {
        private static String _html;
        private static readonly Object _lock = new();

        public static String GetHtml()
        {
            if (_html is not null) return _html;

            lock (_lock)
            {
                if (_html is not null) return _html;

                using var client = new HttpClient();
                client.DefaultRequestHeaders.UserAgent.ParseAdd("AngleSharp.Benchmarks/1.0");
                _html = client.GetStringAsync("http://www.w3.org/TR/html5/single-page.html").GetAwaiter().GetResult();
                Console.WriteLine($"Downloaded HTML5 spec: {_html.Length:N0} chars");
            }

            return _html;
        }
    }

    [MemoryDiagnoser, ShortRunJob]
    public class Html5SpecParseBenchmark
    {
        private static readonly HtmlParser _parser = new();
        private String _html;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _html = Html5SpecData.GetHtml();
        }

        [Benchmark]
        public IDocument Parse()
        {
            return _parser.ParseDocument(_html);
        }
    }

    [MemoryDiagnoser, ShortRunJob]
    public class Html5SpecSelectorBenchmark
    {
        private static readonly HtmlParser _parser = new();
        private IDocument _document;

        [GlobalSetup]
        public void GlobalSetup()
        {
            var html = Html5SpecData.GetHtml();
            _document = _parser.ParseDocument(html);
        }

        [ParamsSource(nameof(GetSelectors))]
        public String Selector { get; set; }

        public IEnumerable<String> GetSelectors =>
        [
            "div",
            "div p",
            "div > p",
            "a[href]",
            ".note",
            "h1, h2, h3, h4, h5, h6",
            "p:first-child",
            "div[class]",
            "ul > li > a",
            "div.example",
        ];

        [Benchmark]
        public IHtmlCollection<IElement> QuerySelectorAll()
        {
            return _document.QuerySelectorAll(Selector);
        }
    }
}
